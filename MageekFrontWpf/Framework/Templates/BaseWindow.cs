using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MaGeek.UI
{

    public class BaseWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
