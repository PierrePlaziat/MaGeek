﻿using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using MaGeek.UI;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MaGeek
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private Visibility preventActionVisibility = Visibility.Hidden;
        public Visibility PreventActionVisibility { 
            get {  return preventActionVisibility; }
            set { preventActionVisibility = value; OnPropertyChanged(); } 
        }

        public MainWindow()
        {
            DataContext = this;
            App.STATE.LayoutActionEvent += HandleLayoutAction;
            App.STATE.PreventUIActionEvent += STATE_PreventUIActionEvent;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            InitializeComponent();
        }

        private void STATE_PreventUIActionEvent(bool on)
        {
            if (on) { PreventActionVisibility = Visibility.Visible; }
            else { PreventActionVisibility = Visibility.Collapsed; }
        }

        void HandleLayoutAction(LayoutEventType type)
        {
            switch(type)
            {
                case LayoutEventType.Save: SaveLayout(); break;
                case LayoutEventType.Load: LoadLayout(); break;
                case LayoutEventType.Open_CardInspector: CI.Show(); break;
                case LayoutEventType.Open_CardSearcher: CS.Show(); break;
                case LayoutEventType.Open_DeckContent: DC.Show(); break;
                case LayoutEventType.Open_DeckList: DL.Show(); break;
                case LayoutEventType.Open_DeckStats: DS.Show(); break;
                case LayoutEventType.Open_DeckTable: DT.Show(); break;
                default: break;
            }
        }

        private void SaveLayout()
        {
            string xmlLayoutString = "";
            using (StringWriter fs = new StringWriter())
            {
                XmlLayoutSerializer xmlLayout = new XmlLayoutSerializer(dockingManager);
                xmlLayout.Serialize(fs);
                xmlLayoutString = fs.ToString();
            }
            File.WriteAllText(App.RoamingFolder+"\\Layout.txt", xmlLayoutString);
        }

        private void LoadLayout()
        {
            var serializer = new XmlLayoutSerializer(dockingManager);
            serializer.LayoutSerializationCallback += (s, args) => {};
            serializer.Deserialize(App.RoamingFolder + "\\Layout.txt");
            foreach (var element in dockingManager.Layout.Descendents().OfType<LayoutAnchorable>())
            {
                switch (element.Title)
                {
                    case "Deck List": element.Content = new DeckList(); break;
                    case "Deck Content": element.Content = new DeckContent(); break;
                    case "Deck Stats": element.Content = new DeckStats(); break;
                    case "Deck Table": element.Content = new DeckTable(); break;
                    case "Card Inspector": element.Content = new CardInspector(); break;
                    case "Card Searcher": element.Content = new CardSearcher(); break;
                }
            }
        }

    }

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

}