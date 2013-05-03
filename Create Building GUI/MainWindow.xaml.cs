#region Using Statements
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeamMinerClassLibrary;
using System.Data; 
#endregion

namespace Create_Building_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserAccount userAccount;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(UserAccount userAccount)
        {
            InitializeComponent();
            this.userAccount = userAccount;
        }

        private void createBuildingBtn(object sender, RoutedEventArgs e)
        {
            Building building = new Building();
            building.Length =  Convert.ToInt16(textBox1.Text);
            building.Width = Convert.ToInt16(textBox2.Text);
            
            if ((bool)radioButton1.IsChecked)
            {
                building.ForSale = 0;
            }
            else 
                building.ForSale = 1;

            building.UserID = userAccount.UserID;
            Point point = Move();
            building.XCoord = (int)point.X;
            building.YCoord = (int)point.Y;
            building.Create();

            this.Close();
        }

        private void deleteBuildingBtn(object sender, RoutedEventArgs e)
        {
            
        }

        private Point Move()
        {
            MapCell mp = new MapCell();
            var mapcelltable = mp.ReadAllMapCellItemsAsDataTable();
            List<Point> listofpoints = new List<Point>();

            // Build a list of points that are known to be occupied in the mapcell table
            foreach (DataRow dr in mapcelltable.Rows)
            {
                if (!dr.IsNull("xcoord") && !dr.IsNull("ycoord"))
                {
                    Int16 x = (Int16)dr.ItemArray[0];
                    Int16 y = (Int16)dr.ItemArray[1];
                    Point p = new Point(x, y);
                    listofpoints.Add(p);
                }

            }


            //Determine a random location for our user
            Random rand = new Random();

            int xpoint = rand.Next(0, 100);
            int ypoint = rand.Next(0, 50);
            Point testPoint = new Point(xpoint, ypoint);

            // Check if the list contains this point already and if
            // so go again
            while (listofpoints.Contains(testPoint))
            {
                xpoint = rand.Next(0, 100);
                ypoint = rand.Next(0, 50);
                testPoint = new Point(xpoint, ypoint);
            }
            return testPoint;
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }

}
