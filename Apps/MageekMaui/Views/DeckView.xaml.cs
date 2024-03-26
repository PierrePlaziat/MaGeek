using MageekMaui.ViewModels;

namespace MageekMaui.Views;

public partial class DeckView : ContentPage
{
	public DeckView(DeckViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;
    }
}