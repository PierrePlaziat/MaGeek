using System.Collections.Generic;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekFrontWpf.UI.Views.AppPanels;
using System.Diagnostics;
using MageekCore.Tools;

namespace MageekFrontWpf.AppValues
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

    public static class WindowsAndTools
    {

        internal static List<AppWindow> GetWindows()
        {
            Logger.Log("");
            return new List<AppWindow>()
            {
                new AppWindow() { id = AppWindowEnum.Main, vm = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() },
                new AppWindow() { id = AppWindowEnum.Welcome, vm = ServiceHelper.GetService<WelcomeWindowViewModel>(), window = ServiceHelper.GetService<WelcomeWindow>() },
                new AppWindow() { id = AppWindowEnum.Print, vm = ServiceHelper.GetService<PrintViewModel>(), window = ServiceHelper.GetService<PrintWindow>() },
            };
        }

        internal static List<AppTool> GetTools()
        {
            Logger.Log("");
            return new List<AppTool>()
            {
                new AppTool() { id = AppToolsEnum.CardSearcher, vm = ServiceHelper.GetService<CardSearcherViewModel>(), tool = ServiceHelper.GetService<CardSearcher>() },
                new AppTool() { id = AppToolsEnum.CardInspector, vm = ServiceHelper.GetService<CardInspectorViewModel>(), tool = ServiceHelper.GetService<CardInspector>() },
                new AppTool() { id = AppToolsEnum.SetList, vm = ServiceHelper.GetService<SetListViewModel>(), tool = ServiceHelper.GetService<SetList>() },
                new AppTool() { id = AppToolsEnum.PrecoList, vm = ServiceHelper.GetService<PrecoListViewModel>(), tool = ServiceHelper.GetService<PrecoList>() },
                new AppTool() { id = AppToolsEnum.DeckList, vm = ServiceHelper.GetService<DeckListViewModel>(), tool = ServiceHelper.GetService<DeckList>() },
                new AppTool() { id = AppToolsEnum.CollecEstimation, vm = ServiceHelper.GetService<CollecEstimationViewModel>(), tool = ServiceHelper.GetService<CollecEstimation>() },
                new AppTool() { id = AppToolsEnum.TxtInput, vm = ServiceHelper.GetService<TxtInputViewModel>(), tool = ServiceHelper.GetService<TxtInput>() },
            };
        }

    }

}
