using System.IO;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using PlaziatTools;
using PlaziatWpf.Docking;
using PlaziatWpf.Mvvm;

namespace PlaziatWpf.Services
{

    public class WindowsService
    {

        private List<AppWindow> appWindows = new();
        private List<AppPanels> appPanels = new();
        private DockingManager dockingManager;

        public void Init(DockingManager dockingManager, List<AppWindow> appWindows, List<AppPanels> appPanels)
        {
            this.dockingManager = dockingManager;
            this.appWindows = appWindows;
            this.appPanels = appPanels;
        }

        public void OpenWindow(string window)
        {
            Logger.Log(window.ToString());
            try
            {
                AppWindow appWindow = appWindows.Where(x => x.id == window).First();
                appWindow.window.Show();
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public void CloseWindow(string window)
        {
            Logger.Log(window.ToString());
            try
            {
                AppWindow appWindow = appWindows.Where(x => x.id == window).First();
                appWindow.window.Close();
            }
            catch (Exception e) { Logger.Log(e); }
        }

        #region Docking system

        public void OpenPanel(string panel)
        {
            Logger.Log(panel.ToString());
            try
            {
                // not first time
                LayoutAnchorable anch = dockingManager.Layout
                    .Descendents().OfType<LayoutAnchorable>()
                    .FirstOrDefault(d => d.Title == panel.ToString());
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
                BaseUserControl control = appPanels.Find(t => t.id == panel).panel;
                anch = new LayoutAnchorable()
                {
                    ContentId = panel.ToString(),
                    Title = panel.ToString(),
                    Content = control,
                    CanFloat = true,
                };
                var anchPane = new LayoutAnchorablePane(anch)
                {
                    Name = panel.ToString(),
                    DockMinWidth = 200,
                };
                var GrpPane = new LayoutAnchorablePaneGroup(anchPane);
                // add to layout
                dockingManager.Layout.RootPanel.Children.Add(GrpPane);
            }
            catch (Exception e) { Logger.Log(e); }
        }

        public void OpenDocument(AbstractDocumentArguments document)
        {
            if (!document.validated) return;
            try
            {
                // not first time
                LayoutDocument anch = dockingManager.Layout
                    .Descendents().OfType<LayoutDocument>()
                    .FirstOrDefault(d => d.ContentId == document.documentId);
                if (anch == null)
                {
                    //  first time
                    IDocument view = ServiceHelper.GetService<IDocument>(); 
                    anch = new LayoutDocument()
                    {
                        Content = view,
                        Title = document.documentTitle,
                        ContentId = document.documentId,
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
                    view.OpenDocument(document); 
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
                    var panel = appPanels.Find(control => control.id.ToString() == element.ContentId);
                    if (panel != null)
                    {
                        element.Content = panel.panel;
                    }
                }
            }
            catch (Exception e) { Logger.Log(e); }
        }

        private string GetLayoutPath(string layoutName)
        {
            string filePath = Paths.Folder_ApplicationData + "\\" + layoutName + ".avalonXml";
            if (!File.Exists(filePath)) SetDefaultLayout(layoutName); 
            return filePath;
        }

        private void SetDefaultLayout(string layoutName)
        {
            File.Copy(
                Paths.Folder_DesktopInstall + "\\Default.avalonXml", 
                Paths.Folder_ApplicationData + "\\"+ layoutName + ".avalonXml", 
                true
            );
        }

        #endregion

    }

    public class AppWindow
    {
        public string id;
        public ObservableViewModel vm;
        public BaseWindow window;
        public AppWindow(string id, ObservableViewModel vm, BaseWindow window)
        {
            this.id = id;
            this.vm = vm;
            this.window = window;
        }
    }

    public class AppPanels
    {
        public string id;
        public ObservableViewModel vm;
        public BaseUserControl panel;
        public AppPanels(string id, ObservableViewModel vm, BaseUserControl panel)
        {
            this.id = id;
            this.vm = vm;
            this.panel = panel;
        }
    }

}