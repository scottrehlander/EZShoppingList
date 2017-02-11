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
using System.ComponentModel;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls.Primitives;

namespace ShoppingList
{
    public partial class MainPage : PhoneApplicationPage
    {
        double originalListHeight = 800;

        MainViewModel mainViewModel;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.RunWorkerAsync();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            mainViewModel = (MainViewModel)DataContext;
        }

        #region Add Item to List

        private void txtAddListItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Reset the text boxes
            txtNewItemName.Text = "";
            txtNewItemDescription.Text = "";

            // Save the original height of the list
            originalListHeight = rowList.ActualHeight;

            // Show the new item box
            rowAdd.Height = new GridLength(800, GridUnitType.Pixel);
            rowList.Height = new GridLength(180, GridUnitType.Pixel);

            txtNewItemName.Focus();
        }

        private void btnOkAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (txtNewItemName.Text.Equals("")) return;

            // Add the item to the list
            mainViewModel.AddItem(txtNewItemName.Text, txtNewItemDescription.Text);

            // Hide the new item box
            rowAdd.Height = new GridLength(0, GridUnitType.Pixel);
            rowList.Height = new GridLength(800, GridUnitType.Pixel);
        }

        private void btnCancelAddItem_Click(object sender, RoutedEventArgs e)
        {
            rowAdd.Height = new GridLength(0, GridUnitType.Pixel);
            rowList.Height = new GridLength(800, GridUnitType.Pixel);
        }

        #endregion


        #region Remove Item from List

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if(!(sender is Button)) return;
            if(!((sender as Button).DataContext is ShoppingListItemViewModel)) return;

            mainViewModel.RemoveItem((sender as Button).DataContext as ShoppingListItemViewModel);
        }

        #endregion


        private void rehlanderLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WebBrowserTask wbt = new WebBrowserTask();
            wbt.URL = "http://rehlander.com";
            wbt.Show();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // If the back button is pressed when the additem panel is open, just close it
            if (rowAdd.Height.Value > 10)
            {
                rowAdd.Height = new GridLength(0, GridUnitType.Pixel);
                rowList.Height = new GridLength(800, GridUnitType.Pixel);

                e.Cancel = true;
            }
        }


        #region Show splash


        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gridSpash.Visibility = System.Windows.Visibility.Collapsed;
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    shoppingCartAnimation.Begin();
                }
            );

        }
        
        #endregion


        #region Change Aisle

        private void AisleSelector_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainContentGrid.IsHitTestVisible = false;
            ChangeAisleDialog itemsPage = new ChangeAisleDialog(((sender as TextBlock).DataContext as ShoppingListItemViewModel).ItemName);
            itemsPage.ChangesCompleted += new ChangeAisleDialog.ChangesCompletedHandler(itemsPage_ChangesCompleted);
            itemsPage.Tag = sender;
            LayoutRoot.Children.Add(itemsPage);
        }

        private void itemsPage_ChangesCompleted(ChangeAisleDialog sender)
        {
            System.Windows.Threading.DispatcherOperation
                dispatcherOp = Dispatcher.BeginInvoke(
                    new Action(
                    delegate()
                    {
                        try
                        {
                            // Set the value of the Aisle
                            ((sender.Tag as TextBlock).DataContext as ShoppingListItemViewModel).Aisle = sender.SelectedValue;
                            LayoutRoot.Children.Remove(sender);
                        }
                        finally
                        {
                            mainContentGrid.IsHitTestVisible = true;
                        }

                        mainViewModel.ShoppingList.Sort();
                    }
            ));

            mainViewModel.SaveLists();
        }

        #endregion


        #region Check Item

        private void CheckItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((sender as TextBlock).DataContext as ShoppingListItemViewModel).IsChecked =
                !((sender as TextBlock).DataContext as ShoppingListItemViewModel).IsChecked;
            if (((sender as TextBlock).DataContext as ShoppingListItemViewModel).IsChecked)
                (sender as TextBlock).Text = "x";
            else
                (sender as TextBlock).Text = "";
            mainViewModel.ShoppingList.Sort();
            mainViewModel.SaveLists();
        }

        #endregion


        #region Rename List

        private void RenameList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainContentGrid.IsHitTestVisible = false;
            RenameListDialog renameDialog = new RenameListDialog(mainViewModel.ShoppingList.Name);
            LayoutRoot.Children.Add(renameDialog);
            renameDialog.CancelDialog += new RenameListDialog.CancelDialogHandler(renameDialog_CancelDialog);
            renameDialog.DialogOk += new RenameListDialog.DialogOkHandler(renameDialog_DialogOk);
        }

        private void renameDialog_DialogOk(RenameListDialog sender)
        {
            System.Windows.Threading.DispatcherOperation
                dispatcherOp = Dispatcher.BeginInvoke(
                    new Action(
                    delegate()
                    {
                        try
                        {
                            mainViewModel.ShoppingList.Name = sender.Value;
                            txtListName.Text = sender.Value;
                            LayoutRoot.Children.Remove(sender);
                        }
                        finally
                        {
                            mainContentGrid.IsHitTestVisible = true;
                        }
                    }
            ));

            mainViewModel.SaveLists();
        }

        private void renameDialog_CancelDialog(RenameListDialog sender)
        {
            System.Windows.Threading.DispatcherOperation
                dispatcherOp = Dispatcher.BeginInvoke(
                    new Action(
                    delegate()
                    {
                        try
                        {
                            LayoutRoot.Children.Remove(sender);
                        }
                        finally
                        {
                            mainContentGrid.IsHitTestVisible = true;
                        }
                    }
            ));
        }

        #endregion


        #region Uncheck all button

        private void uncheckAll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainContentGrid.IsHitTestVisible = false;
            ConfirmationDialog confirmDialog = new ConfirmationDialog("Are you sure you want to uncheck all checked items?");
            LayoutRoot.Children.Add(confirmDialog);
            confirmDialog.CancelDialog += new ConfirmationDialog.CancelDialogHandler(confirmDialog_CancelDialog);
            confirmDialog.DialogOk += new ConfirmationDialog.DialogOkHandler(confirmDialog_DialogOk);
        }

        void confirmDialog_DialogOk(ConfirmationDialog sender)
        {
            mainViewModel.ShoppingList.UncheckAll();
            System.Windows.Threading.DispatcherOperation
                dispatcherOp = Dispatcher.BeginInvoke(
                    new Action(
                    delegate()
                    {
                        try
                        {
                            LayoutRoot.Children.Remove(sender);
                        }
                        finally
                        {
                            mainContentGrid.IsHitTestVisible = true;
                        }
                    }
            ));

            mainViewModel.SaveLists();
        }

        void confirmDialog_CancelDialog(ConfirmationDialog sender)
        {
            System.Windows.Threading.DispatcherOperation
                dispatcherOp = Dispatcher.BeginInvoke(
                    new Action(
                    delegate()
                    {
                        try
                        {
                            LayoutRoot.Children.Remove(sender);
                        }
                        finally
                        {
                            mainContentGrid.IsHitTestVisible = true;
                        }
                    }
            ));
        }

        #endregion


        #region New List Button

        private void newList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mainContentGrid.IsHitTestVisible = false;
            ConfirmationDialog confirmDialog = new ConfirmationDialog("Are you sure you want to create a new list? \n\nThe old list will be saved as \"" +
                mainViewModel.ShoppingList.Name + ".\"");
            LayoutRoot.Children.Add(confirmDialog);
            confirmDialog.CancelDialog += new ConfirmationDialog.CancelDialogHandler(confirmNewListDialog_CancelDialog);
            confirmDialog.DialogOk += new ConfirmationDialog.DialogOkHandler(confirmNewListDialog_DialogOk);
        }

        void confirmNewListDialog_CancelDialog(ConfirmationDialog sender)
        {
            System.Windows.Threading.DispatcherOperation
            dispatcherOp = Dispatcher.BeginInvoke(
                new Action(
                delegate()
                {
                    try
                    {
                        LayoutRoot.Children.Remove(sender);
                    }
                    finally
                    {
                        mainContentGrid.IsHitTestVisible = true;
                    }
                }
            ));
        }

        void confirmNewListDialog_DialogOk(ConfirmationDialog sender)
        {
            // Save the current list into history
            if (mainViewModel.ShoppingList.ShoppingListItems.Count > 0)
            {
                mainViewModel.ShoppingListHistory.Add(mainViewModel.ShoppingList);
            }
            
            // Create a new list
            mainViewModel.CreateNewList();

            System.Windows.Threading.DispatcherOperation
            dispatcherOp = Dispatcher.BeginInvoke(
                new Action(
                delegate()
                {
                    try
                    {
                        LayoutRoot.Children.Remove(sender);
                    }
                    finally
                    {
                        mainContentGrid.IsHitTestVisible = true;
                    }
                }
            ));

            mainViewModel.SaveLists();
            txtListName.Text = mainViewModel.ShoppingList.Name;
        }

        #endregion

    }
}