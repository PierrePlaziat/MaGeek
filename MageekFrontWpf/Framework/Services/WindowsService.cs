using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using PlaziatTools;
using MageekFrontWpf.Framework.AppValues;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekCore.Data;

namespace MageekFrontWpf.Framework.Services
{

    public class WindowsService
    {

        private List<AppWindow> windows = new();
        private List<AppTool> tools = new();
        private DockingManager dockingManager;

        internal void LaunchApp()
        {
            Logger.Log("Start");
            // Init folders
            Folders.InitializeClientFolders();
            // Load app elements
            windows = AppElements.LoadWindows();
            tools = AppElements.LoadTools();
            // Retrieve docking manager
            dockingManager = 
                ((MainWindow) windows
                    .Where(x => x.id == AppWindowEnum.Main)
                    .FirstOrDefault()
                    .window)
                .DockingManager;
            // Show welcome win
            windows.Where(x => x.id == AppWindowEnum.Welcome)
                .FirstOrDefault()
                .window
                .Show();
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
                    ContentId = tool.ToString(),
                    Title = tool.ToString(),
                    Content = control,
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
                dockingManager.Layout.RootPanel.Children.Add(GrpPane);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public void OpenDoc(AbstractDocumentArguments args)
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
                    IDocument view = ServiceHelper.GetService<IDocument>(); 
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
                        dockingManager.Layout.RootPanel.Children.Add(GrpPane);
                    }
                    else
                    {
                        anchPane.Children.Add(anch);
                    }
                    view.OpenDocument(args); 
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
                    var panel = tools.Find(control => control.id.ToString() == element.ContentId);
                    if (panel != null)
                    {
                        element.Content = panel.tool;
                    }
                }
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
        public ObservableViewModel vm;
    }

    public class AppTool
    {
        public AppToolsEnum id;
        public BaseUserControl tool;
        public ObservableViewModel vm;
    }

}