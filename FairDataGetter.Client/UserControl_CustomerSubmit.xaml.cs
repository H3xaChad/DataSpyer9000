﻿using FairDataGetter.Client.Class;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;


namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CustomerSubmit.xaml
    /// </summary>
    public partial class UserControl_CustomerSubmit : UserControl
    {

        Customer newCustomer;
        Company newCompany;

        public UserControl_CustomerSubmit(Customer customer, Company company = null)
        {

            newCustomer = customer;
            newCompany = company;

            InitializeComponent();

            CustomerFirstNameTextbox.Text = newCustomer.FirstName;
            CustomerLastNameTextbox.Text = newCustomer.LastName;
            CustomerAddressTextbox.Text = newCustomer.Address.Street;
            CustomerHouseNumberTextbox.Text = newCustomer.Address.HouseNumber;
            CustomerPostalCodeTextbox.Text = newCustomer.Address.PostalCode;
            CustomerCityTextbox.Text = newCustomer.Address.City;
            CustomerCountryTextbox.Text = newCustomer.Address.Country;
            CustomerPictureImage.Source = ConvertBase64ToImageSource(newCustomer.ImageBase64);
            CustomerProductGroupsListbox.ItemsSource = newCustomer.InterestedProductGroups;

            // If customer is corparate customer
            if (newCustomer.IsCorporateCustomer)
            {
                // Select checkboxes and disable them
                RadioBtn_Yes.IsChecked = true;
                RadioBtn_No.IsChecked = false;
                RadioBtn_Yes.IsEnabled = false;
                RadioBtn_No.IsEnabled = false;

                // Load corparate data into corparate textboxes
                CompanyNameTextbox.Text = newCompany.Name;
                CompanyAddressTextbox.Text = newCompany.Address.Street;
                CompanyHouseNumberTextbox.Text = newCompany.Address.HouseNumber;
                CompanyPostalCodeTextbox.Text = newCompany.Address.PostalCode;
                CompanyCityTextbox.Text = newCompany.Address.City;
                CompanyCountryTextbox.Text = newCompany.Address.Country;
            }
            // If customer is not corparate customer
            else
            {
                // Select checkboxes and disable them
                RadioBtn_Yes.IsChecked = false;
                RadioBtn_No.IsChecked = true;
                RadioBtn_Yes.IsEnabled = false;
                RadioBtn_No.IsEnabled = false;
                
                // Clear and disable corparate textboxes
                CompanyNameTextbox.IsEnabled = false;
                CompanyNameBorder.Background = new SolidColorBrush(Colors.LightGray);
                CompanyAddressTextbox.IsEnabled = false;
                CompanyAddressBorder.Background = new SolidColorBrush(Colors.LightGray);
                CompanyHouseNumberTextbox.IsEnabled = false;
                CompanyHouseNumberBorder.Background = new SolidColorBrush(Colors.LightGray);
                CompanyPostalCodeTextbox.IsEnabled = false;
                CompanyPostalCodeBorder.Background = new SolidColorBrush(Colors.LightGray);
                CompanyCityTextbox.IsEnabled = false;
                CompanyCityBorder.Background = new SolidColorBrush(Colors.LightGray);
                CompanyCountryTextbox.IsEnabled = false;
                CompanyCountryBorder.Background = new SolidColorBrush(Colors.LightGray);

            }
        }

        // Submit
        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get current data from customer textboxes
                newCustomer.FirstName = CustomerFirstNameTextbox.Text;
                newCustomer.LastName = CustomerLastNameTextbox.Text;
                newCustomer.Address.Street = CustomerAddressTextbox.Text;
                newCustomer.Address.HouseNumber = CustomerHouseNumberTextbox.Text;
                newCustomer.Address.PostalCode = CustomerPostalCodeTextbox.Text;
                newCustomer.Address.City = CustomerCityTextbox.Text;
                newCustomer.Address.Country = CustomerCountryTextbox.Text;

                // Get current data from corparate textboxes
                if (newCompany != null)
                {
                     newCompany.Name = CompanyNameTextbox.Text;
                     newCompany.Address.Street = CompanyAddressTextbox.Text;
                     newCompany.Address.HouseNumber = CompanyHouseNumberTextbox.Text;
                     newCompany.Address.PostalCode = CompanyPostalCodeTextbox.Text;
                     newCompany.Address.City = CompanyCityTextbox.Text;
                     newCompany.Address.Country = CompanyCountryTextbox.Text;
                }

                // Create export data
                ExportData newCustomerData = new ExportData();

                if (newCompany != null)
                {
                    newCustomerData.Company = newCompany;
                    newCustomerData.Customer = newCustomer;
                }
                else
                {
                    newCustomerData.Customer = newCustomer;
                }

                // Create list of all data
                List<ExportData> allCustomerData = new List<ExportData>();

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, "customer_data.json");

                // Checks if export files exists
                if (File.Exists(filePath))
                {
                    // If file exists, read the existing data
                    string importDataJson = File.ReadAllText(filePath);
                    allCustomerData = JsonConvert.DeserializeObject<List<ExportData>>(importDataJson);
                }

                // Add new data to existing data
                allCustomerData.Add(newCustomerData);

                // Serialize and export all data
                string exportAllDataJson = JsonConvert.SerializeObject(allCustomerData, Formatting.Indented);
                File.WriteAllText(filePath, exportAllDataJson);

                string message = "Customer successfully added";
                MessageBox.Show(message);

                MainWindow.UpdateView(new UserControl_CustomerData());
            }
            catch(Exception ex) 
            {
                MessageBox.Show($"An error occurred while exporting the data: {ex.Message}");
            }
        }

        // Return
        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerPicture(newCustomer, newCompany));
        }

        // Covert base64 to imagesource, to display customer picture
        private ImageSource ConvertBase64ToImageSource(string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                MemoryStream ms = new MemoryStream(imageBytes);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting Base64 to ImageSource: {ex.Message}");
                return null;
            }
        }
    }
}
