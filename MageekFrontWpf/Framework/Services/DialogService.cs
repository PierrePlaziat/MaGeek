using MaGeek.Utils.CommonWpf;
using System.IO;
using System;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace MageekFrontWpf.Framework.Services
{
    public class DialogService
    {

        public string SelectAFolder()
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
                else InformUser("Couldnt find this path:\n " + dlg.SelectedPath);
            }
            return null;
        }

        public string SelectAFile(string filter)
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
                if (File.Exists(dlg.FileName)) return dlg.FileName;
                else InformUser("Couldnt find this file:\n " + dlg.FileName);
            }
            return null;
        }

        public void InformUser(string message)
        {
            MessageBox.Show(
                message,
                "Information :",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        public bool AskUser(string message)
        {
            var response = MessageBox.Show(
                message,
                "Question :",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            return response == MessageBoxResult.Yes;
        }

        public string GetInpurFromUser(string reason, string preFill = null)
        {
            InputDialog inputDialog = new(reason, preFill);
            if (inputDialog.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(inputDialog.Answer)) return null;
                return inputDialog.Answer;
            }
            else return null;
        }

    }
}