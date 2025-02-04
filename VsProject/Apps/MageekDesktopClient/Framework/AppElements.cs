﻿using System.Collections.Generic;
using PlaziatWpf.Services;
using MageekDesktopClient.UI.ViewModels.AppPanels;
using MageekDesktopClient.UI.ViewModels.AppWindows;
using MageekDesktopClient.UI.Views.AppWindows;
using MageekDesktopClient.UI.Views.AppPanels;
using MageekDesktopClient.UI.ViewModels.Windows;
using PlaziatWpf.Mvvm;

namespace MageekDesktopClient.Framework
{

    public enum AppWindowEnum
    {
        Welcome,
        Main,
        Print,
    }

    public enum AppToolsEnum
    {
        CardInspector,
        CardSearcher,
        SetList,
        PrecoList,
        DeckList,
        CollecEstimation,
        TxtInput,
        Detector,
    }

    public static class AppElements
    {

        internal static List<AppWindow> LoadWindows()
        {
            var wins = new List<AppWindow>()
            {
                new AppWindow(AppWindowEnum.Main.ToString(), ServiceHelper.GetService<MainWindowViewModel>(), ServiceHelper.GetService<MainWindow>()),
                new AppWindow(AppWindowEnum.Welcome.ToString(), ServiceHelper.GetService<WelcomeWindowViewModel>(), ServiceHelper.GetService<WelcomeWindow>()),
                new AppWindow(AppWindowEnum.Print.ToString(), ServiceHelper.GetService<PrintWindowViewModel>(), ServiceHelper.GetService<PrintWindow>()),
            };
            return wins;
        }

        internal static List<AppPanels> LoadTools()
        {
            var panels = new List<AppPanels>()
            {
                new AppPanels(AppToolsEnum.CardSearcher.ToString(), ServiceHelper.GetService<CardSearcherViewModel>(), ServiceHelper.GetService<CardSearcher>()),
                new AppPanels(AppToolsEnum.CardInspector.ToString(), ServiceHelper.GetService<CardInspectorViewModel>(), ServiceHelper.GetService<CardInspector>()),
                new AppPanels(AppToolsEnum.SetList.ToString(), ServiceHelper.GetService<SetListViewModel>(), ServiceHelper.GetService<SetList>()),
                new AppPanels(AppToolsEnum.PrecoList.ToString(), ServiceHelper.GetService<PrecoListViewModel>(), ServiceHelper.GetService<PrecoList>()),
                new AppPanels(AppToolsEnum.DeckList.ToString(), ServiceHelper.GetService<DeckListViewModel>(), ServiceHelper.GetService<DeckList>()),
                new AppPanels(AppToolsEnum.CollecEstimation.ToString(), ServiceHelper.GetService<CollecEstimationViewModel>(), ServiceHelper.GetService<CollecEstimation>()),
                new AppPanels(AppToolsEnum.TxtInput.ToString(), ServiceHelper.GetService<TxtInputViewModel>(), ServiceHelper.GetService<TxtInput>()),
                new AppPanels(AppToolsEnum.Detector.ToString(), ServiceHelper.GetService<DetectorViewModel>(), ServiceHelper.GetService<Detector>()),
            };
            return panels;
        }

    }

}
