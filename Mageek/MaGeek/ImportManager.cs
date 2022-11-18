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
        BackgroundWorker Worker = new BackgroundWorker();

        #region State

        Queue<PendingImport> PendingImport = new Queue<PendingImport>();
        PendingImport? CurrentImport = null;

        enum ImporterState { Init, Play, Pause, Canceling }
        ImporterState state = ImporterState.Init;

        #endregion

        #region UI

        public string Message { get; set; } = "Init";
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

        #endregion

        #region Timer

        Timer timer;

        private void ConfigureTimer()
        {
            timer = new Timer(1000);
            timer.AutoReset=true;
            timer.Elapsed += MainLoop;
            timer.Start();
        }

        #endregion

        #region CTOR

        public ImportManager()
        {
            LoadState();
            ConfigureWorker();
            ConfigureTimer();
            state = ImporterState.Pause;
            Message = "Init done"; 
        }

        private void ConfigureWorker()
        {
            Worker.DoWork += Work_DoNextImport;
            Worker.ProgressChanged += ReportImportProgress;
            Worker.WorkerReportsProgress = true;
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

        #region State

        private void MainLoop(object sender, ElapsedEventArgs e)
        {
            if (state == ImporterState.Init) return;
            if (!Worker.IsBusy && state == ImporterState.Play) CheckNextImport(); 

            //TODO : Move those so they are recalculated only when needed
            SaveState();
            UpdateInfoText();

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

        private void UpdateInfoText()
        {
            int i = 0;
            string s = "";
            if (CurrentImport.HasValue) s += i++ + " : " + CurrentImport.Value.mode + " - " + CurrentImport.Value.title;
            foreach (var v in PendingImport) s += "\n" + i++ + " : " + v.mode + " - " + v.title;
            InfoText = s;
        }

        #endregion

        #region Worker

        private void CheckNextImport()
        {
            if (state != ImporterState.Play) return;
            if (PendingImport.Count>0) CurrentImport = PendingImport.Dequeue();
            if (CurrentImport != null) Worker.RunWorkerAsync();
        }

        private void Work_DoNextImport(object sender, DoWorkEventArgs e)
        {
            if (state == ImporterState.Init) return;
            List<ICard> importResult = new List<ICard>();
            if   (state != ImporterState.Canceling)  importResult = Work_CallApi();
            while(state != ImporterState.Canceling && state == ImporterState.Pause) { };
            App.STATE.RaisePreventUIAction(true);
            if   (state != ImporterState.Canceling)  Work_RecordLocal(importResult);
            App.STATE.RaisePreventUIAction(false);
            while (state != ImporterState.Canceling && state == ImporterState.Pause) { };
            if   (state != ImporterState.Canceling) Work_MakeADeck(importResult);
            Work_Finalize();
        }

        private List<ICard> Work_CallApi()
        {
            Message = "Calling MtgApi";
            Worker.ReportProgress(0);
            return Import(CurrentImport).Result; 
        }

        private void Work_RecordLocal(List<ICard> importResult)
        {
            Message = "Recording cards localy";
            Worker.ReportProgress(50);
            RecordCards(importResult);
        }

        private void Work_MakeADeck(List<ICard> importResult)
        {
            Message = "Making a Deck";
            Worker.ReportProgress(75);
            if (CurrentImport.Value.mode == ImportMode.List) MakeDeck(ParseCardList(CurrentImport.Value.content).Result);
            if (CurrentImport.Value.mode == ImportMode.Set)
            {
                List<ImportLine> importLines = new();
                foreach(var v in importResult) importLines.Add(new ImportLine() { Name= v.Name, Quantity=1 });
                MakeDeck(importLines);
            }
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
            if (state == ImporterState.Canceling) state = ImporterState.Play;
            Message = "Done";
        }

        private void ReportImportProgress(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress = e.ProgressPercentage;
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
                App.DB.SafeSaveChanges();
            }
            // Variant
            var localVariant = localCard.Variants.Where(x => x.Id == cardData.Id).FirstOrDefault();
            if (localVariant == null)
            {
                localCard.AddVariant(cardData);
                App.DB.SafeSaveChanges();
            }
        }

        private void MakeDeck(List<ImportLine> importLines)
        {
            var title = (CurrentImport.Value.title == null) ? DateTime.Now.ToString() : CurrentImport.Value.title;
            var deck = new MagicDeck(title);
            foreach (var cardOccurence in importLines)
            {
                MagicCard card = App.DB.cards.Where(x => x.CardId == cardOccurence.Name).FirstOrDefault();
                if (card != null)
                {
                    MagicCardVariant variant = card.Variants[0];
                    AddCardToDeck(variant, deck, cardOccurence.Quantity,cardOccurence.Side ? 2 : 0);
                }
            }
            if (deck.CardRelations.Count>0)
            {
                deck.CardRelations[0].RelationType = 1;
                App.DB.decks.Add(deck);
                App.DB.SafeSaveChanges();
            }
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
            var x = PendingImport.ToList();
            if (CurrentImport!=null) x.Add(CurrentImport.Value);
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
