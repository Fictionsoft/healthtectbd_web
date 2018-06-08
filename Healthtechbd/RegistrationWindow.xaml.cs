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
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private MainWindow mainWindow;
        private ForgotPasswordWindow forgotPasswordWindow;

        public RegistrationWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        public RegistrationWindow(ForgotPasswordWindow forgotPasswordWindow)
        {
            InitializeComponent();
            this.forgotPasswordWindow = forgotPasswordWindow;
        }

        private void loginLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();            
            this.mainWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
            //this.mainWindow.Show();
        }       
    }
}
