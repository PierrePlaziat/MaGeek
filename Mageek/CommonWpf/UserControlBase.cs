using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace MaGeek.Utils.CommonWpf
{

    internal class UserControlBase : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
