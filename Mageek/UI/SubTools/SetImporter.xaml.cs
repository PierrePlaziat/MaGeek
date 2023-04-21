using MaGeek.AppBusiness;
using ScryfallApi.Client.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        bool asOwned = false;
        public bool AsOwned
        {
            get { return asOwned; }
            set { asOwned = value; OnPropertyChanged(); }
        }

        #endregion

        List<Set> setList = new();
        public List<Set> SetList
        {
            get { return setList; }
            set { setList = value; OnPropertyChanged(); }
        }

        public SetImporter()
        {
            DataContext = this;
            InitializeComponent();
            LoadSets().ConfigureAwait(false);
        }

        private async Task LoadSets()
        {
            List<Set> sets = (await MageekUtils.RetrieveSets()).ToList();
            foreach (var set in sets) SetList.Add(set);
            SetList = null;
            SetList = sets;
        }

        private void ImportSet(string code, string date, string type, bool asOwned)
        {
            App.Biz.Importer.AddImportToQueue(
                new PendingImport
                {
                    Mode = ImportMode.Set,
                    Title = "["+date+"] "+type+" set] "+code,
                    Content = code,
                    AsOwned = asOwned,
                }
            );
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var code = ((Set)SetListView.SelectedItem).Code;
            var date = ((Set)SetListView.SelectedItem).ReleaseDate;
            var type = ((Set)SetListView.SelectedItem).SetType;
            ImportSet(code,date.ToString(),type,asOwned);
            Close();
        }

        private void ImportSelectedDecks(object sender, RoutedEventArgs e)
        {
            foreach (var v in SetListView.SelectedItems)
            {
                var code = ((Set)v).Code;
                var date = ((Set)v).ReleaseDate;
                var type = ((Set)v).SetType;
                ImportSet(code, date.ToString(), type,AsOwned);
            }
            Close();
        }
    }
}
