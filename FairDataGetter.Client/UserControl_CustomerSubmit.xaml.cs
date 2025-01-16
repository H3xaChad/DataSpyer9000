using FairDataGetter.Client.Class;
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

            CustomerFirstNameTextbox.Text = customer.FirstName;
            CustomerLastNameTextbox.Text = customer.LastName;
            CustomerAddressTextbox.Text = customer.Address.Street;
            CustomerHouseNumberTextbox.Text = customer.Address.HouseNumber;
            CustomerPostalCodeTextbox.Text = customer.Address.PostalCode;
            CustomerCityTextbox.Text = customer.Address.City;
            CustomerCountryTextbox.Text = customer.Address.Country;
            CustomerPictureImage.Source = ConvertBase64ToImageSource(customer.ImageBase64);
            CustomerProductGroupsListbox.ItemsSource = customer.InterestedProductGroups;

            if (customer.IsCorporateCustomer)
            {
                RadioBtn_Yes.IsChecked = true;
                RadioBtn_No.IsChecked = false;
                RadioBtn_Yes.IsEnabled = false;
                RadioBtn_No.IsEnabled = false;

                CompanyNameTextbox.Text = company.Name;
                CompanyAddressTextbox.Text = company.Address.Street;
                CompanyHouseNumberTextbox.Text = company.Address.HouseNumber;
                CompanyPostalCodeTextbox.Text = company.Address.PostalCode;
                CompanyCityTextbox.Text = company.Address.City;
                CompanyCountryTextbox.Text = company.Address.Country;
            }
            else
            {
                RadioBtn_Yes.IsChecked = false;
                RadioBtn_No.IsChecked = true;
                RadioBtn_Yes.IsEnabled = false;
                RadioBtn_No.IsEnabled = false;

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
        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
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

                allCustomerData.Add(newCustomerData);

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

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerPicture(newCustomer, newCompany));
        }

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
