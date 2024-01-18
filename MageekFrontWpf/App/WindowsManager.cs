using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using MaGeek.UI;
using static MageekFrontWpf.Framework.Services.AppEvents;
using System.IO;
using System;
using System.Collections.Generic;
using MaGeek;
using MageekService.Tools;
using System.Linq;
using Microsoft.Extensions.Logging;
using MageekFrontWpf.Framework.Services;
using MageekFrontWpf.UI.ViewModels;
using MaGeek.UI.Windows.Importers;
using MageekFrontWpf.ViewModels;
using MageekFrontWpf.UI.ViewModels.AppPanels;
using MageekFrontWpf.UI.ViewModels.AppWindows;
using AvalonDock;
using MageekFrontWpf.Framework.BaseMvvm;
using MaGeek.UI.Menus;

namespace MageekFrontWpf.App
{

    public class WindowsManager
    {

        private readonly string Path_LayoutFolder = Path.Combine(MageekService.Folders.Roaming, "Layout");
        private readonly ILogger<WindowsManager> logger;
        private readonly AppEvents events;
        private List<AppWindow> appWindows = new();
        private List<AppPanel> appPanels = new();
        LayoutRoot rootLayout;
        DockingManager dockingManager;

        public WindowsManager(
            ILogger<WindowsManager> logger,
            AppEvents events
        )
        {
            this.logger = logger;
            this.events = events;
        }

        public void Init()
        {
            events.LayoutActionEvent += HandleLayoutActionEvent;
            rootLayout = ServiceHelper.GetService<MainWindow>().RootLayout;
            dockingManager = ServiceHelper.GetService<MainWindow>().DockingManager;
            AddWindows();
            AddPanels();
        }

        private void AddWindows()
        {
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Main, viewModel = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Welcome, viewModel = ServiceHelper.GetService<WelcomeWindowViewModel>(), window = ServiceHelper.GetService<WelcomeWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Precos, viewModel = ServiceHelper.GetService<PrecosViewModel>(), window = ServiceHelper.GetService<PrecosWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Import, viewModel = ServiceHelper.GetService<ImportViewModel>(), window = ServiceHelper.GetService<ImportWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Print, viewModel = ServiceHelper.GetService<MainWindowViewModel>(), window = ServiceHelper.GetService<MainWindow>() });
            appWindows.Add(new AppWindow() { id = AppWindowEnum.Estimation, viewModel = ServiceHelper.GetService<CollectionEstimationViewModel>(), window = ServiceHelper.GetService<CollectionEstimation>() });
        }

        private void AddPanels()
        {
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckStats, viewModel = ServiceHelper.GetService<DeckStatsViewModel>(), window = ServiceHelper.GetService<DeckStats>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.CardInspector, viewModel = ServiceHelper.GetService<CardInspectorViewModel>(), window = ServiceHelper.GetService<CardInspector>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.CardSearcher, viewModel = ServiceHelper.GetService<CardSearcherViewModel>(), window = ServiceHelper.GetService<CardSearcher>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckContent, viewModel = ServiceHelper.GetService<DeckContentViewModel>(), window = ServiceHelper.GetService<DeckContent>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckList, viewModel = ServiceHelper.GetService<DeckListViewModel>(), window = ServiceHelper.GetService<DeckList>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.DeckTable, viewModel = ServiceHelper.GetService<DeckTableViewModel>(), window = ServiceHelper.GetService<DeckTable>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.Sets, viewModel = ServiceHelper.GetService<SetExplorerViewModel>(), window = ServiceHelper.GetService<SetExplorer>() });
            appPanels.Add(new AppPanel() { id = AppPanelEnum.ImporterUi, viewModel = ServiceHelper.GetService<ImporterUiViewModel>(), window = ServiceHelper.GetService<ImporterUi>() });
        }

        public void OpenWindow(AppWindowEnum id)
        {
            logger.LogTrace("OpenWindow");
            try
            {
                AppWindow appWindow = appWindows.Where(x => x.id == id).First();
                appWindow.window.Show();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public void CloseWindow(AppWindowEnum id)
        {
            try
            {
                AppWindow appWindow = appWindows.Where(x => x.id == id).First();
                appWindow.window.Close();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        public void OpenPanel(string controlName)
        {
            // Guard from an already openned panel
            foreach (var item in rootLayout.RootPanel.Children)
            {
                if (item is LayoutAnchorablePane && ((LayoutAnchorablePane)item).Name == controlName) return;
            }
            // Find corresponding control
            BaseUserControl control = appPanels.Find(tool => tool.window.ControlName == controlName).window;
            if (control == null) return;
            // Open the control in Avalon

            var anch = new LayoutAnchorable()
            {
                IsSelected = true,
                Content = control,
                Title = controlName,
                FloatingHeight = 500,
                FloatingWidth = 300,
            };
            var panel = new LayoutAnchorablePane
            {
                Name = controlName,
                Children = {
                    anch
                },
                DockMinWidth = 200,
                DockMinHeight = 100,
            };
            rootLayout.RootPanel.Children.Add(panel);

        }

        private void HandleLayoutActionEvent(LayoutEventArgs args)
        {
            switch (args.EventType)
            {
                case LayoutEventType.OpenPanel: OpenPanel(args.information); break;
                case LayoutEventType.Save: SaveLayout(args.information); break;
                case LayoutEventType.Load: LoadLayout(args.information); break;
                default: break;
            }
        }

        public void SaveLayout(string layoutName)
        {
            string xmlLayoutString = "";
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(dockingManager);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            File.WriteAllText(GetLayoutPath(layoutName), xmlLayoutString);
        }

        public void LoadLayout(string layoutName)
        {
            try
            {
                var serializer = new XmlLayoutSerializer(dockingManager);
                serializer.LayoutSerializationCallback += (s, args) => { };
                serializer.Deserialize(GetLayoutPath(layoutName));
                foreach (var element in dockingManager.Layout.Descendents().OfType<LayoutAnchorable>())
                {
                    var panel = appPanels.Find(control => control.window.ControlName == element.Title);
                    if (panel != null)
                    {
                        element.Content = panel;
                    }
                }
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public string GetLayoutPath(string layoutName)
        {
            return Path_LayoutFolder + "\\" + layoutName + ".avalonXml";
        }

        class AppWindow
        {
            public AppWindowEnum id;
            public BaseWindow window;
            public BaseViewModel viewModel;
        }

        class AppPanel
        {
            public AppPanelEnum id;
            public BaseUserControl window;
            public BaseViewModel viewModel;
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