using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using MonitorExchangeWPF.Models;
using MonitorExchangeWPF.Services;
using MonitorExchangeWPF.Infrastructure.Helpers;
using System.Diagnostics;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MonitorExchangeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly RemoteDataService _remoteDataService;

        private bool IsMaximized = false;
             
        private List<string> messages = new List<string>();

        public MainWindow()
        {
            _remoteDataService = new RemoteDataService();

            InitializeComponent();

            _ = UpdateDataGridImport();
            _ = UpdateDataGridProdukts();                      

        }

              
        /////////////////////////////////////////////////////////////
        /// METHODS FOR DATAGRID 
        ///  
         
        // DataGrid Imports
        private async Task UpdateDataGridImport(int page = 1, int pageSize = 20)
        {
            
            var filters = FilterDataGridHelper.GetFilterValuesFromDataGrid(ImportsFilesDataGrid);
            filters["Type"] = "Import";

            var request = new RequestLoadData { Page = page, PageSize = pageSize, Filters = filters};       

            var loadData = await _remoteDataService.LoadDataAsync<FileExchange>("FileExchangeWPF/GetFilesExchange", request);

            if (loadData != null)
            {
                ImportsFilesDataGrid.ItemsSource = loadData.listItems;

                if (loadData.meta != null)
                    GeneratePaginationImports(PaginationPanelImports, loadData.meta.page,
                        (int)Math.Ceiling(loadData.meta.totalItems / (double)loadData.meta.pageSize));
            }            
        }

        public void GeneratePaginationImports(StackPanel pagPanel, int currentPage, int totalPages)
        {
            pagPanel.Children.Clear(); // Cleaning old buttons

            // Button "Previous"
            Button prevButton = new Button
            {
                Content = "<",
                Style = FindResource("pagingButton") as Style,
                IsEnabled = currentPage > 1
            };
            prevButton.Click += (s, e) => GoToPageImports(currentPage - 1);
            pagPanel.Children.Add(prevButton);

            // Dynamically creating buttons for pages
            for (int i = 1; i <= totalPages; i++)
            {
                Button pageButton = new Button
                {
                    Content = i.ToString(),
                    Style = FindResource("pagingButton") as Style,
                    Background = (i == currentPage) ? new SolidColorBrush(Color.FromRgb(121, 80, 242)) : Brushes.White,
                    Foreground = (i == currentPage) ? Brushes.White : Brushes.Black
                };

                int page = i;
                pageButton.Click += (s, e) => GoToPageImports(page);
                pagPanel.Children.Add(pageButton);
            }

            // Button "Next"
            Button nextButton = new Button
            {
                Content = ">",
                Style = FindResource("pagingButton") as Style,
                IsEnabled = currentPage < totalPages
            };
            nextButton.Click += (s, e) => GoToPageImports(currentPage + 1);
            pagPanel.Children.Add(nextButton);
        }

        private async void GoToPageImports(int page)
        {
            if (page < 1)
                return;

            await UpdateDataGridImport(page);
        }

        private void FilterImportFileExchangeChanged(object sender, RoutedEventArgs e)
        {
            _ = UpdateDataGridImport();
        }

        // DataGrid Products
        private async Task UpdateDataGridProdukts(int page = 1, int pageSize = 20)
        {
            var filters = FilterDataGridHelper.GetFilterValuesFromDataGrid(ProductsDataGrid);
            filters["Type"] = "Import";

            var request = new RequestLoadData { Page = page, PageSize = pageSize, Filters = filters };

            var loadData = await _remoteDataService.LoadDataAsync<FEImportsProduct>("FileExchangeIEWPF/GetFEImports", request);

            if (loadData != null)
            {
                ProductsDataGrid.ItemsSource = loadData.listItems;

                if (loadData.meta != null)
                    GeneratePaginationProducts(PaginationPanelProducts, loadData.meta.page,
                        (int)Math.Ceiling(loadData.meta.totalItems / (double)loadData.meta.pageSize));
            }
        }

        public void GeneratePaginationProducts(StackPanel pagPanel, int currentPage, int totalPages)
        {
            pagPanel.Children.Clear();

            // Prev button
            Button prevButton = new Button
            {
                Content = "<",
                Style = FindResource("pagingButton") as Style,
                IsEnabled = currentPage > 1
            };
            prevButton.Click += (s, e) => GoToPageProducts(currentPage - 1);
            pagPanel.Children.Add(prevButton);

            HashSet<int> pagesToShow = new HashSet<int>();

            // Always show first 3 and last 3
            for (int i = 1; i <= 3; i++) pagesToShow.Add(i);
            for (int i = totalPages - 2; i <= totalPages; i++) pagesToShow.Add(i);

            // Show current page ±1
            for (int i = currentPage - 1; i <= currentPage + 1; i++)
            {
                if (i >= 1 && i <= totalPages)
                    pagesToShow.Add(i);
            }

            int lastPage = 0;

            for (int i = 1; i <= totalPages; i++)
            {
                if (pagesToShow.Contains(i))
                {
                    if (lastPage != 0 && i - lastPage > 1)
                    {
                        // Insert dots
                        pagPanel.Children.Add(new TextBlock
                        {
                            Text = "...",
                            Margin = new Thickness(5, 0, 5, 0),
                            VerticalAlignment = VerticalAlignment.Center,
                            FontWeight = FontWeights.Bold
                        });
                    }

                    AddPageButton(pagPanel, i, currentPage);
                    lastPage = i;
                }
            }

            // Next button
            Button nextButton = new Button
            {
                Content = ">",
                Style = FindResource("pagingButton") as Style,
                IsEnabled = currentPage < totalPages
            };
            nextButton.Click += (s, e) => GoToPageProducts(currentPage + 1);
            pagPanel.Children.Add(nextButton);
        }
        
        private void AddPageButton(StackPanel panel, int page, int currentPage)
        {
            Button pageButton = new Button
            {
                Content = page.ToString(),
                Style = FindResource("pagingButton") as Style,
                Background = (page == currentPage) ? new SolidColorBrush(Color.FromRgb(121, 80, 242)) : Brushes.White,
                Foreground = (page == currentPage) ? Brushes.White : Brushes.Black
            };

            pageButton.Click += (s, e) => GoToPageProducts(page);
            panel.Children.Add(pageButton);
        }

        private async void GoToPageProducts(int page)
        {
            if (page < 1)
                return;

            await UpdateDataGridProdukts(page);
        }

        private void FilterImportProductChanged(object sender, RoutedEventArgs e)
        {
            _ = UpdateDataGridProdukts();
        }


        private void OpenDateRangeDialog_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            DependencyObject parent = VisualTreeHelper.GetParent(button);
            while (parent != null && parent is not Grid)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var parentGrid = parent as Grid;
            if (parentGrid == null) return;

            TextBox filterDateRangeTextBox = null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentGrid); i++)
            {
                var child = VisualTreeHelper.GetChild(parentGrid, i);
                if (child is TextBox tb && (tb.Tag?.ToString() == "filterDateRangeTextBox"))
                {
                    filterDateRangeTextBox = tb;
                    break;
                }
            }

            if (filterDateRangeTextBox == null) return;

            // Dialog box
            Window dialog = new Window
            {
                Title = "Select a date and time range",
                Width = 200,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize
            };

            var fromDate = DateTime.Now.AddHours(-1);
            var toDate = DateTime.Now;
            
            string[] parts = filterDateRangeTextBox.Text.Split('-');
            if (parts.Length == 2 &&
                DateTime.TryParseExact(parts[0], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime from) &&
                DateTime.TryParseExact(parts[1], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime to))
            {
                fromDate = from;
                toDate = to;
            }

            var fromDatePicker = new DatePicker { SelectedDate = fromDate, Margin = new Thickness(5), Width = 100 };
            var fromTimeBox = new TextBox { Text = fromDate.ToString("HH:mm:ss"), Margin = new Thickness(5), Width = 50 };

            var toDatePicker = new DatePicker { SelectedDate = toDate, Margin = new Thickness(5), Width = 100 };
            var toTimeBox = new TextBox { Text = toDate.ToString("HH:mm:ss"), Margin = new Thickness(5), Width = 50 };

            var okButton = new Button { Content = "OK", Margin = new Thickness(5), Width = 50, HorizontalAlignment = HorizontalAlignment.Right };
            var cancelButton = new Button { Content = "Cancel", Margin = new Thickness(5), Width = 50, HorizontalAlignment = HorizontalAlignment.Right };
            var cleanButton = new Button { Content = "Clean", Margin = new Thickness(5), Width = 50, HorizontalAlignment = HorizontalAlignment.Right };

            okButton.Click += (s, args) =>
            {
                if (fromDatePicker.SelectedDate.HasValue && toDatePicker.SelectedDate.HasValue)
                {
                    if (TimeSpan.TryParse(fromTimeBox.Text, out TimeSpan fromTime) &&
                        TimeSpan.TryParse(toTimeBox.Text, out TimeSpan toTime))
                    {
                        DateTime from = fromDatePicker.SelectedDate.Value.Date + fromTime;
                        DateTime to = toDatePicker.SelectedDate.Value.Date + toTime;

                        filterDateRangeTextBox.Text = $"{from:dd.MM.yyyy HH:mm:ss}-{to:dd.MM.yyyy HH:mm:ss}";
                        dialog.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid time format. Please enter in the format HH:mm", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            };

            cancelButton.Click += (s, args) => {  dialog.Close(); };
            cleanButton.Click += (s, args) => { filterDateRangeTextBox.Text = "";  dialog.Close(); };

            var panel = new StackPanel();

            panel.Children.Add(new TextBlock { Text = "Start date and time:", Margin = new Thickness(5) });
            var fromPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            fromPanel.Children.Add(fromDatePicker);
            fromPanel.Children.Add(fromTimeBox);

            panel.Children.Add(fromPanel);

            panel.Children.Add(new TextBlock { Text = "End date and time:", Margin = new Thickness(5) });
            var toPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            toPanel.Children.Add(toDatePicker);
            toPanel.Children.Add(toTimeBox);

            panel.Children.Add(toPanel);

            var  buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Right };

            buttonPanel.Children.Add(cancelButton);
            buttonPanel.Children.Add(cleanButton);
            buttonPanel.Children.Add(okButton);

            panel.Children.Add(buttonPanel);

            dialog.Content = panel;
            dialog.ShowDialog();
        }


        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }

        }
               
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    //this.Height = 720;
                    //this.Width = 1080;

                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;

                    IsMaximized = true;
                }




            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                UploadFiles(openFileDialog.FileNames);
            }
        }

        private async Task UploadFiles(string[] filePaths)
        {
            List<string> messages = new List<string>();

            List<Task> tasks = new List<Task>();

            foreach (string file in filePaths)
            {
                tasks.Add(UploadFile(file, messages));
            }

            await Task.WhenAll(tasks);

            string resul = string.Join("\n", messages);
            MessageBox.Show("Вибрані файли:\n" + resul);
        }

        private async Task UploadFile(string xmlFilePath, List<string> messages)
        {
            string uri = string.Empty;
            if (!GetURI(out uri, xmlFilePath))
            {
                messages.Add($"File: {xmlFilePath}");
                messages.Add("Failed to get parameters from filename!");
                return;
            }

            string xmlContent = string.Empty;
            if (!GetContent(out xmlContent, xmlFilePath))
            {
                messages.Add($"File: {xmlFilePath}");
                messages.Add("Failed to get content from file!");
                return;
            }

            try
            {                
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5253");
                client.Timeout = TimeSpan.FromSeconds(300);

                HttpResponseMessage response = await client.PostAsync(uri, new StringContent(xmlContent, Encoding.UTF8, "application/xml"));

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    messages.Add($"File: {xmlFilePath} - successful!");
                }
                else
                {
                    messages.Add($"File: {xmlFilePath} - not successful!");
                    messages.Add($"Error: {response.StatusCode}");
                }

            }catch (Exception ex)
            {
                messages.Add($"File: {xmlFilePath} - not successful!");
                messages.Add(ex.Message);
            }            
        }

        private bool GetURI(out string uri, string fileName)
        {
            uri = string.Empty;

            string[] arrayParameters = fileName.Replace(".xml", "").Split("_");

            if (arrayParameters.Length != 4)
                return false;
                         
            if (arrayParameters[3].Length != 36)
                return false;
                       
            var type = arrayParameters[0];
            var item = arrayParameters[1];
            var allIn = arrayParameters[2];
            var guid = arrayParameters[3];

            var _params = $"?strId={guid}&item={item}&allIn={allIn}";
                        
            if (type.ToLower().Contains("import"))
            {
                uri = $"/api/FileExchangeIE/XMLImport{_params}";
            }
            else if (type.ToLower().Contains("offers"))
            {
                uri = $"/api/FileExchangeIE/XMLOffers{_params}";
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool GetContent(out string content, string fileName)
        {
            content = string.Empty;

            try
            {
                StreamReader reader = new StreamReader(fileName);
                content = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                return false;
            }
            
            return true;
        }
             

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            pagImports.Visibility = Visibility.Collapsed;
            

        }

        private void btnImports_Click(object sender, RoutedEventArgs e)
        {
            pagImports.Visibility = Visibility.Visible;

        }

        private void btnFiles_Click(object sender, RoutedEventArgs e)
        {
            ImportsFiles.Visibility = Visibility.Visible;
            ImportsProducts.Visibility = Visibility.Collapsed;
        }

        private void btnProducts_Click(object sender, RoutedEventArgs e)
        {
            ImportsFiles.Visibility = Visibility.Collapsed;
            ImportsProducts.Visibility = Visibility.Visible;
        }

       
    }

    

        

}