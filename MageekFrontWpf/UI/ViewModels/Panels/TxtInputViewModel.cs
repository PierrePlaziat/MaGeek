using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using MageekCore.Data.Collection.Entities;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{
    public partial class TxtInputViewModel : BaseViewModel
    {

        MageekService mageek;
        WindowsService win;

        public TxtInputViewModel(MageekService mageek, WindowsService win)
        {
            this.mageek = mageek;
            this.win = win;
        }

        [ObservableProperty] private string title;
        [ObservableProperty] private string description;
        [ObservableProperty] private bool asOwned;

        [RelayCommand]
        private async Task LaunchImportation(string content)
        {
            List<DeckCard> importLines = await mageek.ParseCardList(content);
            OpenedDeck deck = new OpenedDeck(mageek);
            await deck.Initialize(importLines);
            AppDocumentInitArgs doc = new AppDocumentInitArgs(import: importLines);
            win.OpenDoc(doc);
        }

    }
}
