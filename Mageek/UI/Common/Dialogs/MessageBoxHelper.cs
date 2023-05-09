using MaGeek;
using MaGeek.Utils.CommonWpf;
using System;
using System.Windows;
using System.Windows.Interop;

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
            App.State.LogMessage(messageBoxText);
            //result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
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

        internal static string UserInputString(string message, string preFill)
        {
            InputDialog inputDialog = new InputDialog(message, preFill);
            if (inputDialog.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(inputDialog.Answer)) return null;
                return inputDialog.Answer;
            }
            else return null;
        }

        internal static void ShowError(string errorTitle, Exception e)
        {
            string msg = "ERROR : "+ errorTitle + " \n\n>>> " + e.Message;
            if (e.InnerException != null)
            {
                msg += " \n\n>>> " + e.InnerException.Message;
            }
            App.State.LogMessage(msg);
            //ShowMsg(msg);
        }
    }

}
