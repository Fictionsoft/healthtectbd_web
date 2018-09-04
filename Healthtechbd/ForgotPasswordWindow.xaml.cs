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
using Healthtechbd.model;

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

        contextd_db db = new contextd_db();
        user user = new user();

        private void SubmitForgotPassword_Click(object sender, RoutedEventArgs e)
        {       
            if(EmailAddress.Text != "Email Address")
            {
                try
                {
                    user = db.users.FirstOrDefault(x => x.email == EmailAddress.Text && x.role_id == 1); //user = doctor,,,role_id 1 = doctor

                    if (user != null)
                    {
                        this.Hide();
                        ResetPasswordWindow resetPasswordWindow = new ResetPasswordWindow(user.email);
                        resetPasswordWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("Email not found", "Invalid Doctor", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
            else
            {
                MessageBox.Show("Email is required", "Required Field");
            }            
        }

        private void EmailAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if (EmailAddress.Text == "Email Address")
            {
                EmailAddress.Text = "";
            }
        }

        private void EmailAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            if (EmailAddress.Text == "")
            {
                EmailAddress.Text = "Email Address";
            }
        }
    }
}
