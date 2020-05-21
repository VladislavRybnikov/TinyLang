using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TinyLang.IDE
{
    /// <summary>
    /// Interaction logic for SettingsForm.xaml
    /// </summary>
    public partial class SettingsForm : Window
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private bool _searchTextBoxEditing;

        private void TextBox_Focus(object sender, EventArgs e)
        {
            if (!_searchTextBoxEditing && SearchTextBox.Text == "Search...") SearchTextBox.Clear();
            _searchTextBoxEditing = true;
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            _searchTextBoxEditing = false;
            SearchTextBox.Text = "Search...";
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedNodeHeader = (e.NewValue as TreeViewItem).Header.ToString();

            ExceptionSettings.Visibility = selectedNodeHeader == "Exceptions"
                           ? Visibility.Visible : Visibility.Hidden;

            GeneralSettings.Visibility = selectedNodeHeader == "General Settings"
                           ? Visibility.Visible : Visibility.Hidden;

            SettingsLbl.Content = selectedNodeHeader;
        }
    }
}
