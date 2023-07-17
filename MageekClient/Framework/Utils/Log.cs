using MaGeek.Utils.CommonWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace MaGeek.Framework.Utils
{

    public static class Log
    {

        public static void InformUser(string message)
        {
            MessageBox.Show(
                message,
                "Information :",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        public static bool AskUser(string message)
        {
            var response = MessageBox.Show(
                message,
                "Question :",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            return response == MessageBoxResult.Yes;
        }

        internal static string GetInpurFromUser(string reason, string preFill = null)
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
