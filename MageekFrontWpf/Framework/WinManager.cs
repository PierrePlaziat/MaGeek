using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using AvalonDock;
using MaGeek.UI;
using static MageekFrontWpf.AppEvents;
using System.IO;
using System;
using System.Collections.Generic;
using MaGeek;
using MageekService.Tools;
using System.Linq;
using MageekFrontWpf.ViewModels;
using MaGeek.UI.Menus;
using MaGeek.UI.Windows.Importers;

namespace MageekFrontWpf.Framework
{

    public class WinManager
    {

        public string Path_LayoutFolder { get; } = Path.Combine(MageekService.Folders.Roaming, "Layout");


        MainWindow mainWindow;
        private List<Tuple<BaseWindow,BaseViewModel>> appWindows;
        private readonly List<TemplatedUserControl> AppPanels = new()
        {
            new CardInspector(),
            new CardSearcher(),
            new DeckContent(),
            new DeckList(),
            new DeckStats(),
            new DeckTable(),
            new SetExplorer(),
        };


        public WinManager(
            MainWindow mainWindow,
            WelcomeWindow welcomeWindow ,
            ImporterUi importerUi,
            PrecoImporter precoImporter,
            TxtImporter txtImporter,
            MainWindowViewModel mainWindowVm,
            WelcomeViewModel welcomeWindowVm,
            ImporterUiViewModel importerUiVm,
            PrecoImporterViewModel precoImporterVm,
            TxtImporterViewModel txtImporterVm
        ) {
            this.mainWindow = mainWindow;
            App.Events.LayoutActionEvent += HandleLayoutActionEvent;

            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(mainWindow,mainWindowVm));
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(welcomeWindow, welcomeWindowVm));
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(importerUi, importerUiVm));
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(precoImporter, precoImporterVm));
            appWindows.Add(new Tuple<BaseWindow, BaseViewModel>(txtImporter, txtImporterVm));
        }

        internal void LaunchMainWin()
        {
            mainWindow.Show();
        }

        internal void OpenWindow(BaseViewModel vm)
        {
            try
            {
                appWindows
                    .Where(w=>w.Item2.GetType()==vm.GetType())
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
            TemplatedUserControl control = AppPanels.Find(tool => tool.ControlName == controlName);
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
                    var panel = AppPanels.Find(control => control.ControlName == element.Title);
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

    }

}