using MageekMobile.ViewModels;

namespace MageekMobile.Views;

public partial class CollecView : ContentPage
{
	public CollecView(CollecViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}