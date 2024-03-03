using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using MageekCore.Data.Collection.Entities;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.Framework.AppValues;
using MageekFrontWpf.MageekTools.DeckTools;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{
    public partial class TxtInputViewModel : ObservableViewModel
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
            ManipulableDeck deck = ServiceHelper.GetService<ManipulableDeck>();
            await deck.OpenDeck(importLines);
            DocumentArguments doc = new DocumentArguments(import: importLines);
            win.OpenDoc(doc);
        }

    }
}
