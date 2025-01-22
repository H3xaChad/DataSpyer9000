using FairDataGetter.Client.Class;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Diagnostics;
using System.Net;
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_EmployeeDashboard.xaml
    /// </summary>
    public partial class UserControl_EmployeeDashboard : UserControl
    {
        private List<ExportData> localCustomerData;
        private List<ExportData> transmittedCustomerData = [];
        private ApiPostHandler apiPostHandler = new ApiPostHandler();

        public UserControl_EmployeeDashboard()
        {
            InitializeComponent();
            this.Loaded += UserControl_EmployeeDashboard_Loaded;
        }

        private async void UserControl_EmployeeDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            await CountCustomersFromApiAsync("http://localhost:5019/api/Customer");
            LoadJsonData();
        }

        private void LoadJsonData()
        {
            try 
            {
                // Get the path to the Documents folder
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Define the JSON file name
                string jsonFilePath = Path.Combine(documentsPath, "local_customer_data.json");

                // Check if the file exists
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show($"The file 'local_customer_data.json' was not found in the Documents folder.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Read the JSON file
                string jsonData = File.ReadAllText(jsonFilePath);

                // Deserialize JSON into a list of customers
                localCustomerData = JsonConvert.DeserializeObject<List<ExportData>>(jsonData);


                int localTotalCustomerCount = localCustomerData.Count();
                LocalCustomersTextbox.Text = localTotalCustomerCount.ToString();

                int localCorporateCustomerCount = localCustomerData.Count(c => c.Company != null);
                LocalCorporateCustomersTextbox.Text = localCorporateCustomerCount.ToString();

                var displayData = localCustomerData.Select(c => new
                {
                    FirstName = c.Customer.FirstName,
                    LastName = c.Customer.LastName,
                    Email = c.Customer.Email,
                    Street = c.Customer.Address.Street,
                    HouseNumber = c.Customer.Address.HouseNumber,
                    City = c.Customer.Address.City,
                    PostalCode = c.Customer.Address.PostalCode,
                    Country = c.Customer.Address.Country,
                    InterestedProductGroups = c.Customer.InterestedProductGroups,
                    CompanyName = c.Company?.Name ?? "",
                    CompanyStreet = c.Company?.Address?.Street ?? "",
                    CompanyHouseNumber = c.Company?.Address?.HouseNumber ?? "",
                    CompanyCity = c.Company?.Address?.City ?? "",
                    CompanyPostalCode = c.Company?.Address?.PostalCode ?? "",
                    CompanyCountry = c.Company?.Address?.Country ?? ""
                }).ToList();

                // Bind to the DataGrid
                LocalCustomerDataGrid.ItemsSource = displayData;
            }
            catch (System.Text.Json.JsonException ex)
            {
                MessageBox.Show($"Error parsing JSON data: {ex.Message}", "JSON Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error accessing the file: {ex.Message}", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task CountCustomersFromApiAsync(string apiUrl)
        {
            try
            {
                // Create an HttpClient instance
                using (var httpClient = new HttpClient())
                {
                    // Send GET request to the API
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    // Ensure a successful response
                    response.EnsureSuccessStatusCode();

                    // Read the response content as a string
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Print the response data in the debug output
                    Debug.WriteLine("Response Data: ");
                    Debug.WriteLine(responseData); // This prints the response in the debug console

                    // Deserialize the JSON response into a list of dynamic objects
                    var customers = JsonConvert.DeserializeObject<List<dynamic>>(responseData);

                    // Count total customers (all customers are counted)
                    int totalCustomerCount = customers.Count();

                    // Count corporate customers (those with non-null and non-zero CompanyId)
                    int corporateCustomerCount = customers.Count(c =>
                        c.companyId != null && (int)c.companyId != 0); // Ensure we're checking against the nullable integer

                    // Display the counts in the respective TextBoxes
                    TotalCustomersTextbox.Text = totalCustomerCount.ToString(); // Total customers count
                    TotalCorporateCustomersTextbox.Text = corporateCustomerCount.ToString(); // Corporate customers count
                }
            }
            catch (Exception ex)
            {
                // Handle any errors (e.g., network issues, deserialization errors)
                MessageBox.Show($"Error fetching data: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerData());
        }

        private void ViewRemoteDatabaseButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_EmployeeRemoteDatabase());
        }

        private async void UpdateButtonClicked(object sender, RoutedEventArgs e)
        {
            // Get the path to the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Define the JSON file name
            string transmittedFilePath = Path.Combine(documentsPath, "transmitted_customer_data.json");

            // Ensure the transmitted JSON file exists
            if (!File.Exists(transmittedFilePath))
            {
                File.WriteAllText(transmittedFilePath, "[]"); // Initialize with an empty JSON array
            }
            else
            {
                // Load the existing transmitted customers data
                string transmittedJson = File.ReadAllText(transmittedFilePath);
                transmittedCustomerData = System.Text.Json.JsonSerializer.Deserialize<List<ExportData>>(transmittedJson) ?? new List<ExportData>();
            }

            // Iterate over local customer data and process each customer
            foreach (ExportData customerData in localCustomerData.ToList())
            {
                try
                {
                    int? companyId = null;
                    
                    // Transmit company data if it's a corporate customer
                    if (customerData.Customer.IsCorporateCustomer)
                    {
                        companyId = await apiPostHandler.SendCompanyAsync(customerData.Company);
                    }

                    // Transmit customer data
                    await apiPostHandler.SendCustomerAsync(customerData.Customer, companyId);

                    // Debug Output Statements
                    System.Diagnostics.Debug.WriteLine("****** Output Before Update");
                    System.Diagnostics.Debug.WriteLine(localCustomerData);
                    System.Diagnostics.Debug.WriteLine("****** End of Output");

                    // Move the successfully transmitted customer to the transmitted list
                    transmittedCustomerData.Add(customerData);
                    localCustomerData.Remove(customerData);

                    // Debug Output Statements
                    System.Diagnostics.Debug.WriteLine("****** Output After Update");
                    System.Diagnostics.Debug.WriteLine(localCustomerData);
                    System.Diagnostics.Debug.WriteLine("****** End of Output");

                    // Save the updated local customer data immediately
                    string updatedLocalJson = System.Text.Json.JsonSerializer.Serialize(localCustomerData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(documentsPath, "local_customer_data.json"), updatedLocalJson);

                    LoadJsonData();
                }
                catch (Exception ex)
                {
                    // Log the error or show a message
                    MessageBox.Show($"Error transmitting customer {customerData.Customer.FirstName} {customerData.Customer.LastName}: {ex.Message}");
                    // Skip this customer but keep them in the local JSON
                    continue;
                }
            }

            // Save updated transmitted customer data to JSON after the loop
            string updatedTransmittedJson = System.Text.Json.JsonSerializer.Serialize(transmittedCustomerData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(transmittedFilePath, updatedTransmittedJson);

            // Display the current date and time in the TextBox
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm");
            LastRemoteUpdateTextblock.Text = formattedDateTime;
        }

        
        private void FilterDataGrid(object sender, TextChangedEventArgs e)
        {
            // Get the filter values from the TextBoxes
            string firstNameFilter = FirstNameFilterTextBox.Text.ToLower();
            string lastNameFilter = LastNameFilterTextBox.Text.ToLower();
            string emailFilter = EmailFilterTextBox.Text.ToLower();
            string streetFilter = AddressFilterTextBox.Text.ToLower();
            string houseNumberFilter = HouseNumberFilterTextBox.Text.ToLower();
            string postalCodeFilter = PostalCodeFilterTextBox.Text.ToLower();
            string cityFilter = CityFilterTextBox.Text.ToLower();
            string countryFilter = CountryFilterTextBox.Text.ToLower();

            string companyNameFilter = CompanyNameFilterTextBox.Text.ToLower();
            string companyStreetFilter = CompanyAddressFilterTextBox.Text.ToLower();
            string companyHouseNumberFilter = CompanyHouseNumberFilterTextBox.Text.ToLower();
            string companyPostalCodeFilter = CompanyPostalCodeFilterTextBox.Text.ToLower();
            string companyCityFilter = CompanyCityFilterTextBox.Text.ToLower();
            string companyCountryFilter = CompanyCountryFilterTextBox.Text.ToLower();

            // Apply filters to the localCustomerData
            var filteredData = localCustomerData.Select(c => new
            {
                FirstName = c.Customer.FirstName,
                LastName = c.Customer.LastName,
                Email = c.Customer.Email,
                Street = c.Customer.Address.Street,
                HouseNumber = c.Customer.Address.HouseNumber,
                City = c.Customer.Address.City,
                PostalCode = c.Customer.Address.PostalCode,
                Country = c.Customer.Address.Country,
                InterestedProductGroups = c.Customer.InterestedProductGroups,
                CompanyName = c.Company?.Name ?? "",
                CompanyStreet = c.Company?.Address?.Street ?? "",
                CompanyHouseNumber = c.Company?.Address?.HouseNumber ?? "",
                CompanyCity = c.Company?.Address?.City ?? "",
                CompanyPostalCode = c.Company?.Address?.PostalCode ?? "",
                CompanyCountry = c.Company?.Address?.Country ?? ""
            })
            .Where(item =>
                (string.IsNullOrEmpty(firstNameFilter) || item.FirstName.ToLower().Contains(firstNameFilter)) &&
                (string.IsNullOrEmpty(lastNameFilter) || item.LastName.ToLower().Contains(lastNameFilter)) &&
                (string.IsNullOrEmpty(emailFilter) || item.Email.ToLower().Contains(emailFilter)) &&
                (string.IsNullOrEmpty(streetFilter) || item.Street.ToLower().Contains(streetFilter)) &&
                (string.IsNullOrEmpty(houseNumberFilter) || item.HouseNumber.ToLower().Contains(houseNumberFilter)) &&
                (string.IsNullOrEmpty(postalCodeFilter) || item.PostalCode.ToLower().Contains(postalCodeFilter)) &&
                (string.IsNullOrEmpty(cityFilter) || item.City.ToLower().Contains(cityFilter)) &&
                (string.IsNullOrEmpty(countryFilter) || item.Country.ToLower().Contains(countryFilter)) &&
                (string.IsNullOrEmpty(companyNameFilter) || item.CompanyName.ToLower().Contains(companyNameFilter)) &&
                (string.IsNullOrEmpty(companyStreetFilter) || item.CompanyStreet.ToLower().Contains(companyStreetFilter)) &&
                (string.IsNullOrEmpty(companyHouseNumberFilter) || item.CompanyHouseNumber.ToLower().Contains(companyHouseNumberFilter)) &&
                (string.IsNullOrEmpty(companyPostalCodeFilter) || item.CompanyPostalCode.ToLower().Contains(companyPostalCodeFilter)) &&
                (string.IsNullOrEmpty(companyCityFilter) || item.CompanyCity.ToLower().Contains(companyCityFilter)) &&
                (string.IsNullOrEmpty(companyCountryFilter) || item.CompanyCountry.ToLower().Contains(companyCountryFilter))
            )
            .ToList();

            // Bind the filtered data to the DataGrid
            LocalCustomerDataGrid.ItemsSource = filteredData;
        }


    }
}
