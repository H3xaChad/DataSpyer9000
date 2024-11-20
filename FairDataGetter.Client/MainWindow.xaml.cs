using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;

namespace FairDataGetter.Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private object currentView;

        public static MainWindow Instance { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public object CurrentView
        {
            get => currentView;
            set
            {
                currentView = value;
                OnPropertyChanged(); // Notify the UI about the property change
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            // Set the initial view
            CurrentView = new UserControl_CustomerData();
            DataContext = this; // Set DataContext for binding
        }

        public static void UpdateView(UserControl newView)
        {
            Instance.CurrentView = newView; // Update the view
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
