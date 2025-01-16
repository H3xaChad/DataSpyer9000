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
        Address customerAddress;
        Customer newCustomer;

        public UserControl_CorporateData(Customer customer)
        {
            newCustomer = customer;

            InitializeComponent();
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
