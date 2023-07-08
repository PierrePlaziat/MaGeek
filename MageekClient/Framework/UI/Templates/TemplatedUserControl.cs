﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace MaGeek.UI
{
    public class TemplatedUserControl : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
