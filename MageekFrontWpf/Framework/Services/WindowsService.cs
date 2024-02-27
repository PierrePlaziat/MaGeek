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

namespace MageekFrontWpf.Framework.Services
{

    public class WindowsService
    {

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
                LayoutAnchorable anch = dockingManager.Layout
                    .Descendents().OfType<LayoutAnchorable>()
                    .FirstOrDefault(d => d.Title == tool.ToString());
                if (anch != null)
                {
                    // already opened
                    if (anch.IsVisible) return;
                    // reopen
                    else
                    {
                        anch.Show();
                        return;
                    }
                }
                //  first time
                BaseUserControl control = tools.Find(t => t.id == tool).tool;
                anch = new LayoutAnchorable()
                {
                    Content = control,
                    Title = tool.ToString(),
                    CanFloat = true,
                };
                var anchPane = new LayoutAnchorablePane(anch)
                {
                    Name = tool.ToString(),
                    DockMinWidth = 200,
                    DockMinHeight = 100,
                };
                var GrpPane = new LayoutAnchorablePaneGroup(anchPane);
                // add to layout
                rootPanel.Children.Add(GrpPane);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        // not generic //
        public void OpenDoc(Deck? deck, Preco? preco)
        {
            if (deck == null && preco == null) return;
            string _id = string.Empty;
            string _title = string.Empty;
            if (deck != null)
            {
                _id = deck.DeckId;
                _title = deck.Title;
            }
            else if (preco != null)
            {
                _id = string.Concat("[",preco.Code,"] ",preco.Title);
                _title = _id;
            }
            try
            {
                // not first time
                LayoutDocument anch = dockingManager.Layout
                    .Descendents().OfType<LayoutDocument>()
                    .FirstOrDefault(d => d.ContentId == _id);
                if (anch == null)
                {
                    //  first time
                    DeckDocument view = ServiceHelper.GetService<DeckDocument>();
                    anch = new LayoutDocument()
                    {
                        Content = view,
                        Title = _title,
                        ContentId = _id,
                        CanFloat = true,
                    };
                    LayoutDocumentPane anchPane = dockingManager.Layout
                        .Descendents().OfType<LayoutDocumentPane>()
                        .FirstOrDefault();
                    if (anchPane == null)
                    {
                        anchPane = new LayoutDocumentPane(anch);
                        LayoutDocumentPaneGroup GrpPane = new LayoutDocumentPaneGroup(anchPane);
                        rootPanel.Children.Add(GrpPane);
                    }
                    else
                    {
                        anchPane.Children.Add(anch);
                    }
                    // Launch document initialization
                    if (deck != null) view.Initialize(deck);
                    else if (preco != null) view.Initialize(preco);
                }
            }
            catch (Exception e) { Logger.Log(e); }
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

    }

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

}