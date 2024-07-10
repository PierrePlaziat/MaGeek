using MageekMobile.ViewModels;

namespace MageekMobile.Views;

public partial class GameView : ContentPage
{
	public GameView(GameViewModel viewModel)
	{
		InitializeComponent();
        this.BindingContext = viewModel;
    }
}