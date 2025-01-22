using FairDataGetter.Client.Class;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_EmployeeRemoteDatabase.xaml
    /// </summary>
    public partial class UserControl_EmployeeRemoteDatabase : UserControl
    {
        List<ExportData> remoteData = new List<ExportData>();

        public UserControl_EmployeeRemoteDatabase()
        {
            InitializeComponent();
            LoadApiData();
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_EmployeeDashboard());
        }

        private async Task LoadApiData()
        {
            // Define the API URL (replace with your actual API URL)
            string apiUrl = "http://localhost:5019/api/Customers";



            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Call the API and get the JSON response
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    // Read the JSON response as a string
                    string responseData = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response into a list of dynamic objects
                    List<dynamic> importData = JsonConvert.DeserializeObject<List<dynamic>>(responseData);

                    foreach (var data in importData)
                    {
                        ExportData customerData = new ExportData();

                        bool corporate = false;
                        int customerAddressId = data.addressId;
                        int? customerCompanyId = data.companyId;

                        Address customerAddress = await GetAddressById(customerAddressId);


                        if (customerCompanyId != null)
                        {
                           (Company company, int companyAddressId) = await GetCompanyById((int)customerCompanyId);
                            Address companyAddress = await GetAddressById(companyAddressId);

                            company.Address = companyAddress;
                            customerData.Company = company;
                            corporate = true;
                        }

                        var productGroups = JsonConvert.DeserializeObject<List<string>>(data.interestedProductGroups.ToString());


                        Customer customer = new Customer()
                        {
                            FirstName = data.firstName,
                            LastName = data.lastName,
                            ImageBase64 = data.imagePath,
                            Email = data.email,
                            Address = customerAddress,
                            IsCorporateCustomer = corporate,
                            InterestedProductGroups = productGroups
                        };

                        foreach(var pg in customer.InterestedProductGroups)
                        {
                            Debug.WriteLine(pg);
                        }

                        customerData.Customer = customer;

                        remoteData.Add(customerData);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching data: {ex.Message}");
                }
            }

            // Bind the data to the DataGrid
            RemoteCustomerDataGrid.ItemsSource = remoteData;
        }

        private async Task<Address> GetAddressById(int addressId)
        {
            string apiUrl = $"http://localhost:5019/api/Address/{addressId}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Read the JSON response as a string
                string addressData = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON response into a list of dynamic objects
                Address customerAddress = JsonConvert.DeserializeObject<Address>(addressData);

                return customerAddress;
            }
 
        }

        private async Task<(Company, Int32)> GetCompanyById(int companyId)
        {
            string apiUrl = $"http://localhost:5019/api/Companies/{companyId}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Read the JSON response as a string
                string companyData = await response.Content.ReadAsStringAsync();

                dynamic companyRaw = JsonConvert.DeserializeObject<dynamic>(companyData);

                int addressId = companyRaw.addressId;

                // Deserialize the JSON response into a list of dynamic objects
                Company company = JsonConvert.DeserializeObject<Company>(companyData);

                return (company, addressId);
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
