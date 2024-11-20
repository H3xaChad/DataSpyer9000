﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {
            if (RadioBtn_Yes.IsChecked == true)
            {
                NavigationState.IsCorporateCustomer = true;
                MainWindow.UpdateView(new UserControl_CorporateData());
            }
            else if (RadioBtn_No.IsChecked == true)
            {
                NavigationState.IsCorporateCustomer = false;
                MainWindow.UpdateView(new UserControl_CustomerProductGroups());
            }
        }

        private void LoginButtonClicked(object sender, RoutedEventArgs e)
        {
            //MainWindow.UpdateView(new UserControl_EmployeeLogin());
        }
    }
}
