using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ShoppingList
{
    public class ShoppingListItemViewModel : ViewModel
    {
        private string itemName;
        public string ItemName
        {
            get
            {
                return itemName;
            }
            set
            {
                if (value != itemName)
                {
                    itemName = value;
                    NotifyPropertyChanged("ItemName");
                }
            }
        }

        private string itemDescription;
        public string ItemDescription
        {
            get
            {
                return itemDescription;
            }
            set
            {
                if (value != itemDescription)
                {
                    itemDescription = value;
                    NotifyPropertyChanged("ItemDescription");
                }
            }
        }

        private bool isChecked = false;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (value != isChecked)
                {
                    isChecked = value;
                    NotifyPropertyChanged("IsChecked");
                }
            }
        }

        private int aisle = 1;
        public int Aisle 
        { 
            get { return aisle; } 
            set 
            {
                if (value != aisle)
                {
                    aisle = value;
                    NotifyPropertyChanged("Aisle");
                }
            } 
        }
    }
}