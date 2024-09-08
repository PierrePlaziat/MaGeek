using CommunityToolkit.Mvvm.ComponentModel;

namespace MageekMobile.ViewModels
{
    public partial class ViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isBusy;
    }
}