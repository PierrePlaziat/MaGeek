﻿using MageekFrontWpf.Framework.BaseMvvm;
using System.Collections.Generic;
using static MageekFrontWpf.Framework.Services.WindowsService;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;

namespace MageekFrontWpf.AppValues
{

    public static class WindowsAndPanels
    {
        internal static List<AppPanel> GetPanels()
        {
            List<AppPanel> appPanels = new()
            {
                new AppPanel() { id = AppPanelEnum.DeckStats, viewModel = ServiceHelper.GetService<DeckStatsViewModel>(), window = ServiceHelper.GetService<DeckStats>() },
                new AppPanel() { id = AppPanelEnum.CardInspector, viewModel = ServiceHelper.GetService<CardInspectorViewModel>(), window = ServiceHelper.GetService<CardInspector>() },
                new AppPanel() { id = AppPanelEnum.CardSearcher, viewModel = ServiceHelper.GetService<CardSearcherViewModel>(), window = ServiceHelper.GetService<CardSearcher>() },
                new AppPanel() { id = AppPanelEnum.DeckContent, viewModel = ServiceHelper.GetService<DeckContentViewModel>(), window = ServiceHelper.GetService<DeckContent>() },
                new AppPanel() { id = AppPanelEnum.DeckList, viewModel = ServiceHelper.GetService<DeckListViewModel>(), window = ServiceHelper.GetService<DeckList>() },
                new AppPanel() { id = AppPanelEnum.DeckTable, viewModel = ServiceHelper.GetService<DeckTableViewModel>(), window = ServiceHelper.GetService<DeckTable>() },
                new AppPanel() { id = AppPanelEnum.Sets, viewModel = ServiceHelper.GetService<SetExplorerViewModel>(), window = ServiceHelper.GetService<SetExplorer>() },
                new AppPanel() { id = AppPanelEnum.ImporterUi, viewModel = ServiceHelper.GetService<ImporterUiViewModel>(), window = ServiceHelper.GetService<ImporterUi>() }
            };
            return appPanels;
        }

        internal static List<AppWindow> GetWindows()
        {
            List<AppWindow> appWindows = new()
            {
                new AppWindow() { id = AppWindowEnum.Main, viewModel = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() },
                new AppWindow() { id = AppWindowEnum.Welcome, viewModel = ServiceHelper.GetService<WelcomeWindowViewModel>(), window = ServiceHelper.GetService<WelcomeWindow>() },
                new AppWindow() { id = AppWindowEnum.Precos, viewModel = ServiceHelper.GetService<PrecosViewModel>(), window = ServiceHelper.GetService<PrecosWindow>() },
                new AppWindow() { id = AppWindowEnum.Import, viewModel = ServiceHelper.GetService<ImportViewModel>(), window = ServiceHelper.GetService<ImportWindow>() },
                new AppWindow() { id = AppWindowEnum.Print, viewModel = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() },
                new AppWindow() { id = AppWindowEnum.Estimation, viewModel = ServiceHelper.GetService<CollectionEstimationViewModel>(), window = ServiceHelper.GetService<CollectionEstimation>() }
            };
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