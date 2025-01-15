using FairDataGetter.Client.Class;
using System.Windows;
using System.Windows.Controls;


namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CustomerPicture.xaml
    /// </summary>
    public partial class UserControl_CustomerPicture : UserControl
    {
        Address customerAddress;
        Customer newCustomer;
        Address companyAddress;
        Company company;
        List<ProductGroup> productGroups;

        public UserControl_CustomerPicture(Address address, Customer customer, List<ProductGroup> productGroups, Address companyAddress = null, Company company = null)
        {
            customerAddress = address;
            newCustomer = customer;
            this.companyAddress = companyAddress;
            this.company = company;
            this.productGroups = productGroups;

            InitializeComponent();
        }
        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerSubmit(customerAddress, newCustomer, productGroups, companyAddress, company));
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerProductGroups(customerAddress, newCustomer, companyAddress, company, productGroups));
        }
    }
}
