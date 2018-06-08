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

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();           
        }       

        private void registrationLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            RegistrationWindow registrationWindow = new RegistrationWindow(this);
            registrationWindow.Show();
        }

        private void forgotPasswordLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            ForgotPasswordWindow forgotPasswordWindow = new ForgotPasswordWindow(this);
            forgotPasswordWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }        

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AdminPanelWindow adminPanelWindow = new AdminPanelWindow(this);
            adminPanelWindow.Show();           
        }
    }
}
