using System.Collections.Generic;
using PlaziatWpf.Services;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using PlaziatCore;
using MageekFrontWpf.UI.ViewModels.Windows;
using PlaziatWpf.Mvvm;

namespace MageekFrontWpf.Framework
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
    }

    public static class AppElements
    {

        internal static List<AppWindow> LoadWindows()
        {
            var wins = new List<AppWindow>()
            {
                new AppWindow() { id = AppWindowEnum.Main.ToString(), vm = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() },
                new AppWindow() { id = AppWindowEnum.Welcome.ToString(), vm = ServiceHelper.GetService<WelcomeWindowViewModel>(), window = ServiceHelper.GetService<WelcomeWindow>() },
                new AppWindow() { id = AppWindowEnum.Print.ToString(), vm = ServiceHelper.GetService<PrintWindowViewModel>(), window = ServiceHelper.GetService<PrintWindow>() },
            };
            Logger.Log("Done");
            return wins;
        }

        internal static List<AppTool> LoadTools()
        {
            var panels = new List<AppTool>()
            {
                new AppTool() { id = AppToolsEnum.CardSearcher.ToString(), vm = ServiceHelper.GetService<CardSearcherViewModel>(), tool = ServiceHelper.GetService<CardSearcher>() },
                new AppTool() { id = AppToolsEnum.CardInspector.ToString(), vm = ServiceHelper.GetService<CardInspectorViewModel>(), tool = ServiceHelper.GetService<CardInspector>() },
                new AppTool() { id = AppToolsEnum.SetList.ToString(), vm = ServiceHelper.GetService<SetListViewModel>(), tool = ServiceHelper.GetService<SetList>() },
                new AppTool() { id = AppToolsEnum.PrecoList.ToString(), vm = ServiceHelper.GetService<PrecoListViewModel>(), tool = ServiceHelper.GetService<PrecoList>() },
                new AppTool() { id = AppToolsEnum.DeckList.ToString(), vm = ServiceHelper.GetService<DeckListViewModel>(), tool = ServiceHelper.GetService<DeckList>() },
                new AppTool() { id = AppToolsEnum.CollecEstimation.ToString(), vm = ServiceHelper.GetService<CollecEstimationViewModel>(), tool = ServiceHelper.GetService<CollecEstimation>() },
                new AppTool() { id = AppToolsEnum.TxtInput.ToString(), vm = ServiceHelper.GetService<TxtInputViewModel>(), tool = ServiceHelper.GetService<TxtInput>() },
            };
            Logger.Log("Done");
            return panels;
        }

    }

}
