using MageekMaui.ViewModels;

namespace MageekMaui.Views;

public partial class GameView : ContentPage
{
	public GameView(GameViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;
    }
}