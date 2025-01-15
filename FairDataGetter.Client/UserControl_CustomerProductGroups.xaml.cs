using FairDataGetter.Client.Class;
using System.Windows;
using System.Windows.Controls;


namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CustomerProductGroups.xaml
    /// </summary>
    public partial class UserControl_CustomerProductGroups : UserControl
    {
        private Address customerAddress;
        private Customer newCustomer;
        private Address companyAddress;
        private Company newCompany;

        private List<ProductGroup> selectedProductGroups = new List<ProductGroup>();

        public UserControl_CustomerProductGroups(Address address, Customer customer, Address companyAddress = null, Company company = null, List<ProductGroup> productGroups = null)
        {
            customerAddress = address;
            newCustomer = customer;
            this.companyAddress = companyAddress;
            newCompany = company;

            selectedProductGroups = productGroups ?? new List<ProductGroup>();

            InitializeComponent();
        }

        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {

            // Check each checkbox and save its value if selected
            if (ProductGroup1.IsChecked == true)
            {
                ProductGroup productGroup1 = new ProductGroup
                {
                    Name = ProductGroup1.Content.ToString()
                };

                selectedProductGroups.Add(productGroup1);
            }
            if (ProductGroup2.IsChecked == true)
            {
                ProductGroup productGroup2 = new ProductGroup
                {
                    Name = ProductGroup2.Content.ToString()
                };

                selectedProductGroups.Add(productGroup2);
            }
            if (ProductGroup3.IsChecked == true)
            {
                ProductGroup productGroup3 = new ProductGroup
                {
                    Name = ProductGroup3.Content.ToString()
                };

                selectedProductGroups.Add(productGroup3);
            }
            if (ProductGroup4.IsChecked == true)
            {
                ProductGroup productGroup4 = new ProductGroup
                {
                    Name = ProductGroup4.Content.ToString()
                };

                selectedProductGroups.Add(productGroup4);
            }
            if (ProductGroup5.IsChecked == true)
            {
                ProductGroup productGroup5 = new ProductGroup
                {
                    Name = ProductGroup5.Content.ToString()
                };

                selectedProductGroups.Add(productGroup5);
            }
            if (ProductGroup6.IsChecked == true)
            {
                ProductGroup productGroup6 = new ProductGroup
                {
                    Name = ProductGroup6.Content.ToString()
                };

                selectedProductGroups.Add(productGroup6);
            }
            if (ProductGroup7.IsChecked == true)
            {
                ProductGroup productGroup7 = new ProductGroup
                {
                    Name = ProductGroup7.Content.ToString()
                };

                selectedProductGroups.Add(productGroup7);
            }
            if (ProductGroup8.IsChecked == true)
            {
                ProductGroup productGroup8 = new ProductGroup
                {
                    Name = ProductGroup8.Content.ToString()
                };

                selectedProductGroups.Add(productGroup8);
            }

            // Debug Statements**
            System.Diagnostics.Debug.WriteLine("Selected options:");
            foreach (var option in selectedProductGroups)
            {
                System.Diagnostics.Debug.WriteLine(option.Name);
            }
            //***

            if (companyAddress != null && newCompany != null)
            {
                MainWindow.UpdateView(new UserControl_CustomerPicture(customerAddress, newCustomer, selectedProductGroups, companyAddress, newCompany));
            }
            else
            {
                MainWindow.UpdateView(new UserControl_CustomerPicture(customerAddress, newCustomer, selectedProductGroups));
            }
            
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (NavigationState.IsCorporateCustomer)
            {
                MainWindow.UpdateView(new UserControl_CorporateData(customerAddress, newCustomer));
            }
            else
            {
                MainWindow.UpdateView(new UserControl_CustomerData(customerAddress, newCustomer));
            }
        }
    }
}
