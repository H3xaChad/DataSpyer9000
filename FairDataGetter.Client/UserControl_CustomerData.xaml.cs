using FairDataGetter.Client.Class;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CustomerData.xaml
    /// </summary>
    public partial class UserControl_CustomerData : UserControl
    {
        public UserControl_CustomerData()
        {
            InitializeComponent();
        }

        public UserControl_CustomerData(Address address, Customer customer)
            : this() // Call the default constructor
        {
            // Use the provided Customer and Address arguments
            InitializeWithParameters(address, customer);
        }

        private void InitializeWithParameters(Address address, Customer customer)
        {
            CustomerAddressTextbox.Text = address.Street;
            CustomerHouseNumberTextbox.Text = address.HouseNumber;
            CustomerCityTextbox.Text = address.City;
            CustomerPostalCodeTextbox.Text = address.PostalCode;
            CustomerCountryTextbox.Text = address.Country;

            CustomerFirstNameTextbox.Text = customer.FirstName;
            CustomerLastNameTextbox.Text = customer.LastName;
            CustomerEmailTextbox.Text = customer.Email;

            // Set the radio button based on IsCorporateCustomer attribute
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
        }

        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {
            bool selectionTaken = false;

            if (RadioBtn_Yes.IsChecked == false && RadioBtn_No.IsChecked == false)
            {
                MessageBox.Show("Please select an option regarding if you're a corporate customer.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (RadioBtn_Yes.IsChecked == true)
            {
                selectionTaken = true;
                NavigationState.IsCorporateCustomer = true;
            }
            else if (RadioBtn_No.IsChecked == true)
            {
                selectionTaken = true;
                NavigationState.IsCorporateCustomer = false;
            }

            if(selectionTaken)
            {
                Address customerAddress = new Address
                {
                    Street = CustomerAddressTextbox.Text,
                    HouseNumber = CustomerHouseNumberTextbox.Text,
                    City = CustomerCityTextbox.Text,
                    PostalCode = CustomerPostalCodeTextbox.Text,
                    Country = CustomerCountryTextbox.Text
                };

                Customer newCustomer = new Customer
                {
                    FirstName = CustomerFirstNameTextbox.Text,
                    LastName = CustomerLastNameTextbox.Text,
                    Email = CustomerEmailTextbox.Text,
                    ImagePath = "Hardcoded Image Path",
                    Address = customerAddress,
                    IsCorporateCustomer = NavigationState.IsCorporateCustomer
                };

                // Debug Output Statements
                System.Diagnostics.Debug.WriteLine("****** Output After Customer Data");
                System.Diagnostics.Debug.WriteLine(customerAddress);
                System.Diagnostics.Debug.WriteLine(customerAddress.Street);
                System.Diagnostics.Debug.WriteLine(customerAddress.HouseNumber);
                System.Diagnostics.Debug.WriteLine(customerAddress.City);
                System.Diagnostics.Debug.WriteLine(customerAddress.PostalCode);
                System.Diagnostics.Debug.WriteLine(customerAddress.Country);

                System.Diagnostics.Debug.WriteLine(newCustomer);
                System.Diagnostics.Debug.WriteLine(newCustomer.FirstName);
                System.Diagnostics.Debug.WriteLine(newCustomer.LastName);
                System.Diagnostics.Debug.WriteLine(newCustomer.Email);
                System.Diagnostics.Debug.WriteLine(newCustomer.Address);
                System.Diagnostics.Debug.WriteLine(newCustomer.IsCorporateCustomer);
                System.Diagnostics.Debug.WriteLine("****** End of Output After Customer Data");


                if (newCustomer.IsCorporateCustomer)
                {
                    MainWindow.UpdateView(new UserControl_CorporateData(customerAddress, newCustomer));
                }
                else
                {
                    MainWindow.UpdateView(new UserControl_CustomerProductGroups(customerAddress, newCustomer));
                }
            }
        }

        private void LoginButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_EmployeeLogin());
        }
    }
}
