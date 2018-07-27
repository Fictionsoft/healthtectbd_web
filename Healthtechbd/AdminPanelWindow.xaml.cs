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
using System.Windows.Shapes;


namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        public static Grid sidebar; // To get sidebar
        public static ColumnDefinition sidebarColumnDefination; // To get sidebarColumnDefenation        
        private RegistrationWindow registrationWindow;
        private ResetPasswordWindow resetPasswordWindow;
        public static TextBlock userName;

        public AdminPanelWindow(MainWindow mainWindow)
        {
            InitializeComponent();                       

            sidebar = Sidebar; // To get sidebar
            sidebarColumnDefination = SidebarColumnDefination;// To get sidebar 
            userName = UserName; // To get user name textblock
        }

        public AdminPanelWindow(RegistrationWindow registrationWindow)
        {
            InitializeComponent();
           
            sidebar = Sidebar; // To get sidebar
            sidebarColumnDefination = SidebarColumnDefination;// To get sidebar 
            userName = UserName; // To get user name textblock

            this.registrationWindow = registrationWindow;
        }

        public AdminPanelWindow(ResetPasswordWindow resetPasswordWindow)
        {
            InitializeComponent();
            
            sidebar = Sidebar; // To get sidebar
            sidebarColumnDefination = SidebarColumnDefination;// To get sidebar   
            userName = UserName; // To get user name textblock

            this.resetPasswordWindow = resetPasswordWindow;
        }

        private void header_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonDashboard_MouseDown(object sender, MouseButtonEventArgs e)
        {           
            MainContent.Content = new Dashboard();
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Visible;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigationbar).        
        }

        private void ButtonDiagnosis_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContent.Content = new Diagnosis();
        }

        private void ButtonMedicines_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContent.Content = new Medicines();
        }

        private void ButtonTests_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContent.Content = new Tests();
        }

        private void ButtonPatients_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContent.Content = new Patients();
        }

        private void ButtonDiagnosisTemplates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContent.Content = new DiagnosisTemplates();
        }

        private void ButtonPrescriptions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainContent.Content = new Prescriptions();
        }

        //private void ButtonDoctors_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    MainContent.Content = new Doctors();
        //}

        //private void ButtonSettins_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    MainContent.Content = new Settings();
        //}

        private void ButtonMyProfile_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new MyProfile();
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Visible;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigationbar).                        
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ChangePassword();
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Visible;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigationbar).
        }

        private void ButtonProfileSettins_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Settings();
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Visible;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigationbar).
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();            
        }

        private void ButtonDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Dashboard();
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
