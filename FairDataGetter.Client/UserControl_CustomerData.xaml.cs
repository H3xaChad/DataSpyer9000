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
        private Customer customer;

        public UserControl_CustomerData(Customer customer = null)
        {
            this.customer = customer;
            InitializeComponent();

            // If customer data is present (e.g. after customer returns to first screen)
            if (customer != null)
            {
                // Fill UI with customer information
                CustomerAddressTextbox.Text = customer.Address.Street;
                CustomerHouseNumberTextbox.Text = customer.Address.HouseNumber;
                CustomerCityTextbox.Text = customer.Address.City;
                CustomerPostalCodeTextbox.Text = customer.Address.PostalCode;
                CustomerCountryTextbox.Text = customer.Address.Country;

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
                    ImageBase64 = null,
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
                    MainWindow.UpdateView(new UserControl_CorporateData(newCustomer));
                }
                else
                {
                    MainWindow.UpdateView(new UserControl_CustomerProductGroups(newCustomer));
                }
            }
        }

        private void LoginButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_EmployeeLogin());
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Tag.ToString())  // Check if the text matches placeholder
            {
                textBox.Clear();  // Clear the placeholder text
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();  // Reset to placeholder text if empty
            }
        }
    }
}
