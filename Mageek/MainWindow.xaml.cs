using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using MaGeek.UI;
using System.IO;
using System.Windows;

namespace MaGeek
{

    public enum LayoutEventType
    {
        Save,
        Load,
        Open_CardSearcher,
        Open_CardInspector,
        Open_DeckList,
        Open_DeckContent,
        Open_DeckTable,
        Open_DeckStats,
        ResetLayout,
    }

    public partial class MainWindow : Window
    {

        #region Attributes

        LayoutAnchorable anchor_DeckList = new LayoutAnchorable() {
            Title = "DeckList",
            Content = new DeckList(),
            AutoHideMinWidth = 500,
            AutoHideWidth = 500,
            FloatingWidth = 500,
        };
        LayoutAnchorable anchor_DeckContent = new LayoutAnchorable() { 
            Title = "DeckContent", 
            Content = new DeckContent(),
            AutoHideMinWidth = 500,
            AutoHideWidth = 500,
            FloatingWidth = 500,
        };
        LayoutAnchorable anchor_CardInspector = new LayoutAnchorable() { 
            Title = "CardInspector",
            Content = new CardInspector(),
            AutoHideMinWidth = 255,
            AutoHideWidth = 255,
            FloatingWidth = 255,
        };
        LayoutAnchorable anchor_DeckStats = new LayoutAnchorable() { 
            Title = "DeckStats", 
            Content = new DeckStats(),
            AutoHideHeight = 500,
            AutoHideMinHeight = 500,
            FloatingHeight = 500,
        };
        LayoutAnchorable anchor_DeckTable = new LayoutAnchorable() { 
            Title = "DeckTable", 
            Content = new DeckTable(),
            AutoHideHeight = 500,
            AutoHideMinHeight = 500,
            FloatingHeight = 500,
        };
        LayoutAnchorable anchor_CardSearcher;

        #endregion

        #region CTOR

        public MainWindow()
        {
            DataContext = this;
            App.State.LayoutActionEvent += HandleLayoutAction;
            InitializeComponent();
            InitAvalonDock();
        }

        private void InitAvalonDock()
        {
            anchor_CardSearcher = CS;
            anchor_DeckStats.AddToLayout(dockingManager, AnchorableShowStrategy.Bottom);
            anchor_DeckTable.AddToLayout(anchor_DeckStats.Root.Manager, AnchorableShowStrategy.Bottom);
            anchor_DeckList.AddToLayout(dockingManager, AnchorableShowStrategy.Left);
            anchor_DeckContent.AddToLayout(anchor_DeckList.Root.Manager, AnchorableShowStrategy.Left);
            anchor_CardInspector.AddToLayout(dockingManager, AnchorableShowStrategy.Right);
            //anchor_DeckStats.ToggleAutoHide();
            //anchor_DeckList.ToggleAutoHide();
            anchor_CardInspector.FindParent<LayoutAnchorablePane>().DockMinWidth = 255;
            anchor_CardInspector.FindParent<LayoutAnchorablePane>().DockWidth = new GridLength(255);
            dockingManager.UpdateLayout();
            anchor_CardInspector.ToggleAutoHide();
        }

        #endregion

        #region Methods

        void HandleLayoutAction(LayoutEventType type)
        {
            switch(type)
            {
                case LayoutEventType.Save: SaveLayout(); break;
                case LayoutEventType.Load: LoadLayout(); break;
                case LayoutEventType.Open_CardInspector: anchor_CardInspector.Show(); break;
                case LayoutEventType.Open_CardSearcher: anchor_CardSearcher.Show(); break;
                case LayoutEventType.Open_DeckContent: anchor_DeckContent.Show(); break;
                case LayoutEventType.Open_DeckList: anchor_DeckList.Show(); break;
                case LayoutEventType.Open_DeckStats: anchor_DeckStats.Show(); break;
                case LayoutEventType.Open_DeckTable: anchor_DeckTable.Show(); break;
                default: break;
            }
        }

        private void SaveLayout()
        {
            string xmlLayoutString = "";
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(this.dockingManager);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            File.WriteAllText("WriteText.txt", xmlLayoutString);
        }

        private void LoadLayout()
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            serializer.LayoutSerializationCallback += (s, args) => { };
            serializer.Deserialize("WriteText.txt");
        }

        #endregion

    }

}
