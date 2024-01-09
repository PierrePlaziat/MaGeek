using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using MaGeek.UI;
using static MageekFrontWpf.Framework.AppEvents;
using System.IO;
using System;
using System.Collections.Generic;
using MaGeek;
using MageekService.Tools;
using System.Linq;
using MageekFrontWpf.ViewModels;
using MaGeek.UI.Menus;
using MaGeek.UI.Windows.Importers;
using MageekFrontWpf.Framework.BaseMvvm;

namespace MageekFrontWpf.Framework
{

    enum Windows
    {
        mainWindow, welcomeWindow, precoImporter, txtImporter, proxyPrint, collectionEstimation
    }

    public class WindowsManager
    {

        #region Construction

        public string Path_LayoutFolder { get; } = Path.Combine(MageekService.Folders.Roaming, "Layout");

        MainWindow mainWindow;
        private List<Tuple<BaseWindow,BaseViewModel>> appWindows;
        private readonly List<Tuple<BaseUserControl, BaseViewModel>> appPanels;

        public WindowsManager(
            AppEvents events,
            WelcomeWindow welcomeWindow ,
            MainWindow mainWindow,
            TxtImporter txtImporter,
            PrecoImporter precoImporter,
            DeckStats deckStats,
            CardInspector cardInspector,
            CardSearcher cardSearcher,
            DeckContent deckContent,
            DeckList deckList,
            DeckTable deckTable,
            ImporterUi importerUi,
            WelcomeViewModel welcomeWindowVm,
            MainWindowViewModel mainWindowVm,
            TxtImporterViewModel txtImporterVm,
            PrecoImporterViewModel precoImporterVm,
            DeckStatsViewModel deckStatsViewModel,
            CardInspectorViewModel cardInspectorViewModel,
            CardSearcherViewModel cardSearcherViewModel,
            DeckContentViewModel deckContentViewModel,
            DeckListViewModel deckListViewModel,
            DeckTableViewModel deckTableViewModel,
            ImporterUiViewModel importerUiVm
        ){
            this.mainWindow = mainWindow;
            events.LayoutActionEvent += HandleLayoutActionEvent;
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(mainWindow,mainWindowVm));
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(welcomeWindow, welcomeWindowVm));
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(precoImporter, precoImporterVm));
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(txtImporter, txtImporterVm));
            appPanels.Add(new Tuple<BaseUserControl, BaseViewModel>(deckStats, deckStatsViewModel));
            appPanels.Add(new Tuple<BaseUserControl, BaseViewModel>(cardInspector, cardInspectorViewModel));
            appPanels.Add(new Tuple<BaseUserControl, BaseViewModel>(cardSearcher, cardSearcherViewModel));
            appPanels.Add(new Tuple<BaseUserControl, BaseViewModel>(deckContent, deckContentViewModel));
            appPanels.Add(new Tuple<BaseUserControl, BaseViewModel>(deckList, deckListViewModel));
            appPanels.Add(new Tuple<BaseUserControl, BaseViewModel>(deckTable, deckTableViewModel));
            appPanels.Add(new Tuple<BaseUserControl, BaseViewModel>(importerUi, importerUiVm));
        }

        #endregion

        #region Usage

        internal void LaunchMainWin()
        {
            mainWindow.Show();
        }

        internal void OpenWindow(Windows win)
        {
            try
            {
                appWindows
                    .Where(w=>w.Item2.GetType()==win.GetType())
                    .First()
                    .Item1
                    .Show();
            }
            catch(Exception e)
            {
                Logger.Log(e);
            }
        }

        internal void CloseWindow(BaseViewModel vm)
        {
            try
            {
                appWindows
                    .Where(w => w.Item2.GetType() == vm.GetType())
                    .First()
                    .Item1
                    .Close();
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
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

        public void OpenPanel(string controlName)
        {
            // Guard from an already openned panel
            foreach (var item in mainWindow.RootLayout.RootPanel.Children)
            {
                if (item is LayoutAnchorablePane && ((LayoutAnchorablePane)item).Name == controlName) return;
            }
            // Find corresponding control
            BaseUserControl control = appPanels.Find(tool => tool.Item1.ControlName == controlName).Item1;
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
            mainWindow.RootLayout.RootPanel.Children.Add(panel);

        }

        public void SaveLayout(string layoutName)
        {
            string xmlLayoutString = "";
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(mainWindow.DockingManager);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            File.WriteAllText(GetLayoutPath(layoutName), xmlLayoutString);
        }

        public void LoadLayout(string layoutName)
        {
            try
            {
                var serializer = new XmlLayoutSerializer(mainWindow.DockingManager);
                serializer.LayoutSerializationCallback += (s, args) => { };
                serializer.Deserialize(GetLayoutPath(layoutName));
                foreach (var element in mainWindow.DockingManager.Layout.Descendents().OfType<LayoutAnchorable>())
                {
                    var panel = appPanels.Find(control => control.Item1.ControlName == element.Title);
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

        #endregion

    }

}