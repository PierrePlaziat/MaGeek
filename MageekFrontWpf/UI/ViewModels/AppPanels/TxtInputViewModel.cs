using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekCore;
using MageekCore.Data.Collection.Entities;
using MageekCore.Tools;

namespace MageekFrontWpf.UI.ViewModels.AppWindows
{
    public partial class TxtInputViewModel : BaseViewModel
    {

        private MageekService mageek;

        public TxtInputViewModel
        (
            MageekService mageek
        )
        {
            this.mageek = mageek;
        }

        [ObservableProperty] private string title;
        [ObservableProperty] private string description;
        [ObservableProperty] private bool asOwned;

        [RelayCommand]
        private async Task LaunchImportation(string content)
        {
            List<DeckCard> importLines = await mageek.ParseCardList(content);
            await mageek.CreateDeck_Contructed(
                Title,
                Description,
                importLines
            );
            //TODO asOwned
        }

    }
}
