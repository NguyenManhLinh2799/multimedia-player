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
using System.Windows.Shapes;

namespace multimedia_player
{
    /// <summary>
    /// Interaction logic for NameDialog.xaml
    /// </summary>
    
    public partial class NameDialog : Window
    {
        public string name;
        public NameDialog()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text == "")
            {
                name = "Playlist";
            } else
            {
                name = NameTextBox.Text;
            }
            this.DialogResult = true;
            this.Close();
        }
    }
}
