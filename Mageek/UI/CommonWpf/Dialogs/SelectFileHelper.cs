using System;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Plaziat.CommonWpf
{
    internal class SelectFileHelper
    {

        public static string SelectAFolder()
        {

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = Environment.SpecialFolder.Desktop.ToString();
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return dlg.FileName;
            }
            return null;
        }

        public static string SelectAFile(string filter, string defaultExt)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = filter;
            dlg.DefaultExt = defaultExt;
            if (dlg.ShowDialog() == true) return dlg.FileName;
            return null;
        }

    }

}
