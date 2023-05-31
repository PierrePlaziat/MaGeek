using ScryfallApi.Client.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MaGeek.UI
{
    public partial class SetExplorer : TemplatedUserControl
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

        public SetExplorer()
        {
            DataContext = this;
            InitializeComponent();
            LoadSets();//.ConfigureAwait(false);
        }

        private /*async Task*/void LoadSets()
        {
            //List<Set> sets = (await MageekApi.RetrieveSets()).ToList();
            //foreach (var set in sets) SetList.Add(set);
            SetList = null;
            //SetList = sets;
            IsLoading = Visibility.Collapsed;
        }
    }
}
