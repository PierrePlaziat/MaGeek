using MaGeek.Data.Entities;
using MtgApiManager.Lib.Core;
using MtgApiManager.Lib.Model;
using MtgApiManager.Lib.Service;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace MaGeek.Data
{

    #region Embedded Data Struct

    public enum ImportMode { Set, Card, Deck }

    public struct PendingImport
    {
        public ImportMode mode;
        public string content;
        public bool asOwned;
        public string title;
        public PendingImport(ImportMode Mode, string Content, bool asGot = false, string title = "")
        {
            this.mode = Mode;
            this.content = Content;
            this.asOwned = asGot;
            this.title = title;
        }
    }

    #endregion

    /// <summary>
    /// Retrieve cards from api to local entities
    /// </summary>
    public class CardImporter
    {

        #region Attributes

        IMtgServiceProvider mtg = new MtgServiceProvider();

        Queue<PendingImport> pendingImports = new Queue<PendingImport>();
        PendingImport? currentlyImporting;

        BackgroundWorker ImportWorker = new BackgroundWorker();
        Timer loopTimer;

        #endregion

        #region CTOR

        public CardImporter()
        {
            ConfigureTimer();
            ConfigureWorker();
        }

        private void ConfigureTimer()
        {
            loopTimer = new Timer(1000);
            loopTimer.AutoReset=true;
            loopTimer.Elapsed += LoopTimer;
            loopTimer.Start();
        }

        private void ConfigureWorker()
        {
            ImportWorker.DoWork += DoNextImport;
            ImportWorker.ProgressChanged += ReportImportProgress;
            ImportWorker.RunWorkerCompleted += CompleteImportation;
        }

        #endregion

        #region PUBLIC

        public int pendingCount { get { return pendingImports.Count; } }

        public int workerProgress { get; set; }

        public List<ISet> GetAllSets()
        {
            ISetService service = mtg.GetSetService();
            IOperationResult<List<ISet>> response = null;
            List<ISet> sets = new();

            response = service.AllAsync().Result;

            if (response.IsSuccess)
            {
                sets.AddRange(response.Value);
            }
            else
            {
                var exception = response.Exception;
                MessageBoxHelper.ShowMsg(exception.Message);
            }

            return sets;
        }

        public void AddImport(PendingImport importation)
        {
            pendingImports.Enqueue(importation);
        }

        #endregion

        #region Methods

        private void LoopTimer(object sender, ElapsedEventArgs e)
        {
            if (!ImportWorker.IsBusy)  CheckNextImport();
        }

        private void CheckNextImport()
        {
            if (pendingImports.Count>0) currentlyImporting = pendingImports.Dequeue();
            if (currentlyImporting != null) ImportWorker.RunWorkerAsync();
        }

        private void DoNextImport(object sender, DoWorkEventArgs e)
        {
            ImportWorker.ReportProgress(0);
            List<Tuple<int,string>> deckList = null;
            List<ICard> importResult = new List<ICard>();
            // parse list 
            if (currentlyImporting.Value.mode==ImportMode.Deck) importResult = ImportDeck(currentlyImporting.Value.content).Result;
            ImportWorker.ReportProgress(20);
            // ask api
            switch (currentlyImporting.Value.mode)
            {
                case ImportMode.Deck: deckList = ParseCardList(currentlyImporting.Value.content); break;
                case ImportMode.Card: importResult = ImportCard(currentlyImporting.Value.content).Result; break;
                case ImportMode.Set:  importResult = ImportSet(currentlyImporting.Value.content).Result; break;
            }
            // record results
            ImportWorker.ReportProgress(50);
            foreach (ICard iCard in importResult) App.Database.cards.Add(new MagicCard(iCard));
            ImportWorker.ReportProgress(80);
            if (currentlyImporting.Value.mode == ImportMode.Deck) MakeDeck(currentlyImporting.Value.title, deckList);
            ImportWorker.ReportProgress(100);
        }

        private void ReportImportProgress(object sender, ProgressChangedEventArgs e)
        {
            workerProgress = e.ProgressPercentage;
        }

        private void CompleteImportation(object sender, RunWorkerCompletedEventArgs e)
        {
            App.Database.SaveChanges();
            currentlyImporting = null;
        }

        #region Logic

        private async Task<List<ICard>> ImportDeck(string content)
        {
            List<ICard> cards = new();
            var cardlines = ParseCardList(content);
            foreach (var cardline in cardlines)
            {
                var tmp = await ImportCard(cardline.Item2);
                cards.AddRange(tmp);
            }
            return cards;
        }

        private async Task<List<ICard>> ImportCard(string content)
        {
            ICardService service = mtg.GetCardService();
            IOperationResult<List<ICard>> tmpResult = null;
            List<ICard> cards = new();
            int i = 1;
            do
            {
                tmpResult = await service
                    .Where(x => x.Name, content)
                    .Where(x => x.Page, i)
                    .Where(x => x.PageSize, 100)
                    .AllAsync();
                if (tmpResult.IsSuccess)
                {
                    cards.AddRange(tmpResult.Value);
                }
                else
                {
                    var exception = tmpResult.Exception;
                    MessageBoxHelper.ShowMsg(exception.Message);
                }
                i++;
            }
            while (tmpResult.Value.Count > 0);
            return cards;
        }

        private async Task<List<ICard>> ImportSet(string content)
        {
            ICardService service = mtg.GetCardService();
            IOperationResult<List<ICard>> tmpResult = null;
            List<ICard> cards = new();
            int i = 1;
            do
            {
                tmpResult = await service
                    .Where(x => x.Set, content)
                    .Where(x => x.Page, i)
                    .Where(x => x.PageSize, 100)
                    .AllAsync();
                if (tmpResult.IsSuccess)
                {
                    cards.AddRange(tmpResult.Value);
                }
                else
                {
                    var exception = tmpResult.Exception;
                    MessageBoxHelper.ShowMsg(exception.Message);
                }
                i++;
            }
            while (tmpResult.Value.Count > 0);
            return cards;
        }

        private List<Tuple<int,string>> ParseCardList(string cardlist)
        {
            var lines = cardlist.Split(Environment.NewLine).ToList();
            List<Tuple<int, string>> tuples = new List<Tuple<int, string>>();
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    // TODO debug
                    int quantity = int.Parse(line.Split(" ")[0]);
                    string name = line[(line.IndexOf(' ') + 1)..];
                    name = name.Split(" // ")[0];
                    tuples.Add(new Tuple<int, string>(quantity,name) );
                }
            }
            return tuples;
        }

        private void MakeDeck(string title,List<Tuple<int, string>> deckList)
        {
            var deck = new MagicDeck(title);
            foreach (var cardOccurence in deckList)
            {
                int quantity = cardOccurence.Item1;
                string name = cardOccurence.Item2;
                MagicCard card = App.Database.cards.Where(x => x.CardId == name).FirstOrDefault();
                MagicCardVariant variant = card.Variants[0];
                if (card!=null) App.CardManager.AddCardToDeck(variant, deck, quantity);
            }
        }

        #endregion

        #endregion

    }

}