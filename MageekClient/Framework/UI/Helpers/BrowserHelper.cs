using System;
using System.IO;
using System.Windows.Forms;
using MaGeek.Framework.Utils;

namespace MaGeek.AppFramework.UI.Utils
{
    internal class BrowserHelper
    {

        public static string SelectAFolder()
        {
            var dlg = new FolderBrowserDialog
            {
                InitialDirectory = Environment.SpecialFolder.Desktop.ToString(),
                AddToRecent = false,
                ShowPinnedPlaces = true
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(dlg.SelectedPath)) return dlg.SelectedPath;
                else Log.InformUser("Couldnt find this path:\n " + dlg.SelectedPath);
            }
            return null;
        }

        public static string SelectAFile(string filter)
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = Environment.SpecialFolder.Desktop.ToString(),
                AddToRecent = false,
                ShowPinnedPlaces = true,
                Filter = filter,
                Multiselect = false
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if(File.Exists(dlg.FileName)) return dlg.FileName;
                else Log.InformUser("Couldnt find this file:\n " + dlg.FileName);
            }
            return null;
        }

    }

}
