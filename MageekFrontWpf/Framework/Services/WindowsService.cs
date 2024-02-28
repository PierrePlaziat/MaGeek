using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using PlaziatTools;
using MageekFrontWpf.Framework.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.Views;
using MageekCore.Data;

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

        public void Initialize(DockingManager avalon)
        {
            Logger.Log("Start");
            Folders.InitClientFolders();
            dockingManager = avalon; 
            rootPanel = avalon.Layout.RootPanel;
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
        public void OpenDoc(DocumentInitArgs args)
        {
            if (!args.validated) return;
            try
            {
                // not first time
                LayoutDocument anch = dockingManager.Layout
                    .Descendents().OfType<LayoutDocument>()
                    .FirstOrDefault(d => d.ContentId == args.documentId);
                if (anch == null)
                {
                    //  first time
                    IDocument view = ServiceHelper.GetService<DeckDocument>();
                    anch = new LayoutDocument()
                    {
                        Content = view,
                        Title = args.documentTitle,
                        ContentId = args.documentId,
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
                    view.Initialize(args); 
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