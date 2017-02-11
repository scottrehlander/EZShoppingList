using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;

namespace ShoppingList
{
    public partial class ChangeAisleDialog : PhoneApplicationPage
    {
        public event ChangesCompletedHandler ChangesCompleted;
        public delegate void ChangesCompletedHandler(ChangeAisleDialog sender);

        private int selectedValue = -1;
        public int SelectedValue { get { return selectedValue; } }

        public ChangeAisleDialog(string itemName)
        {
            InitializeComponent();

            aisleText.Text = "Swipe to choose aisle for " + itemName + ":";
        }

        private void WaitThenHide()
        {
            System.Threading.Thread thread = new System.Threading.Thread(
              new System.Threading.ThreadStart(
                delegate()
                {
                    System.Threading.Thread.Sleep(1000);
                    if (ChangesCompleted != null)
                        ChangesCompleted(this);
                }
            ));
            thread.Start();
        }

        private void LoopingSelector_ManipulationCompleted(object sender, SelectionChangedEventArgs e)
        {
            selectedValue = Convert.ToInt32(e.AddedItems[0]);
            new System.Threading.Thread(new System.Threading.ThreadStart(WaitThenHide)).Start();
        }
    }
}