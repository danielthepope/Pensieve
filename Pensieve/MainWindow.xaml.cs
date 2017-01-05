using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pensieve.Controller;
using Pensieve.Model;

namespace Pensieve
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AlbumManager manager;
        Album selectedAlbum = null;

        public MainWindow()
        {
            string path = Properties.Settings.Default.LibraryPath;
            if (String.IsNullOrEmpty(path)) // First use
            {
                manager = new AlbumManager(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%\\Pictures"));
                SetLocationProperty(manager.RootPath);
            }
            else
            {
                manager = new AlbumManager(path);
            }
            InitializeComponent();
            PathTextBox.Text = manager.RootPath;
        }

        private void InfoGrid_Initialized(object sender, EventArgs e)
        {
            InfoGrid.ItemsSource = manager.MediaList;
        }

        private void EnableDescriptionControls(bool enabled)
        {
            TitleBox.IsEnabled = enabled;
            DateBox.IsEnabled = enabled;
            DescriptionBox.IsEnabled = enabled;
            SaveButton.IsEnabled = enabled;
            CancelButton.IsEnabled = enabled;
        }

        private void InfoGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedAlbum != null && (!DescriptionBox.Text.Equals(selectedAlbum.Description)
                 || !DateBox.Text.Equals(selectedAlbum.Date.ToString("dd/MM/yyyy"))))
            {
                SaveButton_Click(sender, null);
            }

            OpenButton.IsEnabled = InfoGrid.Items.Count > 0;

            if (InfoGrid.SelectedItems.Count == 1)
            {
                EnableDescriptionControls(true);
                selectedAlbum = (Album)InfoGrid.SelectedItem;
                TitleBox.Text = selectedAlbum.Title;
                DateBox.Text = selectedAlbum.Date.ToString("dd/MM/yyyy");
                DescriptionBox.Text = selectedAlbum.Description;
            }
            else
            {
                EnableDescriptionControls(false);
                selectedAlbum = null;
                TitleBox.Text = "";
                DateBox.Text = "";
                DescriptionBox.Text = "";
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Album album in InfoGrid.SelectedItems)
            {
                manager.OpenMedia(album);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            selectedAlbum.Title = TitleBox.Text;
            selectedAlbum.Date = DateTime.Parse(DateBox.Text);
            selectedAlbum.Description = DescriptionBox.Text;
            bool completed = manager.PersistMedia(selectedAlbum);
            InfoGrid.Items.Refresh();
            if (!completed) MessageBox.Show("Couldn't save changes");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TitleBox.Text = selectedAlbum.Title;
            DateBox.Text = selectedAlbum.Date.ToString("dd/MM/yyyy");
            DescriptionBox.Text = selectedAlbum.Description;
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            InfoGrid.ItemsSource = manager.FilterMediaList(SearchBox.Text, NoInfoButton.IsChecked.Value);
        }

        private void ClearSearchBoxButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            SearchBox.Focus();
            InfoGrid.SelectedItem = null;
            if (NoInfoButton.IsChecked != null && NoInfoButton.IsChecked == true)
            {
                NoInfoButton_Checked(sender, e);
            }
            else
            {
                NoInfoButton_Unchecked(sender, e);
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            InfoGrid.SelectAll();
        }

        private void NoInfoButton_Checked(object sender, RoutedEventArgs e)
        {
            SearchBox_KeyUp(sender, null);
        }

        private void NoInfoButton_Unchecked(object sender, RoutedEventArgs e)
        {
            SearchBox_KeyUp(sender, null);
        }

        private void PathChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = picker.ShowDialog();
            if (result.Equals(System.Windows.Forms.DialogResult.OK))
            {
                string selectedPath = picker.SelectedPath;
                if (!selectedPath.EndsWith("\\")) selectedPath += "\\";
                manager = new AlbumManager(selectedPath);
                SetLocationProperty(selectedPath);
                InfoGrid_Initialized(sender, null);
                PathTextBox.Text = selectedPath;
            }
        }

        private void SetLocationProperty(string location)
        {
            Properties.Settings.Default.LibraryPath = location;
            Properties.Settings.Default.Save();
        }

        private void InfoGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenButton_Click(sender, null);
        }
    }
}
