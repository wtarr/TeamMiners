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

namespace Mine_Stack_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserAccount useraccount;        
        StackItem stack;
        List<StackItem> stackList;
        StackItem lastItemInStack;
        int orderLastItem;

        public MainWindow(UserAccount useraccount)
        {
            this.useraccount = useraccount;
            stack = new StackItem();
            stack.Xcoord = useraccount.XCoord;
            stack.Ycoord = useraccount.YCoord;
            lastItemInStack = stack.FindLastStackItemAtCoordinate();
            stackList = new List<StackItem>();
            orderLastItem = lastItemInStack.ItemOrder;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();

            for (int i = 1; i < orderLastItem+1; i++)
            {
                StackItem temp = new StackItem();
                temp.Xcoord = useraccount.XCoord;
                temp.Ycoord = useraccount.YCoord;
                temp.ItemOrder = i;
                temp.Read();
                if (temp.Userid == -1) // only add items that were not mined
                    stackList.Add(temp);                      
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (stackList.Count > 0)
            {
                StackItem temp = stackList.First<StackItem>();
                temp.Userid = useraccount.UserID;
                bool success = temp.Update();
                if (success)
                {
                    stackList.Remove(temp);
                    listBox1.Items.Add(temp.ItemName);
                }
            }            
        }
    }//class
}//namespace
