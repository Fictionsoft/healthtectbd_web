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
    /// Interaction logic for ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        private MainWindow mainWindow;  

        public ForgotPasswordWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            //this.mainWindow.Show();
        }

        private void registrationLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            RegistrationWindow registrationWindow = new RegistrationWindow(this);
            registrationWindow.Show();
        }

        private void loginLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            this.mainWindow.Show();
        }
    }
}
