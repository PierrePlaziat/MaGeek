using MaGeek.Utils.CommonWpf;
using System;
using System.Collections.Concurrent;
using System.Windows;

namespace MaGeek
{

    public static class AppLogger
    {


        public static ConcurrentQueue<string> OutputMessages { get; } = new ConcurrentQueue<string>() { };
        const int maxLog = 30;

        public static void LogMessage(string message)
        {
            OutputMessages.Enqueue(message);
            if (OutputMessages.Count > maxLog)
            {
                string dequeud;
                OutputMessages.TryDequeue(out dequeud);
            }
        }

        public static void ShowMessage(string message)
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
            if (MessageBox.Show(message, "Question:", MessageBoxButton.YesNo,
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

        internal static void LogError(string errorTitle, Exception e)
        {
            string msg = "ERROR : " + errorTitle + " \n\n>>> " + e.Message;
            if (e.InnerException != null)
            {
                msg += " >>> " + e.InnerException.Message;
            }
            LogMessage(msg);
            //ShowMsg(msg);
        }

    }

}
