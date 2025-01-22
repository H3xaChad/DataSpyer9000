using System.Diagnostics;
using FairDataGetter.Client.Class;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_EmployeeRemoteDatabase.xaml
    /// </summary>
    public partial class UserControl_EmployeeRemoteDatabase : UserControl
    {
        List<ExportData> remoteData = [];

        public UserControl_EmployeeRemoteDatabase()
        {
            InitializeComponent();

            this.Loaded += UserControl_EmployeeRemoteDatabase_Loaded;
        }

        private async void UserControl_EmployeeRemoteDatabase_Loaded(object sender, RoutedEventArgs e)
        {
            await CountCustomersFromApiAsync("http://localhost:5019/api/Customer");
            await LoadApiData();
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_EmployeeDashboard());
        }

        private async Task LoadApiData() {
            const string apiUrl = "http://localhost:5019/api/Customer";
            using (var client = new HttpClient()) {
                try {
                    var response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();
                    var responseData = await response.Content.ReadAsStringAsync();
                    var dataJson = JsonConvert.DeserializeObject<List<dynamic>>(responseData);
                    if (dataJson == null) return;
                    foreach (var customerJson in dataJson) {
                        Debug.WriteLine($"Customer Json: {customerJson.interestedProductGroups.ToString()}");
                        var customerData = new ExportData();

                        var address = new Address() {
                            Street = customerJson.address.street,
                            HouseNumber = customerJson.address.houseNumber,
                            PostalCode = customerJson.address.postalCode,
                            City = customerJson.address.city,
                            Country = customerJson.address.country
                        };

                        var customer = new Customer() {
                            FirstName = customerJson.firstName,
                            LastName = customerJson.lastName,
                            ImageBase64 = customerJson.imagePath,
                            Email = customerJson.email,
                            Address = address,
                            IsCorporateCustomer = customerJson.companyId != null,
                            InterestedProductGroups = customerJson.interestedProductGroups != null
                            ? JsonConvert.DeserializeObject<List<string>>(customerJson.interestedProductGroups.ToString())
                            : new List<string>()
                        };

                        if (customerJson.company != null)
                        {
                            var companyAddress = new Address()
                            {
                                Street = customerJson.company.address.street,
                                HouseNumber = customerJson.company.address.houseNumber,
                                PostalCode = customerJson.company.address.postalCode,
                                City = customerJson.company.address.city,
                                Country = customerJson.company.address.country
                            };

                            var company = new Company()
                            {
                                Name = customerJson.company.name,
                                Address = companyAddress
                            };

                            customerData.Company = company;
                        }

                        customerData.Customer = customer;

                        remoteData.Add(customerData);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Error fetching data: {ex.Message} - {ex.Source} - {ex.Data}");
                }
            }
            // Bind the data to the DataGrid
            RemoteCustomerDataGrid.ItemsSource = remoteData;
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

                    // Count corporate customers (those with a non-null "company" object)
                    int corporateCustomerCount = customers.Count(c => c.company != null);

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
            var filteredData = remoteData
                .Where(c =>
                    (string.IsNullOrEmpty(firstNameFilter) || c.Customer.FirstName.ToLower().Contains(firstNameFilter)) &&
                    (string.IsNullOrEmpty(lastNameFilter) || c.Customer.LastName.ToLower().Contains(lastNameFilter)) &&
                    (string.IsNullOrEmpty(emailFilter) || c.Customer.Email.ToLower().Contains(emailFilter)) &&
                    (string.IsNullOrEmpty(streetFilter) || c.Customer.Address.Street.ToLower().Contains(streetFilter)) &&
                    (string.IsNullOrEmpty(houseNumberFilter) || c.Customer.Address.HouseNumber.ToLower().Contains(houseNumberFilter)) &&
                    (string.IsNullOrEmpty(postalCodeFilter) || c.Customer.Address.PostalCode.ToLower().Contains(postalCodeFilter)) &&
                    (string.IsNullOrEmpty(cityFilter) || c.Customer.Address.City.ToLower().Contains(cityFilter)) &&
                    (string.IsNullOrEmpty(countryFilter) || c.Customer.Address.Country.ToLower().Contains(countryFilter)) &&
                    (string.IsNullOrEmpty(companyNameFilter) || (c.Company?.Name ?? "").ToLower().Contains(companyNameFilter)) &&
                    (string.IsNullOrEmpty(companyStreetFilter) || (c.Company?.Address?.Street ?? "").ToLower().Contains(companyStreetFilter)) &&
                    (string.IsNullOrEmpty(companyHouseNumberFilter) || (c.Company?.Address?.HouseNumber ?? "").ToLower().Contains(companyHouseNumberFilter)) &&
                    (string.IsNullOrEmpty(companyPostalCodeFilter) || (c.Company?.Address?.PostalCode ?? "").ToLower().Contains(companyPostalCodeFilter)) &&
                    (string.IsNullOrEmpty(companyCityFilter) || (c.Company?.Address?.City ?? "").ToLower().Contains(companyCityFilter)) &&
                    (string.IsNullOrEmpty(companyCountryFilter) || (c.Company?.Address?.Country ?? "").ToLower().Contains(companyCountryFilter))
                )
                .ToList();

            // Bind the filtered data to the DataGrid
            RemoteCustomerDataGrid.ItemsSource = filteredData;
        }
    }
}