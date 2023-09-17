using MaGeek;
using MaGeek.Framework.Utils;
using MageekSdk;
using MageekSdk.Tools;
using MtgSqliveSdk;
using ScryfallApi.Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static MtgSqliveSdk.Mageek;
using Timer = System.Timers.Timer;

namespace MaGeek
{

    /// <summary>
    /// Interacts with API, called regularly to get fresh data
    /// TODO : replace BackgroundWorker by a Task list 
    /// </summary>
    public class DeckImporter
    {

        #region Attributes

        Queue<PendingImport> PendingImport = new();
        PendingImport? CurrentImport = null;

        enum ImporterState { Init, Play, Pause, Canceled }
        ImporterState State { get; set; } = ImporterState.Init;
        bool Importing { get; set; }

        Timer timer;

        //static string SaveStatePath { get { return App.Config.Path_ImporterState; } }

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

        public DeckImporter()
        {
            ConfigureTimer();
            if (PendingCount > 0)
            {
                State = ImporterState.Pause;
                Message = "Paused, " + PendingCount + " imports waiting";
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
            Logger.Log("Deck import : " + CurrentImport.Value.Title);

            while (State != ImporterState.Canceled && State == ImporterState.Pause) { };
            if (State != ImporterState.Canceled)
            {
                Message = "Making a deck";
                WorkerProgress = 75;
                await MakeADeck(CurrentImport.Value);
            }

            while (State != ImporterState.Canceled && State == ImporterState.Pause) { };
            if (State != ImporterState.Canceled)
            {
                WorkerProgress = 99;
                Message = "Finalizing";
                FinalizeImport();
            }
        }

        #region Work

        private async Task<List<DeckLine>> ParseCardList(string cardlist)
        {
            List<DeckLine> tuples = new();
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
                                    tuples.Add(new DeckLine() { Quantity = quantity, Uuid = name.Trim(), Relation = side?2:0 });
                                }
                                catch (Exception e)
                                {
                                    Logger.Log(e);
                                }

                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            return tuples;
        }

        private async Task MakeADeck(PendingImport importing)
        {
            List<DeckLine> importLines;
            importLines = await ParseCardList(importing.Content);
            await Mageek.CreateDeck_Contructed(
                importing.Title ?? DateTime.Now.ToString(),
                "",
                importLines
            );
        }

        private void FinalizeImport()
        {
            if (PendingImport.Count == 0)
            {
                App.Events.RaiseUpdateDeckList();
            }

            CurrentImport = null;
            WorkerProgress = 100;
            Message = "Done";
        }

        private void UpdateInfoText()
        {
            int i = 0;
            string s = "";
            if (CurrentImport.HasValue) s += i++ + " : " + CurrentImport.Value.Title;
            foreach (var v in PendingImport) s += "\n" + i++ + " : " + v.Title;
            InfoText = s;
        }
        private void ReportImportProgress(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress = e.ProgressPercentage;
        }

        #endregion

        #endregion

        #endregion

    }

    #region Data Struct

    public struct PendingImport
    {
        public string Content { get; set; }
        public bool AsOwned { get; set; }
        public string Title { get; set; }

        public PendingImport(string Content, bool asGot = false, string title = "")
        {
            this.Content = Content;
            AsOwned = asGot;
            Title = title;
        }

    }

    #endregion

}
