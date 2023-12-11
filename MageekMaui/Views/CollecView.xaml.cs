using MageekMaui.ViewModels;

namespace MageekMaui.Views;

public partial class CollecView : ContentPage
{
	public CollecView(CollecViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}