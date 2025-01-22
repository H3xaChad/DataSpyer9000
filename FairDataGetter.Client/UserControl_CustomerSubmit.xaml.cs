using FairDataGetter.Client.Class;
using System.ComponentModel.DataAnnotations;
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

            if (newCompany != null)
            {
                SecondStepBorder.Background = System.Windows.Media.Brushes.LightGreen;
            }
            else
            {
                SecondStepBorder.Background = System.Windows.Media.Brushes.LightGray;
            }

            CustomerFirstNameTextbox.Text = newCustomer.FirstName;
            CustomerLastNameTextbox.Text = newCustomer.LastName;
            CustomerEmailTextbox.Text = newCustomer.Email;
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

        private bool ValidateCustomerData(Customer customer, Company company = null)
        {
            // Create an array of tuples with name, content, and length constraints
            var fieldsToValidate = new List<(string fieldName, string fieldContent, (int minLength, int maxLength) lengthRange)>
            {
                ("First Name", customer.FirstName, (1, 42)),
                ("Last Name", customer.LastName, (1, 42)),
                ("Email", customer.Email, (4, 42)),
                ("Customer Address", customer.Address.Street, (3, 42)),
                ("Customer HouseNumber", customer.Address.HouseNumber, (1, 42)),
                ("Customer City", customer.Address.City, (2, 42)),
                ("Customer PostalCode", customer.Address.PostalCode, (3, 42)),
                ("Customer Country", customer.Address.Country, (3, 42))
            };

            // Validate Textboxes not empty and within length requirment of api
            foreach (var (name, content, (min, max)) in fieldsToValidate)
            {
                if (string.IsNullOrEmpty(content))
                {
                    MessageBox.Show($"{name} can not be empty!");
                    return false;
                }

                if (content.Length < min || content.Length > max)
                {
                    MessageBox.Show($"{name} has to be shorter than {min} and longer than {max}");
                    return false;
                }
            }

            // Validate Customer mail
            var emailAddress = new EmailAddressAttribute();
            if (!emailAddress.IsValid(customer.Email))
            {
                MessageBox.Show($"The email address: '{customer.Email}' is not valid! Please enter a valid mail address");
                return false;
            }

            // Validate Company if present
            if (company != null)
            {
                fieldsToValidate = new List<(string fieldName, string fieldContent, (int minLength, int maxLength) lengthRange)>
                {
                    ("Company Name", company.Name, (2, 42)),
                    ("Company Address", company.Address.Street, (3, 42)),
                    ("Company HouseNumber", company.Address.HouseNumber, (1, 42)),
                    ("Company City", company.Address.City, (2, 42)),
                    ("Company PostalCode", company.Address.PostalCode, (3, 42)),
                    ("Company Country", company.Address.Country, (3, 42))
                };

                // Validate Textboxes not empty and within length requirment of api
                foreach (var (name, content, (min, max)) in fieldsToValidate)
                {
                    if (string.IsNullOrEmpty(content))
                    {
                        MessageBox.Show($"{name} can not be empty!");
                        return false;
                    }

                    if (content.Length < min || content.Length > max)
                    {
                        MessageBox.Show($"{name} has to be shorter than {max} and longer than {min}");
                        return false;
                    }
                }
            }

            // Return true, if all checks passed
            return true;
        }

        // Submit
        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create list of all data
                List<ExportData> allCustomerData = new List<ExportData>();

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(documentsPath, "local_customer_data.json");

                // Checks if export files exists
                if (File.Exists(filePath))
                {
                    // If file exists, read the existing data
                    string importDataJson = File.ReadAllText(filePath);
                    allCustomerData = JsonConvert.DeserializeObject<List<ExportData>>(importDataJson);
                }

                // Get current data from customer textboxes
                newCustomer.FirstName = CustomerFirstNameTextbox.Text;
                newCustomer.LastName = CustomerLastNameTextbox.Text;
                newCustomer.Email = CustomerEmailTextbox.Text;
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

                // Validate the data
                if (ValidateCustomerData(newCustomer, newCompany))
                {
                    // Create export data
                    ExportData newCustomerData = new ExportData();
                    newCustomerData.Customer = newCustomer;

                    if (newCompany != null)
                    {
                        newCustomerData.Company = newCompany;
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
