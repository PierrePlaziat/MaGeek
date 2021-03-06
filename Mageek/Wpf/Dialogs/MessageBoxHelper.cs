using MaGeek.Utils.CommonWpf;
using System;
using System.Windows;

namespace Plaziat.CommonWpf
{

    public static class MessageBoxHelper
    {

        public static void ShowMsg(string message)
        {
            string messageBoxText = message;
            string caption = "Message:";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Asterisk;
            MessageBoxResult result;
            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
        }

        public static bool AskUser(string message)
        {
            if (MessageBox.Show(message,"Question:",MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                return true;
            }
            else return false;
        }

        internal static string UserInputString(string message)
        {
            InputDialog inputDialog = new InputDialog(message, "New Deck");
            if (inputDialog.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(inputDialog.Answer)) return null;
                return inputDialog.Answer;
            }
            else return null;
        }
    }

}
