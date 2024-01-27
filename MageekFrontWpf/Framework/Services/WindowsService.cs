using AvalonDock.Layout.Serialization;
using AvalonDock.Layout;
using System.IO;
using System;
using System.Collections.Generic;
using MageekService.Tools;
using System.Linq;
using Microsoft.Extensions.Logging;
using AvalonDock;
using MageekFrontWpf.Framework.BaseMvvm;
using CommunityToolkit.Mvvm.Messaging;
using MageekFrontWpf.AppValues;
using MageekFrontWpf.UI.Views.AppWindows;

namespace MageekFrontWpf.Framework.Services
{

    public class WindowsService : 
        IRecipient<LoadLayoutMessage>,
        IRecipient<SaveLayoutMessage>
    {

        private readonly ILogger<WindowsService> logger;

        public WindowsService(ILogger<WindowsService> logger)
        {
            this.logger = logger;
            WeakReferenceMessenger.Default.RegisterAll(this);
        }

        private readonly string Path_LayoutFolder = Path.Combine(MageekService.Folders.Roaming, "Layout");
        private List<AppWindow> appWindows = new();
        private List<AppPanel> appPanels = new();

        LayoutRoot rootLayout;
        DockingManager dockingManager;

        public void Init()
        {
            if (!File.Exists(Path_LayoutFolder)) Directory.CreateDirectory(Path_LayoutFolder);
            rootLayout = ServiceHelper.GetService<MainWindow>().RootLayout;
            dockingManager = ServiceHelper.GetService<MainWindow>().DockingManager;
            appWindows = WindowsAndPanels.GetWindows();
            appPanels = WindowsAndPanels.GetPanels();
        }

        public void Receive(LoadLayoutMessage message)
        {
            LoadLayout(message.Value);
        }
        
        public void Receive(SaveLayoutMessage message)
        {
            SaveLayout(message.Value);
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

        public void OpenPanel(AppPanelEnum controlName)
        {
            //// Guard from an already openned panel
            //foreach (var item in rootLayout.RootPanel.Children)
            //{
            //    if (item is LayoutAnchorablePane && (LayoutAnchorablePane)item == controlName) return;
            //}
            // Find corresponding control
            BaseUserControl control = appPanels.Find(tool => tool.id == controlName).window;
            if (control == null) return;
            // Open the control in Avalon

            var anch = new LayoutAnchorable()
            {
                IsSelected = true,
                Content = control,
                Title = controlName.ToString(),
                FloatingHeight = 500,
                FloatingWidth = 300,
            };
            var panel = new LayoutAnchorablePane
            {
                Name = controlName.ToString(),
                Children = {
                    anch
                },
                DockMinWidth = 200,
                DockMinHeight = 100,
            };
            rootLayout.RootPanel.Children.Add(panel);

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
            File.WriteAllText(GetLayoutPath("Layout"), xmlLayoutString);
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

        public class AppWindow
        {
            public AppWindowEnum id;
            public BaseWindow window;
            public BaseViewModel viewModel;
        }

        public class AppPanel
        {
            public AppPanelEnum id;
            public BaseUserControl window;
            public BaseViewModel viewModel;
        }

    }

}