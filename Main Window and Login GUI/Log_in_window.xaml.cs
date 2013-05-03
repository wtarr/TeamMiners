using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeamMinerClassLibrary;
using System.IO;
using Microsoft.Win32;

namespace Main_Window_and_Login_GUI
{
    /// <summary>
    /// Interaction logic for Log_in_window.xaml
    /// </summary>
    public partial class Log_in_window : Window
    {

        UserAccount ua;

        public Log_in_window(out UserAccount ua)
        {            
            ua = new UserAccount();
            this.ua = ua;
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow nsw = new NewUserWindow();           
            nsw.ShowDialog();           
                
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            
            ua.UserName = txtUsername.Text;            

            ua.Read();

            if (ua.UserID == 0)
            {
                MessageBox.Show("That username does not exist in our database");
            }
            else
            {
                if (!ua.Password.Equals(txtPassword.Text))
                    MessageBox.Show("That password does not match the stored password");
                else
                    this.Close();
            }   
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
