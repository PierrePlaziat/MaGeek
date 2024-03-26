using MageekMaui.Views;

namespace MageekMaui
{

    public partial class AppShell : Shell
    {

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(WelcomeView),typeof(WelcomeView));
            Routing.RegisterRoute(nameof(CollecView), typeof(CollecView));
            Routing.RegisterRoute(nameof(DeckView), typeof(DeckView));
            Routing.RegisterRoute(nameof(GameView), typeof(GameView));
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
        }

    }

}
