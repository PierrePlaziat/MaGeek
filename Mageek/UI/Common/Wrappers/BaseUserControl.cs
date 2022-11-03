using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace MaGeek.Wpf.Wrappers
{

    internal class BaseUserControl : UserControl, INotifyPropertyChanged
    {

        #region PropertyChange

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

    }

}
