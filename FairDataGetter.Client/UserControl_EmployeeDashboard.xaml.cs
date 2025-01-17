using FairDataGetter.Client.Class;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Text;

namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_EmployeeDashboard.xaml
    /// </summary>
    public partial class UserControl_EmployeeDashboard : UserControl
    {
        private List<ExportData> localCustomerData;

        public UserControl_EmployeeDashboard()
        {
            InitializeComponent();
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
                    MessageBox.Show($"The file 'data.json' was not found in the Documents folder.", "File Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Read the JSON file
                string jsonData = File.ReadAllText(jsonFilePath);

                // Deserialize JSON into a list of customers
                localCustomerData = JsonConvert.DeserializeObject<List<ExportData>>(jsonData);

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
            catch (JsonException ex)
            {
                MessageBox.Show($"Error parsing JSON data: {ex.Message}", "JSON Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Error accessing the file: {ex.Message}", "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerData());
        }

        private async void UpdateButtonClicked(object sender, RoutedEventArgs e)
        {
            // Call the SendCustomersOneByOneAsync method
            await SendCustomersOneByOneAsync(localCustomerData);
        }


        public async Task SendCustomersOneByOneAsync(List<ExportData> customerData)
        {
            /*
            using (HttpClient client = new HttpClient())
            {
                // ToDo: Define API Endpoint
                string apiUrl = "https://yourapi.com/endpoint";

                foreach (ExportData customer in customerData.ToList()) // ToList() to avoid modifying the collection while iterating
                {
                    // Serialize the ExportData (which already contains the necessary data, since ExportData = customer, company)
                    string jsonData = JsonConvert.SerializeObject(customer);

                    // Create HTTP content
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    try
                    {
                        // Send the data to the API
                        HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            // If the request is successful, handle the success
                            MessageBox.Show($"Customer {customer.Customer.FirstName} {customer.Customer.LastName} sent successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Remove the customer from the list (safely done by using ToList() in the foreach)
                            customerData.Remove(customer);

                            // Delete the customer from the local customer data JSON file
                            RemoveCustomerFromLocalData(customer);

                            // Optionally: Save the customer to a separate JSON file
                            // SaveSentCustomerToJson(customer);
                        }
                        else
                        {
                            // Handle API error
                            MessageBox.Show($"Failed to send customer {customer.Customer.FirstName} {customer.Customer.LastName}: {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle network errors or any other exceptions
                        MessageBox.Show($"An error occurred while sending customer {customer.Customer.FirstName}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }*/
        }

        private void RemoveCustomerFromLocalData(ExportData sentCustomer)
        {
            // Get the path to the Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Define the file path for the local customer data
            string localCustomerDataFilePath = Path.Combine(documentsPath, "local_customer_data.json");

            // Check if the file exists
            if (File.Exists(localCustomerDataFilePath))
            {
                // Read the existing customer data into a list
                var existingCustomerData = JsonConvert.DeserializeObject<List<ExportData>>(File.ReadAllText(localCustomerDataFilePath));

                // Find the customer to remove (you may use a unique property like the customer ID for comparison)
                ExportData customerToRemove = existingCustomerData.FirstOrDefault(c => c.Customer.Id == sentCustomer.Customer.Id);

                if (customerToRemove != null)
                {
                    // Remove the customer from the list
                    existingCustomerData.Remove(customerToRemove);

                    // Write the updated list back to the file
                    File.WriteAllText(localCustomerDataFilePath, JsonConvert.SerializeObject(existingCustomerData, Formatting.Indented));
                }
            }
            else
            {
                // Handle case where the file doesn't exist
                MessageBox.Show("The local customer data file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
