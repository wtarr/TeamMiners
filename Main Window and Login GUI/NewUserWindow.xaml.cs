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
using System.IO;
using Microsoft.Win32;
using TeamMinerClassLibrary;

namespace Main_Window_and_Login_GUI
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        private string filename;
        private UserAccount existingAccount;

        public NewUserWindow()
        {
            InitializeComponent();
        }

        public NewUserWindow(UserAccount useraccount)
        {
            existingAccount = useraccount;
            InitializeComponent();
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ope = new OpenFileDialog();

            var res = ope.ShowDialog();

            if (res == true)
            {
                if (File.Exists(ope.FileName))
                {
                    filename = ope.FileName;
                    lblFilePath.Content = filename;
                }
                else
                {
                    throw new IOException("That is not a valid file selection");
                }
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(filename))
            {

                byte[] imagebyteArray = ReadImageAsByteArray();

                UserAccount ua = new UserAccount();
                ua.Userskin = imagebyteArray;
                ua.UserName = txtUsername.Text;
                ua.Password = txtPassword.Text;

                ua.Create();
            }
            else
            {
                MessageBox.Show("You must select an image");
            }
            this.Close();
        }



        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (existingAccount != null && filename != null)
            {

                byte[] imagebyteArray = ReadImageAsByteArray();
                existingAccount.Userskin = imagebyteArray;

                existingAccount.Update();
                this.DialogResult = true;

            }
            this.Close();


        }

        private byte[] ReadImageAsByteArray()
        {

            //Following a combination of tutorials given here at code project and StackOverflow
            //http://www.codeproject.com/Articles/48619/Reading-and-Writing-BLOB-Data-to-Microsoft-SQL-or
            //http://stackoverflow.com/questions/4902250/insert-blob-in-oracle-database-with-c-sharp

            byte[] imagebyteArray = null;

            // Read the image into the byte array - Using statement means the filestream will be disposed of once its
            // job is done
            using (FileStream filestream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {

                imagebyteArray = new byte[filestream.Length];

                filestream.Read(imagebyteArray, 0, System.Convert.ToInt32(filestream.Length));

                filestream.Close();
            }
            return imagebyteArray;


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

        }


    }
}
