using MtgApiManager.Lib.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MaGeek.UI.Windows.Importers
{
    public partial class SetImporter : Window, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        List<ISet> setList = new List<ISet>();
        public List<ISet> SetList
        {
            get { return setList; }
            set { setList = value; OnPropertyChanged(); }
        }

        public SetImporter()
        {
            DataContext = this;
            InitializeComponent();
            LoadSets();
        }

        private void LoadSets()
        {
            foreach (var set in App.CARDS.Importer.GetExistingSets())
            {
                SetList.Add(set);
            }
            OnPropertyChanged("SetList");
        }

        private void ImportSet(string title, string date, string type)
        {
            App.CARDS.Importer.AddImportToQueue(
                new PendingImport
                {
                    mode = ImportMode.Set,
                    title = "["+date+"] "+type+" set] "+title,
                    content = title,
                }
            );
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var title = ((ISet)SetListView.SelectedItem).Name;
            var date = ((ISet)SetListView.SelectedItem).ReleaseDate;
            var type = ((ISet)SetListView.SelectedItem).Type;
            ImportSet(title,date,type);
            Close();
        }

        private void ImportSelectedDecks(object sender, RoutedEventArgs e)
        {
            foreach (var v in SetListView.SelectedItems)
            {
                var title = ((ISet)v).Name;
                var date = ((ISet)v).ReleaseDate;
                var type = ((ISet)v).Type;
                ImportSet(title, date, type);
            }
            Close();
        }
    }
}
