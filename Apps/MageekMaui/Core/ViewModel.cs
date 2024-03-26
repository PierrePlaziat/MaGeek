using CommunityToolkit.Mvvm.ComponentModel;

namespace MageekMaui.ViewModels
{
    public partial class ViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;
    }
}