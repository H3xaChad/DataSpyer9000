using System;
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
    /// Interaction logic for UserControl_CustomerProductGroups.xaml
    /// </summary>
    public partial class UserControl_CustomerProductGroups : UserControl
    {
        public UserControl_CustomerProductGroups()
        {
            InitializeComponent();
        }

        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerPicture());
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (NavigationState.IsCorporateCustomer)
            {
                MainWindow.UpdateView(new UserControl_CorporateData());
            }
            else
            {
                MainWindow.UpdateView(new UserControl_CustomerData());
            }
        }
    }
}
