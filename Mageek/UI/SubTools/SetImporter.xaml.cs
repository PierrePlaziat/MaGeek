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

        private void ImportSet(string title)
        {
            App.CARDS.Importer.AddImportToQueue(
                new PendingImport
                {
                    mode = ImportMode.Set,
                    content = title,
                }
            );
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var title = ((ISet)SetListView.SelectedItem).Name;
            ImportSet(title);
            Close();
        }

    }
}
