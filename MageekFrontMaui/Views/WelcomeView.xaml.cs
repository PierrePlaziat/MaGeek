using MageekMaui.ViewModels;

namespace MageekMaui.Views;

public partial class WelcomeView : ContentPage
{

    public WelcomeView(WelcomeViewModel model)
    {
        InitializeComponent();
        this.BindingContext = model;
	}

}