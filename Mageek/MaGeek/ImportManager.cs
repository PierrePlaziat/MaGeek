using MaGeek.Data.Entities;
using MtgApiManager.Lib.Core;
using MtgApiManager.Lib.Model;
using MtgApiManager.Lib.Service;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MaGeek
{

    public class ImportManager
    {

        #region Attributes

        IMtgServiceProvider MtgApi = new MtgServiceProvider();
        Queue<PendingImport> PendingImport = new Queue<PendingImport>();
        PendingImport? CurrentImport = null;
        BackgroundWorker Worker = new BackgroundWorker();
        Timer timer;

        public string Message { get; set; }
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

        enum ImporterState { Init, Play, Pause, Cancel }
        ImporterState state = ImporterState.Init;

        #endregion

        #region CTOR

        public ImportManager()
        {
            ConfigureTimer();
            ConfigureWorker();
            InitImporter();
        }

        private void InitImporter()
        {
            LoadState();
            state = ImporterState.Play;
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
            Worker.DoWork += Work_DoNextImport;
            Worker.ProgressChanged += ReportImportProgress;
            Worker.WorkerReportsProgress = true;
        }

        private void ReportImportProgress(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress = e.ProgressPercentage;
        }

        #endregion

        #region Methods

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


        public void PlayImports()
        {
            if (state==ImporterState.Pause) state = ImporterState.Play;
        }
        public void PauseImports()
        {
            if (state == ImporterState.Play) state = ImporterState.Pause;
        }
        public void CancelImport()
        {
            if (state == ImporterState.Play) state = ImporterState.Cancel;
        }

        private void UpdateInfoText()
        {
            int i = 0;
            string s = "";
            if (CurrentImport.HasValue) s += i++ + " : " + CurrentImport.Value.mode + " - " + CurrentImport.Value.content.Split("\n")[0];
            foreach (var v in PendingImport) s += "\n" + i++ + " : " + v.mode + " - " + v.content.Split("\n")[0];
            InfoText = s;
        }


        #region Worker

        private void CheckNextImport()
        {
            if (state != ImporterState.Play) return;
            if (PendingImport.Count>0) CurrentImport = PendingImport.Dequeue();
            if (CurrentImport != null) Worker.RunWorkerAsync();
        }

        private void Work_DoNextImport(object sender, DoWorkEventArgs e)
        {
            List<ICard> importResult = new List<ICard>();
            if (state == ImporterState.Cancel) Work_Cancel(); else importResult = Work_CallApi().Result;
            if (state == ImporterState.Cancel) Work_Cancel(); else Work_RecordLocal(importResult);
            if (state == ImporterState.Cancel) Work_Cancel(); else Work_MakeADeck();
            Work_Finalize();
        }

        private async Task<List<ICard>> Work_CallApi()
        {
            Message = "Calling MtgApi";
            Worker.ReportProgress(0);
            return await Import(CurrentImport); 
        }

        private void Work_RecordLocal(List<ICard> importResult)
        {
            Message = "Recording cards localy";
            Worker.ReportProgress(50);
            RecordCards(importResult);
        }

        private void Work_MakeADeck()
        {
            Message = "Making a Deck";
            Worker.ReportProgress(75);
            if (CurrentImport.Value.mode == ImportMode.List) MakeDeck();
        }

        private void Work_Finalize()
        {
            Worker.ReportProgress(99);
            Message = "Finalize";

            CurrentImport = null;
            App.STATE.RaiseUpdateCardCollec();
            App.Current.Dispatcher.Invoke(new Action(() => { 
                App.STATE.RaiseUpdateDeckList();
                var c = App.STATE.SelectedCard;
                App.STATE.RaiseCardSelected(null);
                App.STATE.RaiseCardSelected(c);
            }));

            Worker.ReportProgress(100);
            Message = "Done";
        }

        private void Work_Cancel()
        {
            state = ImporterState.Pause;
        }

        #endregion

        #region Importation

        private async Task<List<ICard>> Import(PendingImport? currentImport)
        {
            switch (CurrentImport.Value.mode)
            {
                case ImportMode.Set:    return await ImportSet(CurrentImport.Value.content);
                case ImportMode.Search: return await ImportCard(CurrentImport.Value.content, false,false,true);
                case ImportMode.Update: return await ImportCard(CurrentImport.Value.content, true, false, false);
                case ImportMode.List:   return await ImportList(await ParseCardList(CurrentImport.Value.content));
                default:                return new List<ICard>();
            }
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

        private async Task<List<ICard>> ImportList(List<ImportLine> deckList)
        {
            List<ICard> cards = new();
            for (int i=0;i<deckList.Count; i++)
            {
                var foundCards = await ImportCard(deckList[i].Name,true,true,false);
                cards.AddRange(foundCards);
                Worker.ReportProgress((i * 100 / deckList.Count) / 2);
            }
            return cards;
        }

        private async Task<List<ImportLine>> ParseCardList(string cardlist)
        {
            List<ImportLine> tuples = new List<ImportLine>();
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
            return tuples;
        }

        private async Task<List<ICard>> ImportCard(string cardName,bool onlyTheOne,bool skipIfExists, bool foreignIncluded)
        {
            Message = "Retrieve Card : " + cardName;
            if (skipIfExists && App.DB.cards .Where(x => x.CardId == cardName).Any()) return new List<ICard>();
            List<ICard> cards = await RequestCard(cardName,foreignIncluded);
            if (onlyTheOne) cards = await FilterExactName(cards, cardName, foreignIncluded);
            return cards;
        }

        #endregion

        #region API interaction

        private async Task<List<ICard>> RequestCard(string cardName,bool foreignIncluded)
        {
            ICardService service = MtgApi.GetCardService();
            List<ICard> cards = new();
            // Search VO
            int nbApiCallsNeedded = await GetApiCallsNeeded(service, cardName, false);
            for (int i = 1; i <= nbApiCallsNeedded; i++)
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
                nbApiCallsNeedded = await GetApiCallsNeeded(service, cardName, true);
                for (int i = 1; i <= nbApiCallsNeedded; i++)
                {
                    Thread.Sleep(200);
                    IOperationResult<List<ICard>> tmpResult = await service
                        .Where(x => x.Name, cardName)
                        .Where(x => x.Language, App.LANG.GetForeignLanguage())
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
                        .Where(x => x.Language, App.LANG.GetForeignLanguage())
                        .Where(x => x.PageSize, 1)
                        .AllAsync();
            else firstResult = await service
                        .Where(x => x.Name, cardName)
                        .Where(x => x.PageSize, 1)
                        .AllAsync();
            if (firstResult.IsSuccess)
                return ((int)firstResult.PagingInfo.TotalPages / 100) + 1;
            else
            {
                MessageBoxHelper.ShowMsg("API error : " + firstResult.Exception.Message);
                return 0;
            }
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
                    if (foreignIncluded && card.ForeignNames!=null && IsExactCardName(cardName, card.ForeignNames.Where(x=>x.Language==App.LANG.GetForeignLanguage()).FirstOrDefault().Name)) 
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
            var localCard = App.DB.cards.Where(x => x.CardId == cardData.Name).FirstOrDefault();
            if (localCard == null)
            {
                localCard = new MagicCard(cardData);
                App.DB.cards.Add(localCard);
                App.DB.SaveChanges();
            }
            // Variant
            var localVariant = localCard.Variants.Where(x => x.Id == cardData.Id).FirstOrDefault();
            if (localVariant == null)
            {
                localCard.AddVariant(cardData);
                App.DB.SaveChanges();
            }
        }

        private void MakeDeck()
        {
            var title = (CurrentImport.Value.title == null) ? DateTime.Now.ToString() : CurrentImport.Value.title;
            var deck = new MagicDeck(title);
            foreach (var cardOccurence in ParseCardList(CurrentImport.Value.content).Result)
            {
                MagicCard card = App.DB.cards.Where(x => x.CardId == cardOccurence.Name).FirstOrDefault();
                if (card != null)
                {
                    MagicCardVariant variant = card.Variants[0];
                    AddCardToDeck(variant, deck, cardOccurence.Quantity,cardOccurence.Side ? 2 : 0);
                }
            }
            deck.CardRelations[0].RelationType = 1;
            App.DB.decks.Add(deck);
            App.DB.SaveChanges();
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
            deck.CardRelations[0].RelationType = 1;
        }

        #endregion

        #region SaveState

        string StatePath { get { return App.RoamingFolder + "\\ImporterState.txt" ; } }

        public void SaveState()
        {
            string jsonString = "";
            foreach (var v in PendingImport)
            {
                jsonString += JsonSerializer.Serialize(PendingImport)+"\n";
            }
            File.WriteAllText(StatePath, jsonString);
        }

        public void LoadState()
        {
            if (!File.Exists(StatePath)) return;
            string jsonString = File.ReadAllText(StatePath);
            PendingImport = new Queue<PendingImport>();
            foreach (var v in jsonString.Split('\n'))
            {
                PendingImport.Enqueue(JsonSerializer.Deserialize<PendingImport>(jsonString)!);
            }
            File.WriteAllText(StatePath, "");
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
