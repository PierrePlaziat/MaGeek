using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MaGeek.UI.Windows.Importers
{

    public partial class PrecoImporter : Window, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        string path = "D:\\PROJECTS\\VS\\MaGeek\\Preco";

        List<string> precoList = new List<string>();
        public List<string> PrecoList { 
            get { return precoList; }
            set { precoList = value; OnPropertyChanged(); }
        } 

        public PrecoImporter()
        {
            DataContext = this;
            InitializeComponent();
            LoadPrecos();
        }

        private void LoadPrecos()
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                var splited = file.Split("\\");
                string title = splited[splited.Length - 1].Replace(".txt","");
                PrecoList.Add(title);
            }
            OnPropertyChanged("PrecoList");
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ImportPreco(PrecoListView.SelectedItem as string);
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var v in PrecoListView.SelectedItems) ImportPreco(v as string);
            Close();
        }

        private void ImportPreco(string title)
        {
            App.CARDS.Importer.AddImportToQueue(
                new PendingImport
                {
                    title = "[Preco] " + title,
                    mode = ImportMode.List,
                    content = File.ReadAllText(path + "\\" + title + ".txt"),
                }
            );
        }

    }
}
