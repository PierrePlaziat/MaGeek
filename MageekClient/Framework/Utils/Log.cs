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

        #region Config

        //TODO use App config
        private static readonly int fileHistoryLimit = 1000;
        private static readonly int inAppHistoryLimit = 30;
        private static readonly bool writeToFile = false;
        private static readonly bool writeToConsole = true;

        #endregion

        public static Queue<LogMessage> Messages { get; } = new Queue<LogMessage>() { };

        #region Methods

        public static void Write(string message)
        {
            LogMessage logLine = new LogMessage()
            {
                Message = message,
                Instant = DateTime.Now,
                Level = LogLevel.Info
            };
            Write(logLine);
        }

        public static void Write(Exception exception, string source = "")
        {
            string message = source + " >> " + exception.Message;
            if (exception.InnerException != null) message += " >>> " + exception.InnerException.Message;
            LogMessage logLine = new LogMessage()
            {
                Message = message,
                Instant = DateTime.Now,
                Level = LogLevel.Error
            };
            Write(logLine);
        }

        private static void Write(LogMessage logLine)
        {
            Messages.Enqueue(logLine);
            if (writeToConsole) Console.WriteLine(logLine);
            if (writeToFile) File.AppendAllText(App.Config.Path_Log, logLine.ToString()); // TODO empty file sometimes
            if (Messages.Count > inAppHistoryLimit) Messages.Dequeue();
        }

        public static void InformUser(string message)
        {
            Write("Information :" + message);
            MessageBox.Show(
                message,
                "Information :",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        public static bool AskUser(string message)
        {
            Write("Question :" + message);
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
            Write("Input needed :" + reason);
            InputDialog inputDialog = new InputDialog(reason, preFill);
            if (inputDialog.ShowDialog() == true)
            {
                if (string.IsNullOrEmpty(inputDialog.Answer)) return null;
                return inputDialog.Answer;
            }
            else return null;
        }

        #endregion

    }

    #region Data formats

    public struct LogMessage
    {
        public DateTime Instant;
        public LogLevel Level;
        public string Message;

        public override string ToString()
        {
            return "[" + Instant + "] " + Level + " - " + Message;
        }
    }

    public enum LogLevel { Info, Error }

    #endregion

}
