using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using MaGeek.UI;
using MageekSdk.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using static MaGeek.AppEvents;

namespace MaGeek
{

    /// <summary>
    /// This MainWindow uses the AvalonDock like so:
    /// It wraps given "TemplatedUserControl" into AnchorablePanes, 
    /// so the user will be able to manage those here said AppPanels into layout.
    /// There is currently no use of the avalon document logic.
    /// </summary>
    public partial class MainWindow : TemplatedWindow
    {

        /// <summary>
        /// Declare available app panels here
        /// </summary>
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

        public MainWindow()
        {
            Application.Current.MainWindow = this;
            //Application.Current.MainWindow.WindowState = WindowState.Maximized;
            App.Events.LayoutActionEvent += HandleLayoutActionEvent;
            DataContext = this;
            InitializeComponent();
            LoadLayout("User");
        }

        private void HandleLayoutActionEvent(LayoutEventArgs args)
        {
            switch(args.EventType)
            {
                case LayoutEventType.OpenPanel: OpenPanel(args.information); break;
                case LayoutEventType.Save: SaveLayout(args.information); break;
                case LayoutEventType.Load: LoadLayout(args.information); break;
                default: break;
            }
        }

        private void OpenPanel(string controlName)
        {
            // Guard from an already openned panel
            foreach (var item in RootLayout.RootPanel.Children)
            {
                if (item is LayoutAnchorablePane &&((LayoutAnchorablePane)item).Name == controlName) return;
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
            RootLayout.RootPanel.Children.Add(panel);
            
        }

        private void SaveLayout(string layoutName)
        {
            string xmlLayoutString = "";
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(DockingManager);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            File.WriteAllText(GetLayoutPath(layoutName), xmlLayoutString);
        }

        private void LoadLayout(string layoutName)
        {
            try
            {
                var serializer = new XmlLayoutSerializer(DockingManager);
                serializer.LayoutSerializationCallback += (s, args) => { };
                serializer.Deserialize(GetLayoutPath(layoutName));
                foreach (var element in DockingManager.Layout.Descendents().OfType<LayoutAnchorable>())
                {
                    var panel = AppPanels.Find(control => control.ControlName == element.Title);
                    if (panel != null)
                    {
                        element.Content = panel;
                    }
                }
            }
            catch (Exception e) { Logger.Log(e.Message,LogLvl.Debug); }
        }

        private string GetLayoutPath(string layoutName)
        {
            return App.Config.Path_LayoutFolder + "\\" + layoutName + ".avalonXml";
        }

    }

}
