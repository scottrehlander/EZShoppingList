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
    public partial class ConfirmationDialog : PhoneApplicationPage
    {

        public delegate void CancelDialogHandler(ConfirmationDialog sender);
        public event CancelDialogHandler CancelDialog;

        public delegate void DialogOkHandler(ConfirmationDialog sender);
        public event DialogOkHandler DialogOk;

        public ConfirmationDialog(string confirmText)
        {
            InitializeComponent();

            tbConfirmText.Text = confirmText;
        }

        private void okDialog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DialogOk != null)
                DialogOk(this);
        }

        private void cancelDialog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CancelDialog != null)
                CancelDialog(this);
        }
    }
}