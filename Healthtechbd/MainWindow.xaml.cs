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

        model.ContextDb db = new model.ContextDb();
        model.user user = new model.user();

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
            AdminPanelWindow adminpanelwindow = new AdminPanelWindow(this);
            adminpanelwindow.Show();
            MessageBox.Show("Login successfully", "Login");            

            //if (EmailAddress.Text != "" && Password.Text != "")
            //{
            //    user = db.users.FirstOrDefault(x => x.email == EmailAddress.Text && x.password == Password.Text);

            //    if (user != null)
            //    {
            //        this.Hide();
            //        AdminPanelWindow adminpanelwindow = new AdminPanelWindow(this);
            //        adminpanelwindow.Show();
            //        MessageBox.Show("Login successfully", "Login");
            //    }
            //    else
            //    {
            //        MessageBox.Show("Email or Password are invalid", "Invalid user", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("Please fill the all field", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}                                 
        }       
    }
}
