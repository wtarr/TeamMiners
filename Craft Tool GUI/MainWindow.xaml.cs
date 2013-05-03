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

namespace Craft_Tool_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private UserAccount useraccount;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(UserAccount useraccount)
        {
            this.useraccount = useraccount;
            
            InitializeComponent();

            udpdateCurrentValue(); //cart before horse
        }



        private void stoneHammerBtn(object sender, RoutedEventArgs e)
        {
            Tool tool = new Tool();
            tool.ToolName = "StoneHammer";
            tool.ToolLife = 10;
            tool.UserID = useraccount.UserID;
            tool.Create();
            udpdateCurrentValue();

            //this.Close();
        }

        private void stonePickBtn(object sender, RoutedEventArgs e)
        {
            Tool tool = new Tool();
            tool.ToolName = "StonePick";
            tool.ToolLife = 10;
            tool.UserID = useraccount.UserID;
            tool.Create();
            udpdateCurrentValue();

            //this.Close();
        }

        private void ironHammerBtn(object sender, RoutedEventArgs e)
        {
            Tool tool = new Tool();
            tool.ToolName = "IronHammer";
            tool.ToolLife = 20;
            tool.UserID = useraccount.UserID;
            tool.Create();
            udpdateCurrentValue();

            //this.Close();
        }

        private void ironPickBtn(object sender, RoutedEventArgs e)
        {
            Tool tool = new Tool();
            tool.ToolName = "IronPick";
            tool.ToolLife = 20;
            tool.UserID = useraccount.UserID;
            tool.Create();
            udpdateCurrentValue();

            ////this.Close();
        }

        private void udpdateCurrentValue()
        {
            InventoryItem iv = new InventoryItem(); // create a new inventory item object
            iv.UserID = useraccount.UserID; // match up the userids
            DataTable datatable = iv.ReadAllInventoryAsADataTable(); // get all the inventory back owned by this user as a table

            int woodcount = 0;
            int ironcount = 0;
            int stonecount = 0;

            foreach (DataRow item in datatable.Rows) // for each row in that table
            {

                if ((String)item.ItemArray[1] == "Wood" && item.ItemArray[4] == DBNull.Value && item.ItemArray[5] == DBNull.Value) // is wood and not already in use for a tool or building
                    woodcount++;
                if ((String)item.ItemArray[1] == "Iron" && item.ItemArray[4] == DBNull.Value && item.ItemArray[5] == DBNull.Value) // is iron and not already in use for a tool or building
                    ironcount++;
                if ((String)item.ItemArray[1] == "Stone" && item.ItemArray[4] == DBNull.Value && item.ItemArray[5] == DBNull.Value) // is stone and not already in use for a tool or building
                    stonecount++;                
            }


            // Update the labels
            lblSHWood.Content = "Wood: " + woodcount;
            lblIHWood.Content = "Wood: " + woodcount;
            lblSPWood.Content = "Wood: " + woodcount;
            lblIPWood.Content = "Wood: " + woodcount;
            lblSHStone.Content = "Stone: " + stonecount;
            lblIHIron.Content = "Iron: " + ironcount;
            lblSPStone.Content = "Stone: " + stonecount;
            lblIPIron.Content = "Iron: " + ironcount;

        }





    }
}
