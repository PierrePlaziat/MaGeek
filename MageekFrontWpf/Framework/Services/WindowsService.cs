using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekFrontWpf.UI.Views;
using MageekFrontWpf.UI.ViewModels;
using MageekCore.Tools;
using System.Windows.Controls;
using AvalonDock.Controls;

namespace MageekFrontWpf.Framework.Services
{

    public class AppWindow
    {
        public AppWindowEnum id;
        public BaseWindow window;
        public BaseViewModel vm;
    }

    public class AppTool
    {
        public AppToolsEnum id;
        public BaseUserControl tool;
        public BaseViewModel vm;
    }

    public class WindowsService : IRecipient<LoadLayoutMessage>, IRecipient<SaveLayoutMessage>
    {

        #region construction


        private List<AppWindow> windows = new();
        private List<AppTool> tools = new();

        private LayoutPanel rootPanel;
        private DockingManager dockingManager;

        public WindowsService()
        {
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Initialize()
        {
            Logger.Log("Start");
            Folders.InitClientFolders();
            rootPanel = ServiceHelper.GetService<MainWindow>().RootLayout.RootPanel;
            dockingManager = ServiceHelper.GetService<MainWindow>().DockingManager;
            windows = WindowsAndTools.LoadWindows();
            tools = WindowsAndTools.LoadTools();
            Logger.Log("Done");
        }

        #endregion

        public void OpenWindow(AppWindowEnum win)
        {
            Logger.Log(win.ToString());
            try
            {
                AppWindow appWindow = windows.Where(x => x.id == win).First();
                appWindow.window.Show();
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public void CloseWindow(AppWindowEnum win)
        {
            Logger.Log(win.ToString());
            try
            {
                AppWindow appWindow = windows.Where(x => x.id == win).First();
                appWindow.window.Close();
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public void OpenTool(AppToolsEnum tool)
        {
            Logger.Log(tool.ToString());
            try
            {

                // not first time

                LayoutAnchorablePane anchPane = dockingManager.Layout
                    .Descendents().OfType<LayoutAnchorablePane>()
                    .FirstOrDefault(d => d.Name == tool.ToString());

                if (anchPane != null)
                {

                    if (anchPane.IsVisible) return;
                    else
                    {
                        //dockingManager.Layout.
                        //AnchorablePaneTabPanel.UnloadedEvent
                        //anchPane
                        //anchPane.ch.Dock();
                        //anchPane.Parent.ReplaceChild = anchPane.Parent;
                        //return;
                    }
                }

                //  first time

                BaseUserControl control = tools.Find(t => t.id == tool).tool;

                var anch = new LayoutAnchorable()
                {
                    IsSelected = true,
                    Content = control,
                    Title = tool.ToString(),
                    FloatingTop = 200,
                    FloatingLeft = 200,
                    FloatingHeight = 200,
                    FloatingWidth = 320,
                    CanFloat = true,
                    
                    
                    ContentId = tool.ToString(),
                };
                anchPane = new LayoutAnchorablePane(anch)
                {
                    Name = tool.ToString(),
                    DockMinWidth = 200,
                    DockMinHeight = 100,
                    FloatingTop = 200,
                    FloatingLeft = 200,
                    FloatingHeight = 200,
                    FloatingWidth = 320,
                };

                var GrpPane = new LayoutAnchorablePaneGroup(anchPane);
                rootPanel.Children.Add(GrpPane);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public void OpenDoc(Deck deck)
        {
            Logger.Log(deck.DeckId + " - " + deck.Title);
            try
            {
                //LayoutDocumentPane docPane = (LayoutDocumentPane)rootLayout.RootPanel.Children.Where(x => x.GetType() == typeof(LayoutDocumentPane)).FirstOrDefault();
                //if (docPane == null)
                //{

                //}
                LayoutDocumentPane docPane = new LayoutDocumentPane
                {
                    DockMinWidth = 200,
                    DockMinHeight = 100,
                };
                rootPanel.Children.Add(docPane);
                LayoutDocument doc = MakeDoc(deck.Title);
                docPane.Children.Add(doc);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        private LayoutDocument MakeDoc(string deckTitle)
        {
            DeckDocument view = ServiceHelper.GetService<DeckDocument>();
            DeckDocumentViewModel vm = ServiceHelper.GetService<DeckDocumentViewModel>();
            view.DataContext = vm;
            LayoutDocument doc = new LayoutDocument()
            {
                //IsSelected = true,
                Content = view,
                Title = deckTitle,
                FloatingHeight = 500,
                FloatingWidth = 300,
            };
            return doc;
        }

        #region Layout gestion

        public void Receive(LoadLayoutMessage message)
        {
            LoadLayout(message.Value);
        }

        public void Receive(SaveLayoutMessage message)
        {
            SaveLayout(message.Value);
        }

        public void SaveLayout(string arg)
        {
            Logger.Log(arg);
            try
            {
                string xmlLayoutString = "";
                using (StringWriter fs = new StringWriter())
                {
                    XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(dockingManager);
                    xmlLayout.Serialize(fs);
                    xmlLayoutString = fs.ToString();
                }
                File.WriteAllText(GetLayoutPath(arg), xmlLayoutString);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public void LoadLayout(string arg)
        {
            Logger.Log(arg);
            try
            {
                var serializer = new XmlLayoutSerializer(dockingManager);
                serializer.LayoutSerializationCallback += (s, args) => { };
                serializer.Deserialize(GetLayoutPath(arg));
                foreach (var element in dockingManager.Layout.Descendents().OfType<LayoutAnchorable>())
                {
                    var panel = tools.Find(control => control.tool.ControlName == element.Title);
                    if (panel != null)
                    {
                        element.Content = panel.tool;
                    }
                }
                rootPanel = dockingManager.Layout.RootPanel;
            }
            catch (Exception e) { Logger.Log(e); }
        }

        private string GetLayoutPath(string layoutName)
        {
            return Folders.LayoutFolder + "\\" + layoutName + ".avalonXml";
        }

        #endregion

    }

}