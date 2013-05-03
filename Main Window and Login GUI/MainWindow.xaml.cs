#region Using statements
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
using System.Data;
using TeamMinerClassLibrary;
using System.IO;
using Microsoft.Win32;
using Craft_Tool_GUI;
using Create_Building_GUI;
using Mine_Stack_GUI;
using Share_Item_GUI; 
#endregion


namespace Main_Window_and_Login_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        UserAccount useraccount; // This is instantiated on startup
        MapCell mp = new MapCell();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            Log_in_window login = new Log_in_window(out useraccount);
            login.Owner = this;
            login.ShowDialog();

            useraccount.OnlineStatus = 1;

            useraccount.Update();

            LoadUsersImage();

            lblUsername.Content = lblUsername.Content + " " + useraccount.UserName;
            lblOnlineStatus.Content = lblOnlineStatus.Content + " " + useraccount.OnlineStatus;
            lblXCoord.Content = lblXCoord.Content + " " + useraccount.XCoord;
            lblYCoord.Content = lblYCoord.Content + " " + useraccount.YCoord;


            UpdateMap();

        }

        private void LoadUsersImage()
        {
            useraccount.Read();
            
            if (useraccount.Userskin != null)
            {
                File.Delete("image.jpg");
                FileStream FS = new FileStream("image.jpg", FileMode.Create);
                FS.Write(useraccount.Userskin, 0, useraccount.Userskin.Length);
                FS.Close();
                FS = null;


                // Loading to a WPF image
                //http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/35a38027-2289-4551-92bb-313b5adce403/ 
                // and the bitmapcacheoption solution from to get it to refresh
                //http://stackoverflow.com/questions/569561/dynamic-loading-of-images-in-wpf
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri("image.jpg", UriKind.Relative);                
                bi.EndInit();
                imgUserSkin.Source = bi;
            }
        }

        private void btnBuildingRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshBuildingTable();

        }

        private void RefreshBuildingTable()
        {
            Building bld = new Building();
            bld.UserID = useraccount.UserID;
            DataTable dt = bld.ReadAllBuildingDataAsDataTable();

            dataGridBuilding.ItemsSource = dt.AsDataView();
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void btnRefreshTools_Click(object sender, RoutedEventArgs e)
        {
            RefreshToolTable();
        }

        private void RefreshToolTable()
        {
            Tool tool = new Tool();
            tool.UserID = useraccount.UserID;
            DataTable dt = tool.ReadAllToolAsDataTable();

            dataGridTool.ItemsSource = dt.AsDataView();
        }

        private void btnRefreshInventory_Click(object sender, RoutedEventArgs e)
        {
            RefreshInventoryTable();
        }

        private void RefreshInventoryTable()
        {
            InventoryItem iv = new InventoryItem();
            iv.UserID = useraccount.UserID;
            DataTable dt = iv.ReadAllInventoryAsADataTable();

            dataGridInventory.ItemsSource = dt.AsDataView();
        }

        private void btnRefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            RefreshUsersTable();
        }

        private void RefreshUsersTable()
        {
            UserAccount ua = new UserAccount();
            DataTable dt = ua.ReadAllSecureUsersAsADataTable();

            dataGridSecureUserData.ItemsSource = dt.AsDataView();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            useraccount.OnlineStatus = 0; // Update the users status as being offline
            useraccount.Update(); // Save the change
            File.Delete("image.jpg"); // Delete the locally saved image that was obtained from the database
        }

        private void btnRefreshMap_Click(object sender, RoutedEventArgs e)
        {
            UpdateMap();
        }

        private void UpdateMap()
        {
            mp = new MapCell();

            var usertable = useraccount.ReadAllSecureUsersAsADataTable();
            var mapcelltable = mp.ReadAllMapCellItemsAsDataTable();

            HashSet<Point> listofpoints = new HashSet<Point>();

            foreach (DataRow dr in usertable.Rows)
            {
                if (!dr.IsNull("xcoord") && !dr.IsNull("ycoord"))
                {
                    Ellipse player = new Ellipse();
                    player.Width = 5;
                    player.Height = 5;
                    player.Stroke = System.Windows.Media.Brushes.Black;

                    string username = (String)dr.ItemArray[0];
                    if (useraccount.UserName.Equals(username))
                        player.Fill = System.Windows.Media.Brushes.Red;
                    canvasMap.Children.Add(player);

                    Int16 x = (Int16)dr.ItemArray[2];
                    Int16 y = (Int16)dr.ItemArray[3];

                    Canvas.SetLeft(player, x * 5);
                    Canvas.SetTop(player, y * 5);

                    Point p = new Point(x, y);
                    listofpoints.Add(p);
                }

            }

            foreach (DataRow dr in mapcelltable.Rows)
            {
                if (!dr.IsNull("xcoord") && !dr.IsNull("ycoord"))
                {
                    Int16 x = (Int16)dr.ItemArray[0];
                    Int16 y = (Int16)dr.ItemArray[1];
                    Point p = new Point(x, y);

                    if (!listofpoints.Contains(p))
                    {

                        Rectangle mine = new Rectangle();
                        mine.Width = 5;
                        mine.Height = 5;
                        mine.Stroke = System.Windows.Media.Brushes.Black;
                        if (!dr.IsNull("buildingid"))
                            mine.Fill = System.Windows.Media.Brushes.Blue;
                        canvasMap.Children.Add(mine);

                        Canvas.SetLeft(mine, x * 5);
                        Canvas.SetTop(mine, y * 5);

                        listofpoints.Add(p);
                    }
                }

            }



        }

        private void btnMove_Click(object sender, RoutedEventArgs e)
        {
            Move();
            UpdateMap();
        }

        private void Move()
        {
            mp = new MapCell();
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


            // If all is OK go ahead and store it.
            lblXCoord.Content = "XCoord: " + xpoint.ToString();
            lblYCoord.Content = "YCoord: " + ypoint.ToString();

            

            mp = new MapCell();
            mp.Userid = useraccount.UserID;
            mp.XCoord = xpoint;
            mp.YCoord = ypoint;
            mp.Currentblock = 1;
            mp.Buildingid = 0;

            mp.Create();

            useraccount.Read(); // the trigger fired from creating a mapcell will update the users XY also so refresh the user

            // Now we must populate the stack for that coordinate so that it can be mined
            PopulateStackItems(xpoint, ypoint);
        }

        private void PopulateStackItems(int xpoint, int ypoint)
        {

            Stack<StackItem> itemStack = new Stack<StackItem>();                       

            Random rand = new Random();

            // Determine the stack height
            int stackheight = rand.Next(6, 15);

            //Lava block is always last so create one and push it on the stack
            StackItem si = new StackItem();
            si.ItemName = "Lava";
            si.ItemOrder = stackheight;
            si.ToolRequired = 0;
            si.Xcoord = xpoint;
            si.Ycoord = ypoint;

            itemStack.Push(si);
            
            for (int i = stackheight-1; i > 0; i--)
            {
                // populate the rest
                // items avail are Earth, Stone, Iron, Wood
                // Pick another random number
                int luckDip = rand.Next(0, 4);
                switch (luckDip)
                {
                    case 0: 
                        //Earth
                        si = new StackItem();
                        si.ItemName = "Earth";
                        si.ItemOrder = i;
                        si.ToolRequired = 0;
                        si.Xcoord = xpoint;
                        si.Ycoord = ypoint;
                        itemStack.Push(si);
                        break;
                    case 1:
                        //Stone
                        si = new StackItem();
                        si.ItemName = "Stone";
                        si.ItemOrder = i;
                        si.ToolRequired = 0;
                        si.Xcoord = xpoint;
                        si.Ycoord = ypoint;
                        itemStack.Push(si);
                        break;
                    case 2:
                        //Iron
                        si = new StackItem();
                        si.ItemName = "Iron";
                        si.ItemOrder = i;
                        si.ToolRequired = 1;
                        si.Xcoord = xpoint;
                        si.Ycoord = ypoint;
                        itemStack.Push(si);
                        break;
                    case 3:
                        //Wood
                        si = new StackItem();
                        si.ItemName = "Wood";
                        si.ItemOrder = i;
                        si.ToolRequired = 0;
                        si.Xcoord = xpoint;
                        si.Ycoord = ypoint;
                        itemStack.Push(si);
                        break;
                }
                
            }

            // Our stack is now complete
            // Now save it to the Database
            while (itemStack.Count != 0)
            {
                si = itemStack.Pop();
                si.Create();
            }


        }

        private void TabSelectedChanged(object sender, SelectionChangedEventArgs e)
        {           
            
            if (tabUsers.IsSelected)
                RefreshUsersTable();

            if (tabInventory.IsSelected)
                RefreshInventoryTable();

            if (tabTool.IsSelected)
                RefreshToolTable();

            if (tabBuilding.IsSelected)
                RefreshBuildingTable();
        }

        private void lblChangeSkin_KeyUp(object sender, KeyEventArgs e)
        {
            MessageBox.Show("Test");
        }

        private void ImageClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                NewUserWindow nuw = new NewUserWindow(useraccount);
                //nuw.l
                nuw.btnAddUser.Content = "Update";
                nuw.lblNewUser.Content = "Update User skin";
                nuw.txtPassword.IsEnabled = false;
                nuw.txtUsername.IsEnabled = false;
                nuw.btnAddUser.IsEnabled = false;
                nuw.btnAddUser.Visibility = Visibility.Hidden;
                nuw.btnUpdate.IsEnabled = true;
                nuw.btnUpdate.Visibility = Visibility.Visible;

                nuw.ShowDialog();

                if (nuw.DialogResult.HasValue && nuw.DialogResult.Value)
                    LoadUsersImage();
                               
            }
        }

        private void imgUserSkin_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void btn_Craft_Click(object sender, RoutedEventArgs e)
        {
            Craft_Tool_GUI.MainWindow craft = new Craft_Tool_GUI.MainWindow(useraccount);
            craft.ShowDialog();


        }

        private void btnBuild_Click(object sender, RoutedEventArgs e)
        {
            Create_Building_GUI.MainWindow build = new Create_Building_GUI.MainWindow(useraccount);
            build.ShowDialog();
        }

        private void btn_mine_Click(object sender, RoutedEventArgs e)
        {

            if (useraccount.XCoord > -1 && useraccount.YCoord > -1)
            {
                Mine_Stack_GUI.MainWindow mine = new Mine_Stack_GUI.MainWindow(useraccount);
                mine.ShowDialog();
            }
            else
            {
                MessageBox.Show("You must move on to the map before you can mine an item");
            }
        }

        private void btn_Share_Click(object sender, RoutedEventArgs e)
        {
            Share_Item_GUI.MainWindow share = new Share_Item_GUI.MainWindow(useraccount);
            share.ShowDialog();
        }

       

    }
}
