using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.IO;


namespace ShoppingList
{
    public class MainViewModel : ViewModel
    {
        const string ITEM_DESC_DELIMITER = "¯";

        // This is the name of the current list
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

        // These are the items in the current list
        private ShoppingListViewModel shoppingList = new ShoppingListViewModel();
        public ShoppingListViewModel ShoppingList 
        {
            get { return shoppingList; } 
            set 
            {
                shoppingList = value;
                NotifyPropertyChanged("ShoppingList");
            }
        }

        // This ia a list of previous lists
        private ObservableCollection<ShoppingListViewModel> shoppingListHistory = 
            new ObservableCollection<ShoppingListViewModel>();
        public ObservableCollection<ShoppingListViewModel> ShoppingListHistory
        {
            get
            {
                return shoppingListHistory;
            }
            set
            {
                shoppingListHistory = value;
                NotifyPropertyChanged("ShoppingListHistory");
            }
        }

        public MainViewModel()
        {
            AddTestData();
            LoadLists();
        }

        private void AddTestData()
        {
            // Add a current list
            //ShoppingList = new ShoppingListViewModel();
            //ShoppingList.ShoppingListItems = new ObservableCollection<ShoppingListItemViewModel>();
            //ShoppingList.ShoppingListItems.Add(new ShoppingListItemViewModel() { ItemName = "Nutrigrain bars", ItemDescription = "Bars" });
            //ShoppingList.ShoppingListItems.Add(new ShoppingListItemViewModel() { ItemName = "Gatorade", ItemDescription = "Drink" });
            //ShoppingList.ShoppingListItems.Add(new ShoppingListItemViewModel() { ItemName = "Turkey", ItemDescription = "Lunch Meat" });


            // Add some history data
            //ShoppingListHistory = new ObservableCollection<ShoppingListViewModel>();
            //ShoppingListViewModel newList = new ShoppingListViewModel();
            //newList.Name = "Historical List 1";
            //newList.Date = DateTime.Now;
            //newList.ShoppingListItems = new ObservableCollection<ShoppingListItemViewModel>();
            //newList.ShoppingListItems.Add(new ShoppingListItemViewModel() { ItemName = "Gatorade", ItemDescription = "Drink" });
            //newList.ShoppingListItems.Add(new ShoppingListItemViewModel() { ItemName = "Turkey", ItemDescription = "Lunch Meat" });
            //ShoppingListHistory.Add(newList);

            //newList = new ShoppingListViewModel();
            //newList.Name = "Historical List 2";
            //newList.Date = DateTime.Now - new TimeSpan(24, 0, 0);
            //newList.ShoppingListItems = new ObservableCollection<ShoppingListItemViewModel>();
            //newList.ShoppingListItems.Add(new ShoppingListItemViewModel() { ItemName = "Gatorade", ItemDescription = "Drink" });
            //newList.ShoppingListItems.Add(new ShoppingListItemViewModel() { ItemName = "Turkey", ItemDescription = "Lunch Meat" });
            //ShoppingListHistory.Add(newList);

        }


        #region Manipulate current list

        public void CreateNewList()
        {
            ShoppingList = new ShoppingListViewModel();
            ShoppingList.ShoppingListItems = new ObservableCollection<ShoppingListItemViewModel>();

            // Make sure the name of this list isn't the same as any other existing list
            int i = 1;
            bool foundName = false;
            while (!foundName)
            {
                foundName = true;
                foreach (ShoppingListViewModel list in ShoppingListHistory)
                {
                    if (list.Name == "List " + i.ToString())
                    {
                        i++;
                        foundName = false;
                        break;
                    }
                }
            }

            ShoppingList.Name = "List " + i.ToString();
        }

        public void AddItem(string itemName, string itemDescription)
        {
            if (ShoppingList == null)
                ShoppingList = new ShoppingListViewModel();

            if (ShoppingList.ShoppingListItems == null)
                ShoppingList.ShoppingListItems = new ObservableCollection<ShoppingListItemViewModel>();

            ShoppingListItemViewModel item = new ShoppingListItemViewModel();
            item.ItemName = itemName;
            item.ItemDescription = itemDescription;
            ShoppingList.ShoppingListItems.Add(item);

            SaveLists();
        }

        public void RemoveItem(ShoppingListItemViewModel itemToRemove)
        {
            if (ShoppingList.ShoppingListItems.Contains(itemToRemove))
                ShoppingList.ShoppingListItems.Remove(itemToRemove);

            SaveLists();
        }

        #endregion


        #region Save and Load

        public void SaveLists()
        {
            // Example List:
            // CurrentList
            // ListName
            // ListItem
            // ...
            // ListItem
            //
            // HistoricList
            // ListName
            // ListItem
            // ...
            // ListItem
            //
            // Historic List
            // ListName
            // ListItem
            // ...
            // ListItem
            
            using(IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using(IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream("listData.dat", System.IO.FileMode.Create, isf))
                {
                    using(StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
                    {
                        sw.WriteLine("CurrentList");
                        sw.WriteLine(ShoppingList.Name);
                        foreach (ShoppingListItemViewModel item in ShoppingList.ShoppingListItems)
                        {
                            sw.WriteLine(item.ItemName + ITEM_DESC_DELIMITER + item.ItemDescription + ITEM_DESC_DELIMITER + item.Aisle + ITEM_DESC_DELIMITER + item.IsChecked);
                        }
                        sw.WriteLine("");

                        foreach (ShoppingListViewModel historicList in ShoppingListHistory)
                        {
                            sw.WriteLine("HistoricList");
                            sw.WriteLine(historicList.Name);
                            foreach (ShoppingListItemViewModel item in historicList.ShoppingListItems)
                            {
                                sw.WriteLine(item.ItemName + ITEM_DESC_DELIMITER + item.ItemDescription + ITEM_DESC_DELIMITER + item.Aisle + ITEM_DESC_DELIMITER + item.IsChecked);
                            }
                            sw.WriteLine("");
                        }
                    }
                }
            }
        }

        public void LoadLists()
        {
            ShoppingList = new ShoppingListViewModel();
            ShoppingList.ShoppingListItems = new ObservableCollection<ShoppingListItemViewModel>();

            try
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream =
                        new IsolatedStorageFileStream("listData.dat", FileMode.Open, isf))
                    {
                        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                        {
                            string currentLine = "";
                            int i = -1;
                            while (!sr.EndOfStream)
                            {
                                i++;

                                currentLine = sr.ReadLine();
                                if (currentLine.Trim().Equals("")) break;

                                if (i == 0)
                                {
                                    continue;
                                }
                                else if (i == 1)
                                {
                                    ShoppingList.Name = currentLine;
                                }
                                else
                                {
                                    // This is the current list
                                    string[] splitLine = currentLine.Split(ITEM_DESC_DELIMITER.ToCharArray());
                                    string itemName = splitLine[0];
                                    string itemDescription = splitLine[1];
                                    int aisle = Convert.ToInt32(splitLine[2]);
                                    bool isChecked = Convert.ToBoolean(splitLine[3]);
                                    if (splitLine.Length > 1)
                                        itemDescription = splitLine[1];
                                    ShoppingList.ShoppingListItems.Add(
                                        new ShoppingListItemViewModel() { ItemName = itemName, ItemDescription = itemDescription, Aisle = aisle, IsChecked = isChecked, }
                                        );
                                }
                            }

                            int currentList = -1;
                            while (!sr.EndOfStream)
                            {
                                currentLine = sr.ReadLine();
                                if (currentLine.Trim().Equals("")) continue;

                                // Now we are into historical lists
                                if (currentLine.Equals("HistoricList"))
                                {
                                    // We haven't made this list yet
                                    ShoppingListViewModel historicList = new ShoppingListViewModel();
                                    historicList.ShoppingListItems = new ObservableCollection<ShoppingListItemViewModel>();
                                    ShoppingListHistory.Add(historicList);

                                    currentLine = sr.ReadLine();
                                    historicList.Name = currentLine;
                                    currentList++;
                                }
                                else
                                {
                                    string[] splitLine = currentLine.Split(ITEM_DESC_DELIMITER.ToCharArray());
                                    string itemName = splitLine[0];
                                    string itemDescription = splitLine[1];
                                    int aisle = Convert.ToInt32(splitLine[2]);
                                    bool isChecked = Convert.ToBoolean(splitLine[3]);
                                    if (splitLine.Length > 1)
                                        itemDescription = splitLine[1];
                                    shoppingListHistory[currentList].ShoppingListItems.Add(
                                        new ShoppingListItemViewModel() { ItemName = itemName, ItemDescription = itemDescription, Aisle = aisle, IsChecked = isChecked, }
                                    );
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to load file... This is probably first run on new boot");
            }
        }

        #endregion

    }
}