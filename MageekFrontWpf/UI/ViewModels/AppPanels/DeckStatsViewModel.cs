using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekService.Data.Collection.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MageekFrontWpf.UI.ViewModels.AppPanels
{
    public partial class DeckStatsViewModel : BaseViewModel,
        IRecipient<DeckSelectMessage>,
        IRecipient<UpdateDeckMessage>
    {
        private MageekService.MageekService mageek;
        private DialogService dialog;

        public DeckStatsViewModel(DialogService dialog, MageekService.MageekService mageek)
        {
            this.mageek = mageek;
            this.dialog = dialog;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        [ObservableProperty] private Deck currentDeck;
        [ObservableProperty] int creatureCount;
        [ObservableProperty] int instantCount;
        [ObservableProperty] int sorceryCount;
        [ObservableProperty] int enchantmentCount;
        [ObservableProperty] int artifactCount;
        [ObservableProperty] int basicLandCount;
        [ObservableProperty] int specialLandCount;
        [ObservableProperty] int otherCount;
        [ObservableProperty] int devotionB;
        [ObservableProperty] int devotionW;
        [ObservableProperty] int devotionU;
        [ObservableProperty] int devotionG;
        [ObservableProperty] int devotionR;
        [ObservableProperty] string standardOk;
        [ObservableProperty] string commanderOk;
        [ObservableProperty] int ownedRatio;
        [ObservableProperty] bool isActive;
        [ObservableProperty] bool isLoading = false;
        [ObservableProperty] PointCollection curvePoints;
        [ObservableProperty] Point curveStart;
        [ObservableProperty] List<string> hand;
        List<int> alreadyDrawed;
        readonly Random random = new();

        public void Receive(UpdateDeckMessage message)
        {
            AsyncReload();
        }

        public void Receive(DeckSelectMessage message)
        {
            CurrentDeck = mageek.GetDeck(message.Value).Result;
        }

        #region Async Reload

        private void AsyncReload()
        {
            DoAsyncReload().ConfigureAwait(false);
        }

        private async Task DoAsyncReload()
        {
            if (CurrentDeck == null) return;
            IsLoading = true;
            CreatureCount = await mageek.Count_Typed(CurrentDeck.DeckId, "Creature");
            InstantCount = await mageek.Count_Typed(CurrentDeck.DeckId, "Instant");
            SorceryCount = await mageek.Count_Typed(CurrentDeck.DeckId, "Sorcery");
            EnchantmentCount = await mageek.Count_Typed(CurrentDeck.DeckId, "Enchantment");
            ArtifactCount = await mageek.Count_Typed(CurrentDeck.DeckId, "Artifact");
            BasicLandCount = await mageek.Count_Typed(CurrentDeck.DeckId, "Planeswalker");
            BasicLandCount = await mageek.Count_Typed(CurrentDeck.DeckId, "Land");
            DevotionB = await mageek.DeckDevotion(CurrentDeck.DeckId, 'B');
            DevotionW = await mageek.DeckDevotion(CurrentDeck.DeckId, 'W');
            DevotionU = await mageek.DeckDevotion(CurrentDeck.DeckId, 'U');
            DevotionG = await mageek.DeckDevotion(CurrentDeck.DeckId, 'G');
            DevotionR = await mageek.DeckDevotion(CurrentDeck.DeckId, 'R');
            StandardOk = await mageek.DeckValidity(CurrentDeck, "Standard");
            CommanderOk = await mageek.DeckValidity(CurrentDeck, "Commander");
            OwnedRatio = await mageek.OwnedRatio(CurrentDeck.DeckId);
            OnPropertyChanged(nameof(CreatureCount));
            OnPropertyChanged(nameof(InstantCount));
            OnPropertyChanged(nameof(SorceryCount));
            OnPropertyChanged(nameof(EnchantmentCount));
            OnPropertyChanged(nameof(ArtifactCount));
            OnPropertyChanged(nameof(BasicLandCount));
            OnPropertyChanged(nameof(SpecialLandCount));
            OnPropertyChanged(nameof(OtherCount));
            OnPropertyChanged(nameof(StandardOk));
            OnPropertyChanged(nameof(CommanderOk));
            OnPropertyChanged(nameof(DevotionB));
            OnPropertyChanged(nameof(DevotionW));
            OnPropertyChanged(nameof(DevotionU));
            OnPropertyChanged(nameof(DevotionG));
            OnPropertyChanged(nameof(DevotionR));
            OnPropertyChanged(nameof(OwnedRatio));
            int[] manacurve = await mageek.GetManaCurve(CurrentDeck.DeckId);
            DrawNewHand();
            DrawManacurve(manacurve);
            IsLoading = false;
        }

        #endregion

        #region Methods

        [RelayCommand]
        private async Task ListMissing()
        {
            string missList = await mageek
                .ListMissingCards(CurrentDeck.DeckId);
            if (!string.IsNullOrEmpty(missList))
            {
                Clipboard.SetText(missList);
                dialog.Notif("Missing:","Copied to clipboard.");
            }
        }

        private void DrawManacurve(int[] manaCurve)
        {
            CurveStart = new Point(0, 0);
            CurvePoints = null;
            PointCollection Points = new();
            if (CurrentDeck != null)
            {
                int manaMax = manaCurve.ToList().Max();
                float factor;
                if (manaMax == 0) factor = 0;
                else factor = 100 / manaMax;
                CurveStart = new Point(0, 100 - factor * manaCurve[0]);
                Points.Add(CurveStart);
                for (int i = 1; i < manaCurve.Length; i++) Points.Add(new Point(50 * i, 100 - factor * manaCurve[i]));

            }
            CurvePoints = Points;
        }

        #region ManaCurve

        #endregion

        #region Hand

        private void DrawNewHand()
        {
            Hand = new List<string>();
            alreadyDrawed = new List<int>();
            for (int i = 0; i < 7; i++) DrawNewCard();
        }

        private async void DrawNewCard()
        {
            string newCard = await DoDraw();
            if (newCard != null)
            {
                Hand.Add(newCard);
            }
        }

        private async Task<string> DoDraw()
        {
            if (CurrentDeck == null) return null;
            if (CurrentDeck.CardCount <= alreadyDrawed.Count) return null;
            int rgn;
            do { rgn = random.Next(CurrentDeck.CardCount); }
            while (alreadyDrawed.Contains(rgn));
            //TODO
            await Task.Delay(0);
            return "";
            //return await Mageek.GetDeckContent(currentDeck.DeckId)[rgn].;
        }

        #region UI Link

        private void DrawNewHandButtonClick(object sender, RoutedEventArgs e)
        {
            DrawNewHand();
        }

        private void DrawCardButtonClick(object sender, RoutedEventArgs e)
        {
            DrawNewCard();
        }

        #endregion

        #endregion

        #endregion

    }
}
