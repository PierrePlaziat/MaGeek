using MaGeek.Data.Entities;
using MtgApiManager.Lib.Core;
using MtgApiManager.Lib.Model;
using MtgApiManager.Lib.Service;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MaGeek.Data
{

    public class CardImporter
    {

        #region Attributes

        IMtgServiceProvider MtgApi = new MtgServiceProvider();
        Queue<PendingImport> PendingImport = new Queue<PendingImport>();
        PendingImport? CurrentImport = null;
        BackgroundWorker Worker = new BackgroundWorker();
        Timer timer;

        #endregion

        #region Accessors

        public string State { get; set; }
        public int WorkerProgress { get; set; }
        public int PendingCount { 
            get { 
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

        #region CTOR

        public CardImporter()
        {
            ConfigureTimer();
            ConfigureWorker();
        }

        private void ConfigureTimer()
        {
            timer = new Timer(1000);
            timer.AutoReset=true;
            timer.Elapsed += LoopTimer;
            timer.Start();
        }

        private void LoopTimer(object sender, ElapsedEventArgs e)
        {
            UpdateInfoText();
            if (!Worker.IsBusy) CheckNextImport(); 
        }

        private void ConfigureWorker()
        {
            Worker.DoWork += DoNextImport;
            Worker.ProgressChanged += ReportImportProgress;
            Worker.WorkerReportsProgress = true;
        }

        private void ReportImportProgress(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress = e.ProgressPercentage;
        }

        #endregion

        #region Functions

        public List<ISet> GetExistingSets()
        {
            ISetService service = MtgApi.GetSetService();
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

        public void AddImportToQueue(PendingImport importation)
        {
            PendingImport.Enqueue(importation);
        }

        private void UpdateInfoText()
        {
            int i = 0;
            string s = "";
            if (CurrentImport.HasValue) s += i++ + " : " + CurrentImport.Value.mode + " - " + CurrentImport.Value.content.Split("\n")[0];
            foreach (var v in PendingImport) s += "\n" + i++ + " : " + v.mode + " - " + v.content.Split("\n")[0];
            InfoText = s;
        }

        #endregion

        #region Methods

        #region Loop

        private void CheckNextImport()
        {
            if (PendingImport.Count>0) CurrentImport = PendingImport.Dequeue();
            if (CurrentImport != null) Worker.RunWorkerAsync();
        }

        private void DoNextImport(object sender, DoWorkEventArgs e)
        {
            State = "Calling MtgApi";
            Worker.ReportProgress(0);
            List<ICard> importResult = Import(CurrentImport);

            State = "Recording cards localy";
            Worker.ReportProgress(50);
            RecordCards(importResult);

            Worker.ReportProgress(100);
            State = "Making a Deck";
            if (CurrentImport.Value.mode == ImportMode.List) MakeDeck();

            CurrentImport = null;
            State = "Done";
            App.State.ModifCollec();
        }

        #endregion

        #region Importation

        private List<ICard> Import(PendingImport? currentImport)
        {
            switch (CurrentImport.Value.mode)
            {
                case ImportMode.Set:    return ImportSet(CurrentImport.Value.content).Result;
                case ImportMode.Search: return ImportCard(CurrentImport.Value.content, false,false,true).Result;
                case ImportMode.Update: return ImportCard(CurrentImport.Value.content, true, false, false).Result;
                case ImportMode.List:   return ImportList(ParseCardList(CurrentImport.Value.content)).Result;
                default:                    return new List<ICard>();
            }
        }

        private async Task<List<ICard>> ImportCard(string cardName,bool onlyTheOne,bool skipIfExists, bool foreignIncluded)
        {
            State = "Retrieve Card : " + cardName;
            if (skipIfExists && App.Database.cards .Where(x => x.CardId == cardName).Any()) return new List<ICard>();
            List<ICard> cards = await RequestCard(cardName,foreignIncluded);
            if (onlyTheOne) cards = await FilterExactName(cards, cardName, foreignIncluded);
            return cards;
        }

        private async Task<List<ICard>> ImportSet(string content)
        {
            ICardService service = MtgApi.GetCardService();
            IOperationResult<List<ICard>> tmpResult = null;
            List<ICard> cards = new();
            int i = 1;
            do
            {
                Thread.Sleep(200);
                tmpResult = await service
                    .Where(x => x.SetName, content)
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

        private async Task<List<ICard>> ImportList(List<Tuple<int, string>> deckList)
        {
            List<ICard> cards = new();
            for (int i=0;i<deckList.Count; i++)
            {
                var foundCards = await ImportCard(deckList[i].Item2,true,true,false);
                cards.AddRange(foundCards);
                Worker.ReportProgress((i * 100 / deckList.Count) / 2);
            }
            return cards;
        }

        private List<Tuple<int, string>> ParseCardList(string cardlist)
        {
            var lines = cardlist.Split(Environment.NewLine).ToList();
            List<Tuple<int, string>> tuples = new List<Tuple<int, string>>();
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    State = "Parse List : " + line;
                    try
                    {
                        int quantity = int.Parse(line.Split(" ")[0]);
                        string name = line[(line.IndexOf(' ') + 1)..];
                        name = name.Split(" // ")[0];
                        tuples.Add(new Tuple<int, string>(quantity, name));
                    }
                    catch { };
                }
            }
            return tuples;
        }

        #endregion

        #region API interaction

        private async Task<List<ICard>> RequestCard(string cardName,bool foreignIncluded)
        {
            ICardService service = MtgApi.GetCardService();
            List<ICard> cards = new();
            // Search VO
            int nbApiCallsNedded = await GetApiCallsNeeded(service, cardName, false);
            for (int i = 1; i <= nbApiCallsNedded; i++)
            {
                Thread.Sleep(200);
                IOperationResult<List<ICard>> tmpResult = await service
                    .Where(x => x.Name, cardName)
                    .Where(x => x.Page, i)
                    .Where(x => x.PageSize, 100)
                    .AllAsync();
                if (tmpResult.IsSuccess) cards.AddRange(tmpResult.Value);
                else MessageBoxHelper.ShowMsg(tmpResult.Exception.Message);
            }
            // search Foreign
            if (foreignIncluded)
            {
                nbApiCallsNedded = await GetApiCallsNeeded(service, cardName, true);
                for (int i = 1; i <= nbApiCallsNedded; i++)
                {
                    Thread.Sleep(200);
                    IOperationResult<List<ICard>> tmpResult = await service
                        .Where(x => x.Name, cardName)
                        .Where(x => x.Language, App.State.GetForeignLanguage())
                        .Where(x => x.Page, i)
                        .Where(x => x.PageSize, 100)
                        .AllAsync();
                    if (tmpResult.IsSuccess) cards.AddRange(tmpResult.Value);
                    else MessageBoxHelper.ShowMsg(tmpResult.Exception.Message);
                }
            }
            return cards;
        }

        private async Task<int> GetApiCallsNeeded(ICardService service, string cardName, bool foreign)
        {
            IOperationResult<List<ICard>> firstResult = null;
            if (foreign) firstResult = await service
                        .Where(x => x.Name, cardName)
                        .Where(x => x.Language, App.State.GetForeignLanguage())
                        .Where(x => x.PageSize, 1)
                        .AllAsync();
            else firstResult = await service
                        .Where(x => x.Name, cardName)
                        .Where(x => x.PageSize, 1)
                        .AllAsync();
            return ((int)firstResult.PagingInfo.TotalPages / 100) + 1;
        }

        private async Task<List<ICard>> FilterExactName(List<ICard> cards,string cardName, bool foreignIncluded)
        {
            List<ICard> filteredCards = new List<ICard>();
            await Task.Run(() =>
            {
                foreach (var card in cards)
                {
                    if (IsExactCardName(cardName, card.Name)) 
                        filteredCards.Add(card);
                    if (foreignIncluded && card.ForeignNames!=null && IsExactCardName(cardName, card.ForeignNames.Where(x=>x.Language==App.State.GetForeignLanguage()).FirstOrDefault().Name)) 
                        filteredCards.Add(card);
                }
            });
            return filteredCards;
        }

        private static bool IsExactCardName(string name, string cardname)
        {
            string[] ss = name.Split(" // "); // Separate doubled sided
            foreach (string ss2 in ss)  if (ss2 == cardname) return true; // Compare each side
            return false;
        }

        #endregion

        #region Local Db interaction

        private void RecordCards(List<ICard> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                RecordCard(results[i]);
                Worker.ReportProgress((i * 100 / results.Count) / 2 + 50);
            }
        }

        private static void RecordCard(ICard cardData)
        {
            // Card
            var localCard = App.Database.cards.Where(x => x.CardId == cardData.Name).FirstOrDefault();
            if (localCard == null)
            {
                localCard = new MagicCard(cardData);
                App.Database.cards.Add(localCard);
                App.Database.SaveChanges();
            }
            // Variant
            var localVariant = localCard.Variants.Where(x => x.Id == cardData.Id).FirstOrDefault();
            if (localVariant == null)
            {
                localCard.AddVariant(cardData);
                App.Database.SaveChanges();
            }
        }

        private void MakeDeck()
        {
            var title = (CurrentImport.Value.title == null) ? DateTime.Now.ToString() : CurrentImport.Value.title;
            var deck = new MagicDeck(title);
            foreach (var cardOccurence in ParseCardList(CurrentImport.Value.content))
            {
                int quantity = cardOccurence.Item1;
                string name = cardOccurence.Item2;
                MagicCard card = App.Database.cards.Where(x => x.CardId == name).FirstOrDefault();
                if (card != null)
                {
                    MagicCardVariant variant = card.Variants[0];
                    AddCardToDeck(variant, deck, quantity);
                }
            }
            deck.CardRelations[0].RelationType = 1;
            App.Database.decks.Add(deck);
            App.Database.SaveChanges();
        }

        public void AddCardToDeck(MagicCardVariant card, MagicDeck deck, int qty, int relation = 0)
        {
            if (card == null || deck == null) return;
            var cardRelation = deck.CardRelations.Where(x => x.Card.Card.CardId == card.Card.CardId).FirstOrDefault();
            if (cardRelation == null)
            {
                cardRelation = new CardDeckRelation()
                {
                    Card = card,
                    Deck = deck,
                    Quantity = 0,
                    RelationType = relation
                };
                deck.CardRelations.Add(cardRelation);
            }
            cardRelation.Quantity += qty;
        }

        #endregion

        #endregion

    }

    #region Data Struct

    public struct PendingImport
    {

        public ImportMode mode { get; set; }
        public string content { get; set; }
        public bool asOwned { get; set; }
        public string title { get; set; }

        public PendingImport(ImportMode Mode, string Content, bool asGot = false, string title = "")
        {
            this.mode = Mode;
            this.content = Content;
            this.asOwned = asGot;
            this.title = title;
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
