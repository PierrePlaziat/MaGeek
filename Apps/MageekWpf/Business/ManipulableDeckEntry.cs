using CommunityToolkit.Mvvm.ComponentModel;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;

namespace MageekDesktop.MageekTools.DeckTools
{

    public partial class ManipulableDeckEntry : ObservableObject
    {

        [ObservableProperty] DeckCard line;
        [ObservableProperty] Cards card;

    }

}
