using CommunityToolkit.Mvvm.ComponentModel;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;

namespace MageekFrontWpf.MageekTools.DeckTools
{

    public partial class ManipulableDeckEntry : ObservableObject
    {

        [ObservableProperty] DeckCard line;
        [ObservableProperty] Cards card;

    }

}
