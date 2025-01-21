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
        private List<ExportData> transmittedCustomerData = new List<ExportData>();

        public UserControl_EmployeeDashboard()
        {
            InitializeComponent();
            this.Loaded += UserControl_EmployeeDashboard_Loaded;
        }

        private async void UserControl_EmployeeDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            await CountCustomersFromApiAsync("https://localhost:7126/api/Customers");
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
                    InterestedProductGroups = string.Join(", ", c.Customer.InterestedProductGroups.Select(p => p.Name)),
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
                        companyId = await SendCompanyAsync(customerData.Company);
                    }

                    // Transmit customer data
                    await SendCustomerAsync(customerData.Customer, companyId);

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


        private async Task<int> SendCompanyAsync(Company company)
        {
            string companyAPI = "https://localhost:7126/api/Companies";

            if (company == null || company.Address == null)
            {
                throw new ArgumentNullException("Company or Address cannot be null.");
            }

            // Prepare the data to be sent
            var companyData = new
            {
                name = company.Name,
                address = new
                {
                    id = 0,
                    street = company.Address.Street,
                    houseNumber = company.Address.HouseNumber,
                    city = company.Address.City,
                    postalCode = company.Address.PostalCode,
                    country = company.Address.Country
                }
            };

            // Serialize to JSON (requires System.Text.Json)
            string jsonData = System.Text.Json.JsonSerializer.Serialize(companyData);

            // Simulate sending the data (e.g., via HTTP POST)
            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await httpClient.PostAsync(companyAPI, content);

                // Check if the request was successful (status code 2xx)
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as string
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the response to get the company ID (expecting JSON in { "Id": 123 })
                    var responseData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseContent);

                    // Extract the ID from the response JSON
                    int companyId = responseData.GetProperty("id").GetInt32();

                    // Return the company ID
                    MessageBox.Show($"Company Id: {companyId}");
                    return companyId;
                }
                else
                {
                    // Handle error if the request was unsuccessful
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error creating company: {errorResponse}");
                }
            }

        }

        private async Task<int> SendCustomerAsync(Customer customer, int? companyId = null)
        {
            string customerAPI = "https://localhost:7126/api/Customers"; 

            if (customer == null || customer.Address == null)
            {
                throw new ArgumentNullException("Customer or Address cannot be null.");
            }

            using (HttpClient httpClient = new HttpClient())
            {
                // Create a MultipartFormDataContent to send the data as form-data
                var content = new MultipartFormDataContent();

                // Add customer details as form fields
                content.Add(new StringContent(customer.FirstName), "FirstName");
                content.Add(new StringContent(customer.LastName), "LastName");
                content.Add(new StringContent(customer.Email), "Email");

                content.Add(new StringContent("0"), "Address.Id");
                content.Add(new StringContent(customer.Address.Street), "Address.Street");
                content.Add(new StringContent(customer.Address.HouseNumber), "Address.HouseNumber");
                content.Add(new StringContent(customer.Address.City), "Address.City");
                content.Add(new StringContent(customer.Address.PostalCode), "Address.PostalCode");
                content.Add(new StringContent(customer.Address.Country), "Address.Country");

                var productGroupNamesJson = System.Text.Json.JsonSerializer.Serialize(
                    customer.InterestedProductGroups.Select(p => p.Name).ToList()
                );

                customer.InterestedProductGroups = customer.InterestedProductGroups ?? new List<ProductGroup>();

                content.Add(new StringContent(productGroupNamesJson, Encoding.UTF8, "application/json"), "InterestedProductGroupNames");

                // Debug Output Statements
                System.Diagnostics.Debug.WriteLine("****** Output After Customer Data");
                System.Diagnostics.Debug.WriteLine(productGroupNamesJson);
                System.Diagnostics.Debug.WriteLine("****** End of Output After Customer Data");

                // Optionally add the companyId to the form data if provided
                if (companyId.HasValue)
                {
                    content.Add(new StringContent(companyId.Value.ToString()), "CompanyId");
                }

                // Convert Base64 string to byte array
                byte[] imageBytes = Convert.FromBase64String(customer.ImageBase64);

                // Create ByteArrayContent and set the correct content type
                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

                // Add image content to the form data with the field name "Image"
                content.Add(imageContent, "Image", "image.png"); // "image.png" is the filename in the form


                // Send the POST request
                HttpResponseMessage response = await httpClient.PostAsync(customerAPI, content);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as string
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserialize the response to get the customer ID (expecting JSON in { "Id": 123 })
                    var responseData = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseContent);

                    // Extract the ID from the response JSON
                    int customerId = responseData.GetProperty("id").GetInt32();

                    // Return the customer ID
                    MessageBox.Show($"Customer Id: {customerId}");
                    return customerId;
                }
                else
                {
                    // Handle error if the request was unsuccessful
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error creating customer: {errorResponse}");
                }
            }
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
                InterestedProductGroups = string.Join(", ", c.Customer.InterestedProductGroups.Select(p => p.Name)),
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
