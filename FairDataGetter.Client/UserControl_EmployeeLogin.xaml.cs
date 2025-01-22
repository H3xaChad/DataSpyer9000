using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Security.Cryptography;


namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_EmployeeLogin.xaml
    /// </summary>
    public partial class UserControl_EmployeeLogin : UserControl
    {
        private string _employeeID;
        private string _password;
        
        public UserControl_EmployeeLogin()
        {
            InitializeComponent();
        }

        private void LoginButtonClicked(object sender, RoutedEventArgs e)
        {
            _employeeID = EmployeeIdTextbox.Text;

            if (IsValidLogin(_employeeID, _password))
            {
                MainWindow.UpdateView(new UserControl_EmployeeDashboard());
            }
            else
            {
                MessageBox.Show("Invalid Credentials!");
            }
        }

        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateView(new UserControl_CustomerData());
        }

        // Check if employeeID and password are correct
        private bool IsValidLogin(string employeeID, string password)
        {
            string storedHashedPassword = "ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f";

            return employeeID == "admin" && HashPassword(password) == storedHashedPassword;
        }

        // Hash password using sha256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        // Aktualisiere das Passwort-Feld (_password) bei Änderungen in der PasswordBox
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _password = passwordBox.Password;
            }
        }
    }
}
