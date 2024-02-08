using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.Messaging;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using MageekFrontWpf.Framework.BaseMvvm;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.UI.Views.AppWindows;
using MageekServices.Data;
using MageekServices.Data.Collection.Entities;
using MageekFrontWpf.UI.Views;
using MageekFrontWpf.UI.ViewModels;

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

        private readonly string Path_LayoutFolder = Path.Combine(Folders.Roaming, "Layout");

        private List<AppWindow> windows = new();
        private List<AppTool> tools = new();

        private LayoutRoot rootLayout;
        private DockingManager dockingManager;
        

        private readonly ILogger<WindowsService> logger;

        public WindowsService(ILogger<WindowsService> logger)
        {
            this.logger = logger;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        public void Init()
        {
            if (!File.Exists(Path_LayoutFolder)) Directory.CreateDirectory(Path_LayoutFolder);
            rootLayout = ServiceHelper.GetService<MainWindow>().RootLayout;
            dockingManager = ServiceHelper.GetService<MainWindow>().DockingManager;
            windows = WindowsAndTools.GetWindows();
            tools = WindowsAndTools.GetTools();
        }

        #endregion

        public void OpenWindow(AppWindowEnum win)
        {
            logger.LogTrace("OpenWindow : " + win);
            try
            {
                AppWindow appWindow = windows.Where(x => x.id == win).First();
                appWindow.window.Show();
            }
            catch (Exception e) { logger.LogError(e.Message); }
        }

        public void CloseWindow(AppWindowEnum win)
        {
            logger.LogTrace("CloseWindow");
            try
            {
                AppWindow appWindow = windows.Where(x => x.id == win).First();
                appWindow.window.Close();
            }
            catch (Exception e) { logger.LogError(e.Message); }
        }

        public void OpenTool(AppToolsEnum tool)
        {
            logger.LogTrace("OpenTool : " + tool);
            try
            {
                BaseUserControl control = tools.Find(t => t.id == tool).tool;
                if (control == null) return;

                var anch = new LayoutAnchorable()
                {
                    IsSelected = true,
                    Content = control,
                    Title = tool.ToString(),
                    FloatingHeight = 500,
                    FloatingWidth = 300,
                };
                var anchPane = new LayoutAnchorablePane
                {
                    Name = tool.ToString(),
                    Children = {
                        anch
                    },
                    DockMinWidth = 200,
                    DockMinHeight = 100,
                };
                rootLayout.RootPanel.Children.Add(anchPane);
            }
            catch (Exception e) { logger.LogError(e.Message); }
        }

        public void OpenDoc(Deck deck)
        {
            logger.LogTrace("OpenDocument : " + deck.Title);
            try
            {

                var view = ServiceHelper.GetService<DeckDocument>();
                var vm = ServiceHelper.GetService<DeckDocumentViewModel>();
                view.DataContext = vm;

                //BaseUserControl control = tools.Find(tool => tool.id == AppToolsEnum.DeckContent).tool;
                //if (control == null) return;

                LayoutDocumentPane docPane = (LayoutDocumentPane)rootLayout.RootPanel.Children.Where(x=>x.GetType() == typeof(LayoutDocumentPane)).FirstOrDefault();
                if (docPane == null)
                {
                    docPane = new LayoutDocumentPane
                    {
                        DockMinWidth = 200,
                        DockMinHeight = 100,
                    };
                    rootLayout.RootPanel.Children.Add(docPane);
                }

                var doc = new LayoutDocument()
                {
                    IsSelected = true,
                    Content = view,
                    Title = deck.Title,
                    FloatingHeight = 500,
                    FloatingWidth = 300,
                };

                docPane.Children.Add(doc);
            }
            catch (Exception e) { logger.LogError(e.Message); }
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
            logger.LogTrace("SaveLayout : " + arg);
            try
            {
                string xmlLayoutString = "";
                using (StringWriter fs = new StringWriter())
                {
                    XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(dockingManager);
                    xmlLayout.Serialize(fs);
                    xmlLayoutString = fs.ToString();
                }
                File.WriteAllText(GetLayoutPath("Layout"), xmlLayoutString);
            }
            catch (Exception e) { logger.LogError(e.Message); }
        }

        public void LoadLayout(string arg)
        {
            logger.LogTrace("LoadLayout : " + arg);
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
                        element.Content = panel;
                    }
                }
            }
            catch (Exception e) { logger.LogError(e.Message); }
        }

        private string GetLayoutPath(string layoutName)
        {
            return Path_LayoutFolder + "\\" + layoutName + ".avalonXml";
        }

        #endregion

    }

}