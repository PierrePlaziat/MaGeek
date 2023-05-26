using MaGeek.Framework;
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
    /// Interacts with API, called regularly to get fresh data
    /// TODO : replace BackgroundWorker by a Task list 
    /// </summary>
    public class MageekImporter
    {

        #region Attributes

        Queue<PendingImport> PendingImport = new();
        PendingImport? CurrentImport = null;

        enum ImporterState { Init, Play, Pause, Canceled }
        ImporterState State { get; set; } = ImporterState.Init;
        bool Importing { get; set; }

        Timer timer;

        static string SaveStatePath { get { return App.Config.Path_ImporterState; } }

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
            if (PendingCount > 0)
            {
                State = ImporterState.Pause;
                Message = "Paused, "+ PendingCount + " imports waiting";
            }
            else
            {
                State = ImporterState.Play;
                Message = "";
            }
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
            if (State == ImporterState.Pause || State == ImporterState.Canceled)
            {
                State = ImporterState.Play;
                Message = "Play";
            }
        }

        public void Pause()
        {
            if (State == ImporterState.Play)
            {
                State = ImporterState.Pause;
                Message = "Pause";
            }
        }

        public void CancelAll()
        {
            Message = "Canceling";
            State = ImporterState.Canceled;
            PendingImport = new();
            CurrentImport = null;
            Message = "Canceled";
        }

        #region Import

        private void MainLoop(object sender, ElapsedEventArgs e)
        {
            if (!Importing && State == ImporterState.Play) CheckNextImport().ConfigureAwait(false);
        }

        private async Task CheckNextImport()
        {
            SaveState();
            if (State == ImporterState.Play)
            {
                if (PendingImport.Count > 0) CurrentImport = PendingImport.Dequeue();
                UpdateInfoText();
                if (CurrentImport != null)
                {
                    Importing = true;
                    await DoImport();
                    Importing = false;
                }
            }
        }

        private async Task DoImport()
        {
            List<Card> importResult = new();

            //while (state != ImporterState.Canceled && state == ImporterState.Pause) { };
            //if (state != ImporterState.Canceled)
            //{
            //    Message = "Retrieving cards";
            //    WorkerProgress=(0);
            //    importResult = await RetrieveCards(CurrentImport);
            //}

            //while (state != ImporterState.Canceled && state == ImporterState.Pause) { };
            //if (state != ImporterState.Canceled)  //App.Events.RaisePreventUIAction(true);
            //{ 
            //    Message = "Recording cards";
            //    WorkerProgress=(50);
            //    await RecordCards(importResult);
            //}

            while (State != ImporterState.Canceled && State == ImporterState.Pause) { };
            if (State != ImporterState.Canceled)
            {
                Message = "Making a deck";
                WorkerProgress=(75);
                await MakeADeck(CurrentImport.Value, importResult);
            }

            while (State != ImporterState.Canceled && State == ImporterState.Pause) { };
            if (State != ImporterState.Canceled)
            {
                WorkerProgress=(99);
                Message = "Finalizing";
                FinalizeImport();
            }

        }

        #region Work

        //private /*async Task<*/List<Card>/*>*/ RetrieveCards(PendingImport? currentImport)
        //{
        //    List<Card> list = new();
        //    //switch (currentImport.Value.Mode)
        //    //{
        //        //case ImportMode.Set: list = await MageekApi.RetrieveSetCards(CurrentImport.Value.Content); break;
        //        //case ImportMode.Search: list = await MageekApi.RetrieveCard(CurrentImport.Value.Content, false, false,true); break;
        //        //case ImportMode.Update: list = await MageekApi.RetrieveCard(CurrentImport.Value.Content, true, false, false); break;
        //        //case ImportMode.List: list = await RetrieveCardList(await ParseCardList(CurrentImport.Value.Content)); break;
        //    //};
        //    return list;
        //}

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
            catch (Exception e) { Log.Write(e); }
            return tuples;
        }

        //private async Task<List<Card>> RetrieveCardList(List<ImportLine> deckList)
        //{
        //    List<Card> cards = new();
        //    try
        //    {
        //        for (int i = 0; i < deckList.Count; i++)
        //        {
        //            Message = "Retrieve Card : " + deckList[i].Name;
        //            var foundCards = await MageekApi.RetrieveCard(deckList[i].Name, true, true,true);
        //            cards.AddRange(foundCards);
        //            WorkerProgress=i * 100 / deckList.Count / 2;
        //        }
        //    }
        //    catch (Exception e) { AppLogger.ShowError("ImportList", e); }
        //    return cards;
        //}

        //private async Task RecordCards(List<Card> results)
        //{
        //    bool owned = CurrentImport.HasValue && CurrentImport.Value.AsOwned;
        //    for (int i = 0; i < results.Count; i++)
        //    {
        //        await MageekApi.RecordCard(results[i], owned);
        //        WorkerProgress=i * 100 / results.Count / 2 + 50;
        //    }
        //}

        private async Task MakeADeck(PendingImport importing, List<Card> importResult)
        {
            if (importing.Mode == ImportMode.Search || importing.Mode == ImportMode.Update) return;
            //if (importResult.Count == 0) return;
            List<ImportLine> importLines = new();
            if (importing.Mode == ImportMode.List)
            {
                importLines = await ParseCardList(importing.Content);
            }
            if (importing.Mode == ImportMode.Set)
            {
                foreach (var v in importResult) importLines.Add(new ImportLine() { Name = v.Name, Quantity = 1 });
            }
            await MageekCollection.AddDeck(
                importLines,
                importing.Title ?? DateTime.Now.ToString()
            );
        }

        private void FinalizeImport()
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
                    //var c = App.State.SelectedCard;
                    //if (c!=null) App.Events.RaiseCardSelected(c);
                }
                CurrentImport = null;
                WorkerProgress = 100;
                Message = "Done";
        }

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

        public void SaveState()
        {
            try
            {
                string jsonString = "";
                var x = PendingImport.ToList();
                if (CurrentImport != null) x.Add(CurrentImport.Value);
                jsonString += JsonSerializer.Serialize(x);
                File.WriteAllText(SaveStatePath, jsonString);
            }
            catch (Exception e) { Log.Write(e); }
        }

        public void LoadState()
        {
            try
            {
                if (!File.Exists(SaveStatePath)) return;
                string jsonString = File.ReadAllText(SaveStatePath);
                File.WriteAllText(SaveStatePath, "");
                if (string.IsNullOrEmpty(jsonString)) return;
                List<PendingImport> loadedImports = JsonSerializer.Deserialize<List<PendingImport>>(jsonString);
                PendingImport = new Queue<PendingImport>();
                foreach (var import in loadedImports) PendingImport.Enqueue(import);
            }
            catch (Exception e)
            {
                File.WriteAllText(SaveStatePath, "");
                Log.Write(e);
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
