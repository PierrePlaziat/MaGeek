using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using MageekCore.Data.Collection.Entities;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;
using MageekFrontWpf.MageekTools.DeckTools;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Documents;
using MageekCore.Data;
using System;
using System.Diagnostics.Eventing.Reader;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class TxtInputViewModel : ObservableViewModel
    {

        MageekService mageek;
        WindowsService win;
        DialogService dialog;

        public TxtInputViewModel(MageekService mageek, WindowsService win, DialogService dialog)
        {
            this.mageek = mageek;
            this.win = win;
            this.dialog = dialog;
        }

        [ObservableProperty] string document;
        [ObservableProperty] string checkResult = string.Empty;
        [ObservableProperty] string checkDetail= string.Empty;

        [RelayCommand]
        private async Task<TxtImportResult> Check()
        {
            TxtImportResult result = await mageek.ParseCardList(Document);
            CheckResult = result.Status;
            CheckDetail= result.Detail;
            return result;
        }
        
        [RelayCommand]
        private async Task Clear()
        {
            Document = string.Empty;
            CheckResult = "Cleared";
        }
        
        [RelayCommand]
        private async Task Collect()
        {
            TxtImportResult result = await Check();
            bool ok = AskUser(result);
            if (ok)
            {
                DoAddToCollec(result.Cards);
            }
        }

        [RelayCommand]
        private async Task Open()
        {
            TxtImportResult result = await Check();
            bool ok = AskUser(result);
            if (ok)
            {
                DoOpenTheDeck(result.Cards);
            }
        }

        [RelayCommand]
        private async Task CollectAndOpen()
        {
            TxtImportResult result = await Check();
            bool ok = AskUser(result);
            if (ok)
            {
                DoAddToCollec(result.Cards);
                DoOpenTheDeck(result.Cards);
            }
        }

        private bool AskUser(TxtImportResult result)
        {
            if (result.Status == "KO")
            {
                dialog.InformUser("Fatal error : " + result.Detail);
                return false;
            }
            else if (result.Status == "errors") return dialog.AskUser("Some errors, import anyway?\n" + result.Detail);
            else if (result.Status == "OK") return dialog.AskUser("Check OK, perform operation?" + result.Detail);
            else return dialog.AskUser("??? import anyway ???\n" + result.Status+":\n"+ result.Detail);
        }

        private async void DoAddToCollec(List<DeckCard> cards)
        {
            foreach (var v in cards) await mageek.CollecMove(v.CardUuid, v.Quantity);
        }

        private void DoOpenTheDeck(List<DeckCard> cards)
        {
            ManipulableDeck deck = ServiceHelper.GetService<ManipulableDeck>();
            DocumentArguments doc = new DocumentArguments(import: cards);
            win.OpenDoc(doc);
        }

    }
}
