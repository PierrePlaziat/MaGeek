﻿using PlaziatWpf.Controls.TextBoxHelpers;
using System.ComponentModel;
using System.Windows;

namespace MageekDesktopServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        RichTextBoxLogger logger;
        ConsoleAppManager manager;

        public MainWindow(ConsoleAppManager manager)
        {
            this.manager = manager;
            DataContext = this;
            InitializeComponent();
            logger = new RichTextBoxLogger(RTB);
            manager.StandartTextReceived += Manager_StandartTextReceived;
            manager.ErrorTextReceived += Manager_ErrorTextReceived;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Manager_ErrorTextReceived(object sender, string e)
        {
            logger.Log(e, "Red");
        }

        private void Manager_StandartTextReceived(object sender, string e)
        {
            string color = "White";
            if (e.Contains("fail: ")) color = "Red";
            if (e.Contains("/!\\")) color = "Red";
            logger.Log(e, color);
        }

    }
}