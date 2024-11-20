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
    /// Interaction logic for UserControl_CustomerSubmit.xaml
    /// </summary>
    public partial class UserControl_CustomerSubmit : UserControl
    {
        public UserControl_CustomerSubmit()
        {
            InitializeComponent();
        }
        private void SubmitButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerData());
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerPicture());
        }
    }
}
