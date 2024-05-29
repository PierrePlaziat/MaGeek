using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace PlaziatWpf.Mvvm
{

    public class BaseUserControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
