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
        MediaManager mediaManager;
        Media selectedItem = null;
        private bool ShowAlbums { get; set; }
        private bool ShowVideos { get; set; }

        public MainWindow()
        {
            string path = Properties.Settings.Default.LibraryPath;
            if (String.IsNullOrEmpty(path)) // First use
            {
                mediaManager = new MediaManager(Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%\\Pictures"));
                SetLocationProperty(mediaManager.RootPath);
            }
            else
            {
                mediaManager = new MediaManager(path);
            }
            InitializeComponent();
            PathTextBox.Text = mediaManager.RootPath;
        }

        private void InfoGrid_Initialized(object sender, EventArgs e)
        {
            InfoGrid.ItemsSource = mediaManager.MediaList;
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
            if (selectedItem != null && (!DescriptionBox.Text.Equals(selectedItem.Description)
                 || !DateBox.Text.Equals(selectedItem.Date.ToString("dd/MM/yyyy"))))
            {
                SaveButton_Click(sender, null);
            }

            OpenButton.IsEnabled = InfoGrid.Items.Count > 0;

            if (InfoGrid.SelectedItems.Count == 1)
            {
                EnableDescriptionControls(true);
                selectedItem = (Media)InfoGrid.SelectedItem;
                TitleBox.Text = selectedItem.Title;
                DateBox.Text = selectedItem.Date.ToString("dd/MM/yyyy");
                DescriptionBox.Text = selectedItem.Description;
            }
            else
            {
                EnableDescriptionControls(false);
                selectedItem = null;
                TitleBox.Text = "";
                DateBox.Text = "";
                DescriptionBox.Text = "";
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Media media in InfoGrid.SelectedItems)
            {
                mediaManager.OpenMedia(media);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            selectedItem.Title = TitleBox.Text;
            selectedItem.Date = DateTime.Parse(DateBox.Text);
            selectedItem.Description = DescriptionBox.Text;
            bool completed = mediaManager.PersistMedia(selectedItem);
            InfoGrid.Items.Refresh();
            if (!completed) MessageBox.Show("Couldn't save changes");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TitleBox.Text = selectedItem.Title;
            DateBox.Text = selectedItem.Date.ToString("dd/MM/yyyy");
            DescriptionBox.Text = selectedItem.Description;
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            InfoGrid.ItemsSource = mediaManager.FilterMediaList(SearchBox.Text, NoInfoButton.IsChecked.Value, ShowAlbums, ShowVideos);
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
                mediaManager.RootPath = selectedPath;
                InfoGrid_Initialized(sender, null);
                PathTextBox.Text = selectedPath;
                SetLocationProperty(selectedPath);
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

        private void PhotosRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowAlbums = true;
            ShowVideos = false;
            SearchBox_KeyUp(sender, null);
        }

        private void VideosRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowAlbums = false;
            ShowVideos = true;
            SearchBox_KeyUp(sender, null);
        }

        private void BothButton_Checked(object sender, RoutedEventArgs e)
        {
            ShowAlbums = true;
            ShowVideos = true;
            SearchBox_KeyUp(sender, null);
        }
    }
}
