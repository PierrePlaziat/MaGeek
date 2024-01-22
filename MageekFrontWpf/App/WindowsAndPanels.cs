using MaGeek.UI.Menus;
using MaGeek.UI;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.ViewModels;
using MageekFrontWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MageekFrontWpf.App.WindowsManager;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MaGeek;
using MaGeek.UI.Windows.Importers;

namespace MageekFrontWpf.App
{

    public static class WindowsAndPanels
    {
        internal static List<AppPanel> GetPanels()
        {
            List<AppPanel> appPanels = new();
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckStats, viewModel = ServiceHelper.GetService<DeckStatsViewModel>(), window = ServiceHelper.GetService<DeckStats>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.CardInspector, viewModel = ServiceHelper.GetService<CardInspectorViewModel>(), window = ServiceHelper.GetService<CardInspector>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.CardSearcher, viewModel = ServiceHelper.GetService<CardSearcherViewModel>(), window = ServiceHelper.GetService<CardSearcher>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckContent, viewModel = ServiceHelper.GetService<DeckContentViewModel>(), window = ServiceHelper.GetService<DeckContent>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckList, viewModel = ServiceHelper.GetService<DeckListViewModel>(), window = ServiceHelper.GetService<DeckList>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckTable, viewModel = ServiceHelper.GetService<DeckTableViewModel>(), window = ServiceHelper.GetService<DeckTable>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.Sets, viewModel = ServiceHelper.GetService<SetExplorerViewModel>(), window = ServiceHelper.GetService<SetExplorer>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.ImporterUi, viewModel = ServiceHelper.GetService<ImporterUiViewModel>(), window = ServiceHelper.GetService<ImporterUi>() });
            return appPanels;
        }

        internal static List<AppWindow> GetWindows()
        {
            List<AppWindow> appWindows = new();
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Main, viewModel = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Welcome, viewModel = ServiceHelper.GetService<WelcomeWindowViewModel>(), window = ServiceHelper.GetService<WelcomeWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Precos, viewModel = ServiceHelper.GetService<PrecosViewModel>(), window = ServiceHelper.GetService<PrecosWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Import, viewModel = ServiceHelper.GetService<ImportViewModel>(), window = ServiceHelper.GetService<ImportWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Print, viewModel = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Estimation, viewModel = ServiceHelper.GetService<CollectionEstimationViewModel>(), window = ServiceHelper.GetService<CollectionEstimation>() });
            return appWindows;
        }
    }

    public enum AppPanelEnum
    {
        DeckStats,
        CardInspector,
        CardSearcher,
        DeckContent,
        DeckList,
        DeckTable,
        ImporterUi,
        Sets
    }

    public enum AppWindowEnum
    {
        Main,
        Welcome,
        Precos,
        Import,
        Print,
        Estimation
    }


}
