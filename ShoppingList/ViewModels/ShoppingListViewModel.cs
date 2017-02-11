using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ShoppingList
{
    public class ShoppingListViewModel : ViewModel
    {
        private string name = "List 1";
        public string Name 
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private DateTime date = DateTime.MaxValue;
        public DateTime Date 
        {
            get { return date; }
            set
            {
                date = value;
                NotifyPropertyChanged("Date");
            }
        }

        private ObservableCollection<ShoppingListItemViewModel> shoppingListItems;
        public ObservableCollection<ShoppingListItemViewModel> ShoppingListItems 
        {
            get { return shoppingListItems; }
            set
            {
                shoppingListItems = value;
                NotifyPropertyChanged("ShoppingListItems");
            }
        }


        public void Sort()
        {
            Sort(false);
        }

        public void Sort(bool uncheckAll)
        {
            int maxAisle = 0;

            // Grab the highest Aisle
            foreach (ShoppingListItemViewModel item in shoppingListItems)
                if (item.Aisle > maxAisle)
                    maxAisle = item.Aisle;

            ObservableCollection<ShoppingListItemViewModel> sortedItems = new ObservableCollection<ShoppingListItemViewModel>();
            
            // Sorted Checked Items
            List<ShoppingListItemViewModel> sortedCheckedItems = new List<ShoppingListItemViewModel>();

            for (int i = 1; i <= maxAisle; i++)
            {
                foreach (ShoppingListItemViewModel item in shoppingListItems)
                {
                    if (item.Aisle == i)
                    {
                        if (!item.IsChecked || uncheckAll)
                        {
                            sortedItems.Add(item);

                            if (uncheckAll)
                                item.IsChecked = false;
                        }
                        else
                        {
                            sortedCheckedItems.Add(item);
                        }
                    }
                }
            }

            foreach (ShoppingListItemViewModel item in sortedCheckedItems)
            {
                sortedItems.Add(item);
            }

            ShoppingListItems = sortedItems;
        }

        public void UncheckAll()
        {
            Sort(true);
        }

    }
}
