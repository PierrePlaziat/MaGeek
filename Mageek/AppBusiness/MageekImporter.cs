using Plaziat.CommonWpf;
using ScryfallApi.Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MaGeek.AppBusiness
{

    /// <summary>
    /// API helper
    /// </summary>
    public class MageekImporter
    {

        #region Attributes

        Queue<PendingImport> PendingImport = new();
        PendingImport? CurrentImport = null;

        enum ImporterState { Init, Play, Pause, Canceling }
        ImporterState state = ImporterState.Init;

        Timer timer;
        private bool isWorking;

        #region UI

        public string Message { get; set; } = "Init";
        public int WorkerProgress { get; set; }
        public int PendingCount
        {
            get
            {
                return PendingImport.Count + (CurrentImport != null ? 1 : 0);
            }
        }
        private string infoText = "";

        public string InfoText
        {
            get { return infoText; }
            set { infoText = value; }
        }

        #endregion

        #endregion

        #region CTOR

        public MageekImporter()
        {
            LoadState();
            ConfigureTimer();
            state = ImporterState.Pause;
            Message = "Init done";
        }

        private void ConfigureTimer()
        {
            timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += MainLoop;
            timer.Start();
        }

        #endregion

        #region Methods

        public void AddImportToQueue(PendingImport importation)
        {
            PendingImport.Enqueue(importation);
        }
        public void Play()
        {
            if (state == ImporterState.Pause)
            {
                state = ImporterState.Play;
                Message = "Play";
            }
        }
        public void Pause()
        {
            if (state == ImporterState.Play)
            {
                state = ImporterState.Pause;
                Message = "Pause";
            }
        }
        public void CancelAll()
        {
            Message = "Canceling";
            state = ImporterState.Canceling;
            PendingImport = new();
            CurrentImport = null;
            Message = "Canceled";
        }

        #region Import

        private void MainLoop(object sender, ElapsedEventArgs e)
        {
            if (!isWorking && state == ImporterState.Play) CheckNextImport().ConfigureAwait(false);
        }
        private async Task CheckNextImport()
        {
            if (state == ImporterState.Play)
            {
                if (PendingImport.Count > 0) CurrentImport = PendingImport.Dequeue();
                if (CurrentImport != null)
                {
                    await WorkerTask();
                }
            }
            SaveState();
            UpdateInfoText();
        }


        private async Task WorkerTask()
        {
            isWorking = true;
            if (state == ImporterState.Init) return;
            List<Card> importResult = new();
            while (state != ImporterState.Canceling && state == ImporterState.Pause) { };
            if (state != ImporterState.Canceling)
            {
                Message = "Calling MtgApi";
                WorkerProgress=(0);
                importResult = await Import(CurrentImport);
            }
            while (state != ImporterState.Canceling && state == ImporterState.Pause) { };
            if (state != ImporterState.Canceling)  //App.Events.RaisePreventUIAction(true);
            { 
                Message = "Recording cards localy";
                WorkerProgress=(50);
                await RecordCards(importResult);
            }
            while (state != ImporterState.Canceling && state == ImporterState.Pause) { };
            if (state != ImporterState.Canceling)
            {
                Message = "Making Deck";
                WorkerProgress=(75);
                await MakeItADeck(importResult);
            }
            while (state != ImporterState.Canceling && state == ImporterState.Pause) { };
            if (state != ImporterState.Canceling)
            {
                WorkerProgress=(99);
                Message = "Finalize";
                await FinalizeImportation();
            }
            isWorking = false;
        }

        #region Work

        private async Task<List<Card>> Import(PendingImport? currentImport)
        {
            List<Card> list = new();
            switch (currentImport.Value.Mode)
            {
                case ImportMode.Set: list = await MageekUtils.ImportSet(CurrentImport.Value.Content); break;
                case ImportMode.Search: list = await MageekUtils.ImportCard(CurrentImport.Value.Content, false, false, true); break;
                case ImportMode.Update: list = await MageekUtils.ImportCard(CurrentImport.Value.Content, true, false, false); break;
                case ImportMode.List: list = await ImportCardList(await ParseCardList(CurrentImport.Value.Content)); break;
            };
            return list;
        }

        private async Task<List<ImportLine>> ParseCardList(string cardlist)
        {
            List<ImportLine> tuples = new();
            try
            {
                await Task.Run(() =>
                {
                    var lines = cardlist.Split(Environment.NewLine).ToList();
                    bool side = false;
                    foreach (string line in lines)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            Message = "Parse List : " + line;
                            if (line.StartsWith("Sideboard")) side = true;
                            else
                            {
                                try
                                {
                                    int quantity = int.Parse(line.Split(" ")[0]);
                                    string name = line[(line.IndexOf(' ') + 1)..];
                                    name = name.Split(" // ")[0];
                                    tuples.Add(new ImportLine() { Quantity = quantity, Name = name.Trim(), Side = side });
                                }
                                catch { };

                            }
                        }
                    }
                });
            }
            catch (Exception e) { MessageBoxHelper.ShowError("ParseCardList", e); }
            return tuples;
        }

        private async Task<List<Card>> ImportCardList(List<ImportLine> deckList)
        {
            List<Card> cards = new();
            try
            {
                for (int i = 0; i < deckList.Count; i++)
                {
                    Message = "Retrieve Card : " + deckList[i].Name;
                    var foundCards = await MageekUtils.ImportCard(deckList[i].Name, true, true, false);
                    cards.AddRange(foundCards);
                    WorkerProgress=i * 100 / deckList.Count / 2;
                }
            }
            catch (Exception e) { MessageBoxHelper.ShowError("ImportList", e); }
            return cards;
        }

        private async Task RecordCards(List<Card> results)
        {
            bool owned = CurrentImport.HasValue && CurrentImport.Value.AsOwned;
            for (int i = 0; i < results.Count; i++)
            {
                await MageekUtils.RecordCard(results[i], owned);
                WorkerProgress=i * 100 / results.Count / 2 + 50;
            }
        }

        private async Task MakeItADeck(List<Card> importResult)
        {
            if (importResult.Count == 0) return;
            List<ImportLine> importLines = new();
            if (CurrentImport.Value.Mode == ImportMode.List)
            {
                importLines = await ParseCardList(CurrentImport.Value.Content);
            }
            if (CurrentImport.Value.Mode == ImportMode.Set)
            {
                foreach (var v in importResult) importLines.Add(new ImportLine() { Name = v.Name, Quantity = 1 });
            }
            await MageekUtils.AddDeck(
                importLines,
                CurrentImport.Value.Title ?? DateTime.Now.ToString()
            );
        }

        private async Task FinalizeImportation()
        {
            if (CurrentImport.Value.Mode == ImportMode.Search)
            {
                App.Events.RaiseUpdateCardCollec();
            }
            if (CurrentImport.Value.Mode == ImportMode.Set)
            {
                App.Events.RaiseUpdateCardCollec();
                App.Events.RaiseUpdateDeckList();
            }
            if (CurrentImport.Value.Mode == ImportMode.List)
            {
                App.Events.RaiseUpdateCardCollec();
                App.Events.RaiseUpdateDeckList();
            }
            if (CurrentImport.Value.Mode == ImportMode.Update)
            {
                var c = App.State.SelectedCard;
                App.Events.RaiseCardSelected(null);
                App.Events.RaiseCardSelected(c);
            }
            CurrentImport = null;
            WorkerProgress=100;
            Message = "Done";
        }

        #endregion

        #region Report Progress

        private void UpdateInfoText()
        {
            int i = 0;
            string s = "";
            if (CurrentImport.HasValue) s += i++ + " : " + CurrentImport.Value.Mode + " - " + CurrentImport.Value.Title;
            foreach (var v in PendingImport) s += "\n" + i++ + " : " + v.Mode + " - " + v.Title;
            InfoText = s;
        }
        private void ReportImportProgress(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress = e.ProgressPercentage;
        }

        #endregion

        #endregion

        #region SaveState

        static string StatePath { get { return App.Config.Path_ImporterState; } }

        public void SaveState()
        {
            string jsonString = "";
            var x = PendingImport.ToList();
            if (CurrentImport != null) x.Add(CurrentImport.Value);
            jsonString += JsonSerializer.Serialize(x);
            File.WriteAllText(StatePath, jsonString);
        }

        public void LoadState()
        {
            try
            {
                if (!File.Exists(StatePath)) return;
                string jsonString = File.ReadAllText(StatePath);
                File.WriteAllText(StatePath, "");
                if (string.IsNullOrEmpty(jsonString)) return;
                List<PendingImport> loadedImports = JsonSerializer.Deserialize<List<PendingImport>>(jsonString);
                PendingImport = new Queue<PendingImport>();
                foreach (var import in loadedImports) PendingImport.Enqueue(import);
            }
            catch
            {
                File.WriteAllText(StatePath, "");
            }
        }

        #endregion

        #endregion

    }

    #region Data Struct

    public struct ImportLine
    {
        public int Quantity;
        public string Name;
        public bool Side;
    }

    public struct PendingImport
    {

        public ImportMode Mode { get; set; }
        public string Content { get; set; }
        public bool AsOwned { get; set; }
        public string Title { get; set; }

        public PendingImport(ImportMode Mode, string Content, bool asGot = false, string title = "")
        {
            this.Mode = Mode;
            this.Content = Content;
            AsOwned = asGot;
            this.Title = title;
        }

    }

    public enum ImportMode
    {
        Search,
        Set,
        List,
        Update
    }

    #endregion

}
