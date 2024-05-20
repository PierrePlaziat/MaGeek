using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore.Data.Collection.Entities;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;
using MageekFrontWpf.MageekTools.DeckTools;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Documents;
using MageekCore.Data;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;
using MageekCore.Service;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{

    public partial class TxtInputViewModel : ObservableViewModel
    {

        IMageekService mageek;
        WindowsService win;
        DialogService dialog;

        public TxtInputViewModel(IMageekService mageek, WindowsService win, DialogService dialog)
        {
            this.mageek = mageek;
            this.win = win;
            this.dialog = dialog;
        }

        [ObservableProperty] string document = string.Empty;
        [ObservableProperty] string checkResult = string.Empty;
        [ObservableProperty] string checkDetail= string.Empty;

        [RelayCommand]
        private async Task<CardList> Check()
        {
            string cardList = FlowDocumentStringToSimpleString(Document);
            CardList result = await mageek.CardLists_Parse(Document);
            CheckResult = result.Status;
            CheckDetail = result.Detail;
            return result;
        }

        private string FlowDocumentStringToSimpleString(string document)
        {
            string s = document
                .Replace("<LineBreak />", Environment.NewLine)
                .Replace("</Paragraph>", Environment.NewLine);
            return Regex.Replace(s, "<[a-zA-Z/].*?>", String.Empty);
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
            CardList result = await Check();
            bool ok = UserComfirmation(result);
            if (ok)
            {
                DoAddToCollec(result.Cards);
            }
        }

        [RelayCommand]
        private async Task Open()
        {
            CardList result = await Check();
            bool ok = UserComfirmation(result);
            if (ok)
            {
                DoOpenTheDeck(result.Cards);
            }
        }

        [RelayCommand]
        private async Task CollectAndOpen()
        {
            CardList result = await Check();
            bool ok = UserComfirmation(result);
            if (ok)
            {
                DoAddToCollec(result.Cards);
                DoOpenTheDeck(result.Cards);
            }
        }

        private bool UserComfirmation(CardList result)
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
            foreach (var v in cards) await mageek.Collec_Move(v.CardUuid, v.Quantity);
        }

        private void DoOpenTheDeck(List<DeckCard> cards)
        {
            ManipulableDeck deck = ServiceHelper.GetService<ManipulableDeck>();
            DocumentArguments doc = new DocumentArguments(import: cards);
            win.OpenDoc(doc);
        }

    }
}
