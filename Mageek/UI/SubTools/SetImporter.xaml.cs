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

        Visibility isLoading = Visibility.Visible; 
        public Visibility IsLoading { 
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); } 
        }

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
            List<Set> sets = (await MageekApi.RetrieveSets()).ToList();
            foreach (var set in sets) SetList.Add(set);
            SetList = null;
            SetList = sets;
            IsLoading = Visibility.Collapsed;
        }

        private void ImportSet(Set set)
        {
            App.Importer.AddImportToQueue(
                new PendingImport
                {
                    Mode = ImportMode.Set,
                    Title = "[Set] "+set.ReleaseDate.Value.ToShortDateString()+" "+set.Name,
                    Content = set.Code,
                    AsOwned = asOwned,
                }
            );
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var set = ((Set)SetListView.SelectedItem);
            if(set!=null) ImportSet(set);
            Close();
        }

        private void ImportSelectedDecks(object sender, RoutedEventArgs e)
        {
            foreach (var v in SetListView.SelectedItems)
            {
                var set = (Set)v;
                if (set == null) return;
                ImportSet(set);
            }
            Close();
        }
    }
}
