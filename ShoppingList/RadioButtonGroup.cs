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
using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace ShoppingList
{
    public static class RadioButtonGroup
    {
            #region Name (Attached DependencyProperty)

            public static readonly DependencyProperty NameProperty =
                DependencyProperty.RegisterAttached("Name", typeof(string), typeof(RadioButtonGroup), new PropertyMetadata(new PropertyChangedCallback(OnNameChanged)));

            public static void SetName(DependencyObject o, string value)
            {
                o.SetValue(NameProperty, value);
            }

            public static string GetName(DependencyObject o)
            {
                return (string)o.GetValue(NameProperty);
            }

            private static void OnNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
            {
                ToggleButton toggleButton = obj as ToggleButton;

                string newName = e.NewValue as string;
                string oldName = e.OldValue as string;


                if (toggleButton == null)
                {
                    string msg = String.Format(@"RadioButtonGroup Error; Target Element must be a ToggleButton, but found {0}", new object[] { obj.GetType().ToString() });
                    System.Diagnostics.Debug.WriteLine(msg);
                    throw new InvalidOperationException(msg);
                }

                if (!String.IsNullOrEmpty(oldName))
                    Unregister(oldName, toggleButton);

                if (!String.IsNullOrEmpty(newName))
                    Register(newName, toggleButton);
            }

            #endregion

            #region OnChecked (Event Handler)

            private static void OnChecked(object sender, RoutedEventArgs e)
            {
                UpdateRadioButtonGroup((ToggleButton)sender, GetName((ToggleButton)sender));
            }

            #endregion

            #region private methods

            private static void Register(string groupName, ToggleButton toggleButton)
            {
                if (_groupNameToElements == null)
                {
                    _groupNameToElements = new Dictionary<string, List<WeakReference>>(1);
                }

                List<WeakReference> list = null;
                if (!_groupNameToElements.TryGetValue(groupName, out list) || (list == null))
                {
                    list = new List<WeakReference>(1);
                    _groupNameToElements[groupName] = list;
                }
                else
                {
                    PurgeExpiredReferences(list, null);
                }

                toggleButton.Checked += OnChecked;
                list.Add(new WeakReference(toggleButton));
            }

            private static void Unregister(string groupName, ToggleButton toggleButton)
            {
                if (_groupNameToElements != null)
                {
                    List<WeakReference> elements = _groupNameToElements[groupName];
                    if (elements != null)
                    {
                        PurgeExpiredReferences(elements, toggleButton);
                        if (elements.Count == 0)
                        {
                            _groupNameToElements.Remove(groupName);
                        }
                    }
                }
            }

            private static void PurgeExpiredReferences(List<WeakReference> elements, ToggleButton elementToRemove)
            {
                int index = 0;
                while (index < elements.Count)
                {
                    object target = elements[index].Target;
                    if (target == null)
                    {
                        elements.RemoveAt(index);
                    }
                    else if (target == elementToRemove)
                    {
                        elementToRemove.Checked -= OnChecked;
                        elements.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }

            private static void UpdateRadioButtonGroup(ToggleButton toggleButton, string groupName)
            {
                List<WeakReference> list = _groupNameToElements[groupName];
                int index = 0;
                while (index < list.Count)
                {
                    //ToggleButton toggleButton
                    ToggleButton target = list[index].Target as ToggleButton;
                    if (target == null)
                    {
                        list.RemoveAt(index);
                    }
                    else
                    {
                        if ((target != toggleButton) && (target.IsChecked == true))
                        {
                            target.IsChecked = false;
                        }
                        index++;
                    }
                }
            }

            #endregion

            #region private fields

            private static Dictionary<string, List<WeakReference>> _groupNameToElements;

            #endregion
        
    }
}
