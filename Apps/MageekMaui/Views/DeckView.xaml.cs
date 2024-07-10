using MageekMobile.ViewModels;

namespace MageekMobile.Views;

public partial class DeckView : ContentPage
{
	public DeckView(DeckViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;
    }
}