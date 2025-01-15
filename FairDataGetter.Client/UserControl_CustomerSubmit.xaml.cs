using FairDataGetter.Client.Class;
using System.Windows;
using System.Windows.Controls;


namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CustomerSubmit.xaml
    /// </summary>
    public partial class UserControl_CustomerSubmit : UserControl
    {
        Address customerAddress;
        Customer newCustomer;
        Address companyAddress;
        Company newCompany;
        List<ProductGroup> productGroups;

        bool IsCorporateCustomer = false;

        public UserControl_CustomerSubmit(Address address, Customer customer, List<ProductGroup> productGroups, Address companyAddress = null, Company company = null)
        {
            customerAddress = address;
            newCustomer = customer;
            this.productGroups = productGroups;
            this.companyAddress = companyAddress;
            newCompany = company;

            InitializeComponent();

            CustomerFirstNameTextbox.Text = customer.FirstName;
            CustomerLastNameTextbox.Text = customer.LastName;
            CustomerAddressTextbox.Text = customer.Address.Street;
            CustomerHouseNumberTextbox.Text = customer.Address.HouseNumber;
            CustomerPostalCodeTextbox.Text = customer.Address.PostalCode;
            CustomerCityTextbox.Text = customer.Address.City;
            CustomerCountryTextbox.Text = customer.Address.Country;

            if (customer.IsCorporateCustomer)
            {
                RadioBtn_Yes.IsChecked = true;
                RadioBtn_No.IsChecked = false;
            }
            else
            {
                RadioBtn_Yes.IsChecked = false;
                RadioBtn_No.IsChecked = true;
            }

            CompanyNameTextbox.Text = company.Name;
            CompanyAddressTextbox.Text = company.Address.Street;
            CompanyHouseNumberTextbox.Text = company.Address.HouseNumber;
            CompanyPostalCodeTextbox.Text = company.Address.PostalCode;
            CompanyCityTextbox.Text = company.Address.City;

        }
        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
         
            string message = "Customer successfully added";
            MessageBox.Show(message);

            MainWindow.UpdateView(new UserControl_CustomerData());
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerPicture(customerAddress, newCustomer, productGroups));
        }

        private void YesRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsCorporateCustomer = true;
        }

        private void NoRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsCorporateCustomer = false;
        }
    }
}
