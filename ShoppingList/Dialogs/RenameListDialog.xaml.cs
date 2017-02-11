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

namespace ShoppingList
{
    public partial class RenameListDialog : PhoneApplicationPage
    {
        public delegate void CancelDialogHandler(RenameListDialog sender);
        public event CancelDialogHandler CancelDialog;

        public delegate void DialogOkHandler(RenameListDialog sender);
        public event DialogOkHandler DialogOk;

        private string selectedValue = "";
        public string Value { get { return selectedValue; } set { selectedValue = value; }}

        public RenameListDialog(string textValue)
        {
            InitializeComponent();

            selectedValue = textValue;
            txtValue.Text = selectedValue;
        }

        private void cancelDialog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CancelDialog != null)
                CancelDialog(this);
        }

        private void okDialog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (txtValue.Text.Trim().Equals(""))
                return;

            Value = txtValue.Text;
            if (DialogOk != null)
                DialogOk(this);
        }
    }
}