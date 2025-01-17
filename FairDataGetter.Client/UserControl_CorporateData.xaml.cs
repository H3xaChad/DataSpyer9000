using FairDataGetter.Client.Class;
using System.Windows;
using System.Windows.Controls;

namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CorporateData.xaml
    /// </summary>
    public partial class UserControl_CorporateData : UserControl
    {
        Customer newCustomer;
        Company newCompany;

        public UserControl_CorporateData(Customer customer, Company company = null)
        {
            newCustomer = customer;
            newCompany = company;

            InitializeComponent();

            // Load Corparate Data if available
            if (newCompany != null)
            {
                CompanyNameTextbox.Text = newCompany.Name;
                CompanyAddressTextbox.Text = newCompany.Address.Street;
                CompanyHouseNumberTextbox.Text = newCompany.Address.HouseNumber;
                CompanyPostalCodeTextbox.Text = newCompany.Address.PostalCode;
                CompanyCityTextbox.Text = newCompany.Address.City;
                CompanyCountryTextbox.Text = newCompany.Address.Country;
            }
        }

        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {
            Address companyAddress = new Address
            {
                Street = CompanyAddressTextbox.Text,
                HouseNumber = CompanyHouseNumberTextbox.Text,
                City = CompanyCityTextbox.Text,
                PostalCode = CompanyPostalCodeTextbox.Text,
                Country = CompanyCountryTextbox.Text
            };

            Company newCompany = new Company
            { 
                Name = CompanyNameTextbox.Text,
                Address = companyAddress
            };

            System.Diagnostics.Debug.WriteLine("****** Output After Company Data");
            System.Diagnostics.Debug.WriteLine(newCompany);
            System.Diagnostics.Debug.WriteLine(newCompany.Name);
            System.Diagnostics.Debug.WriteLine(newCompany.Address);
            System.Diagnostics.Debug.WriteLine(newCompany.Address.Street);
            System.Diagnostics.Debug.WriteLine(newCompany.Address.HouseNumber);
            System.Diagnostics.Debug.WriteLine(newCompany.Address.City);
            System.Diagnostics.Debug.WriteLine(newCompany.Address.PostalCode);
            System.Diagnostics.Debug.WriteLine(newCompany.Address.Country);
            System.Diagnostics.Debug.WriteLine("****** End of Output After Company Data");

            MainWindow.UpdateView(new UserControl_CustomerProductGroups(newCustomer, newCompany));
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerData(newCustomer));
        }
    }
}
