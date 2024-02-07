using CommunityToolkit.Mvvm.ComponentModel;
using MageekFrontWpf.Framework.BaseMvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageekFrontWpf.UI.ViewModels
{

    internal partial class DeckDocumentViewModel : BaseViewModel
    {

        public DeckDocumentViewModel()
        {
                
        }

        [ObservableProperty] private string deckUuid;

    }

}
