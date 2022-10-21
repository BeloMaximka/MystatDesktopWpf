﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using MystatDesktopWpf.Domain;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for MainMenuUserControl.xaml
    /// </summary>
    public partial class MainMenuUserControl : UserControl
    {
        public MainMenuUserControl()
        {
            this.DataContext = new MainMenuViewModel();
            InitializeComponent();
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            // Back to login
            Transitioner.MovePreviousCommand.Execute(null, null);
        }
    }
}
