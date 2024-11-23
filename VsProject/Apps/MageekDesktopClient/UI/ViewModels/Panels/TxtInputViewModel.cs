using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using PlaziatWpf.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using MageekCore.Data;
using System;
using System.Text.RegularExpressions;
using MageekCore.Services;
using PlaziatWpf.Mvvm;
using MageekDesktopClient.Framework;

namespace MageekDesktopClient.UI.ViewModels.AppWindows
{

    public partial class TxtInputViewModel : ObservableViewModel
    {

        IMageekService mageek;
        WindowsService win;
        DialogService dialog;
        SessionBag session;

        public TxtInputViewModel(IMageekService mageek, WindowsService win, DialogService dialog, SessionBag session)
        {
            this.mageek = mageek;
            this.win = win;
            this.dialog = dialog;
            this.session = session;
        }

        [ObservableProperty] string document;
        [ObservableProperty] string checkResult = string.Empty;

        [RelayCommand]
        private async Task<CardList> Check()
        {
            string cardList = FlowDocumentStringToSimpleString(Document);
            CardList result = await mageek.CardLists_Parse(cardList);
            if (!string.IsNullOrEmpty(result.Detail))
            {
                dialog.InformUser(result.Detail);
                CheckResult = "errors";
            }
            else
                CheckResult = "OK";

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
        private async Task Open()
        {
            CardList result = await Check();
            if (CheckResult == "OK")
            {
                DocumentArguments doc = new DocumentArguments(import: result.Cards);
                win.OpenDocument(doc);
            }
        }

    }
}
