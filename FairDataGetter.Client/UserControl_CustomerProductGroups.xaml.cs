using FairDataGetter.Client.Class;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CustomerProductGroups.xaml
    /// </summary>
    public partial class UserControl_CustomerProductGroups : UserControl
    {

        private Customer newCustomer;
        private Company newCompany;

        public UserControl_CustomerProductGroups(Customer customer, Company company = null)
        {
            newCustomer = customer;
            newCompany = company;

            InitializeComponent();

            if (newCompany != null)
            {
                SecondStepBorder.Background = Brushes.LightGreen;
            }
            else
            {
                SecondStepBorder.Background = Brushes.LightGray;
            }
        }

        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {
            newCustomer.InterestedProductGroups = ProductGroupList.Items.Cast<string>().ToList();
            if (newCompany != null)
            {
                MainWindow.UpdateView(new UserControl_CustomerPicture(newCustomer, newCompany));
            }
            else
            {
                MainWindow.UpdateView(new UserControl_CustomerPicture(newCustomer));
            }
            
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (newCompany != null)
            {
                MainWindow.UpdateView(new UserControl_CorporateData(newCustomer, newCompany));
            }
            else
            {
                MainWindow.UpdateView(new UserControl_CustomerData(newCustomer));
            }
        }

        private void AddProductGroup(object sender, RoutedEventArgs e)
        {
            var productGroup = ProductGroupInput.Text.Trim();
            if (!string.IsNullOrEmpty(productGroup) && !ProductGroupList.Items.Contains(productGroup))
            {
                ProductGroupList.Items.Add(productGroup);
                ProductGroupInput.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a unique product group name.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveProductGroup(object sender, RoutedEventArgs e)
        {
            if (sender is not Button removeButton) return;
            var productGroup = removeButton.DataContext as string;
            if (!string.IsNullOrEmpty(productGroup))
            {
                ProductGroupList.Items.Remove(productGroup);
            }
        }

    }
}
