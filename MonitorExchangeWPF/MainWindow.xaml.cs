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
        private async Task UpdateDataGridImport(int page = 1, int pageSize = 25)
        {
            var filters = FilterDataGridHelper.GetFiltersFromDataGrid(ImportsFilesDataGrid);
            var request = new RequestLoadData { Page = page, PageSize = pageSize, Filters = filters};       

            var loadData = await _remoteDataService.LoadDataAsync<FileExchange>("FileExchange", request);

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

        // DataGrid Products
        private async Task UpdateDataGridProdukts(int page = 1, int pageSize = 20)
        {
            var filters = FilterDataGridHelper.GetFiltersFromDataGrid(ProductsDataGrid);
            var request = new RequestLoadData { Page = page, PageSize = pageSize, Filters = filters };

            var loadData = await _remoteDataService.LoadDataAsync<FEImportsProduct>("FileExchangeIE/FEImports", request);

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