using MageekMobile.ViewModels;

namespace MageekMobile.Views;

public partial class WelcomeView : ContentPage
{

    public WelcomeView(WelcomeViewModel model)
    {
        InitializeComponent();
        this.BindingContext = model;
	}

}