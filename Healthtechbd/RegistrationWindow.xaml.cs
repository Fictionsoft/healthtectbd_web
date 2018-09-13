using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using WpfChosenControl.model;

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

        public RegistrationWindow(ResetPasswordWindow resetPasswordWindow)
        {
            InitializeComponent();
            this.resetPasswordWindow = resetPasswordWindow;
        }

        private void loginLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();            
            this.mainWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        contextd_db db = new contextd_db();
        user user = new user();
        private ResetPasswordWindow resetPasswordWindow;

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if(FirstName.Text != "Frist Name" && LastName.Text != "Last Name" && Phone.Text != "Phone Number" && EmailAddress.Text != "Email Address" && Password.Password != "Password")
            {
                //try
                //{
                    var haveEmail = db.users.FirstOrDefault(x => x.email == EmailAddress.Text);

                    if (haveEmail == null)
                    {
                        user.role_id = 1; //Doctor role_id = 1
                        user.first_name = FirstName.Text;
                        user.last_name = LastName.Text;
                        user.email = EmailAddress.Text;                       
                        user.password = Password.Password;
                        user.created = DateTime.Now;

                        db.users.Add(user);
                        db.SaveChanges();                       
                       
                        MainWindow.Session.doctorId = user.id;
                        MainWindow.Session.userFirstName = FirstName.Text;
                        MainWindow.Session.userLastName = LastName.Text;
                        MainWindow.Session.userPhone = int.Parse(Phone.Text);
                        MainWindow.Session.userEmail = EmailAddress.Text;

                        this.Hide();
                        AdminPanelWindow adminpanelwindow = new AdminPanelWindow(this);
                        adminpanelwindow.Show();

                        if (MessageBox.Show("Registration is successfull.", "Success") == MessageBoxResult.OK)
                        {
                            TextBlock UserName = AdminPanelWindow.userName;
                            UserName.Text = MainWindow.Session.userFirstName + " " + MainWindow.Session.userLastName;
                        }                        
                    }
                    else
                    {
                        MessageBox.Show("The Email already exist.", "Already Exit");
                    }
                //}
                //catch
                //{
                //    MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);                
                //}                
            }
            else
            {
                MessageBox.Show("Please fill up the all field.", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                                  
        }        

        private void FirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text == "First Name")
            {
                FirstName.Text = "";
            }
        }

        private void FirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text == "")
            {
                FirstName.Text = "First Name";
            }
        }

        private void LastName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (LastName.Text == "Last Name")
            {
                LastName.Text = "";
            }
        }

        private void LastName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (LastName.Text == "")
            {
                LastName.Text = "Last Name";
            }
        }

        private void Email_GotFocus(object sender, RoutedEventArgs e)
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

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Password.Password == "Password")
            {
                Password.Password = "";
            }
        }

        private void Password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Password.Password == "")
            {
               Password.Password = "Password";
            }
        }

        private void Phone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Phone.Text == "Phone Number")
            {
                Phone.Text = "";
            }
        }

        private void Phone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Phone.Text == "")
            {
                Phone.Text = "Phone Number";
            }
        }
    }
}
