using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

            if (FirstName.Text != "Frist Name" && LastName.Text != "Last Name" && Phone.Text != "Phone Number" && EmailAddress.Text != "Email Address" && Password.Password != "Password")
            {
                if(IsValidEmail(EmailAddress.Text) == true)
                {
                    try
                    {
                        var haveEmail = db.users.FirstOrDefault(x => x.email == EmailAddress.Text);
                        var havePhone = db.users.FirstOrDefault(x => x.phone == Phone.Text);

                        if (haveEmail == null)
                        {
                            if (havePhone == null)
                            {
                                if (CheckForInternetConnection() == true)
                                {
                                    apiRegister();
                                    user.is_sync = 1;
                                }

                                user.role_id = 2; //Doctor role_id = 2
                                user.first_name = FirstName.Text;
                                user.last_name = LastName.Text;
                                user.email = EmailAddress.Text;
                                user.phone = Phone.Text;
                                user.password = Password.Password;
                                user.prescription_template_id = 1; //Default Prescription Template id
                                user.is_localhost = 1;
                                user.created = DateTime.Now;

                                user.expire_date = DateTime.Now.AddYears(1).ToString("dd/MM/yyyy");

                                db.users.Add(user);
                                db.SaveChanges();

                                MainWindow.Session.doctorId = user.id;
                                MainWindow.Session.doctorFirstName = FirstName.Text;
                                MainWindow.Session.doctorLastName = LastName.Text;
                                MainWindow.Session.doctorPhone = Phone.Text;
                                MainWindow.Session.doctorEmail = EmailAddress.Text;
                                MainWindow.Session.doctorPrescriptionTemId = user.prescription_template_id;

                                this.Hide();
                                AdminPanelWindow adminpanelWindow = new AdminPanelWindow(this);
                                adminpanelWindow.Show();

                                if (MessageBox.Show("Registration and Login is successfull", "Success") == MessageBoxResult.OK)
                                {
                                    TextBlock UserName = AdminPanelWindow.userName;
                                    UserName.Text = MainWindow.Session.doctorFirstName + " " + MainWindow.Session.doctorLastName;

                                    Image ProfilePic = AdminPanelWindow.profilePic;

                                    if (user.profile_picture != null)
                                    {
                                        ProfilePic.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "images/" + user.profile_picture));
                                    }
                                    else
                                    {
                                        ProfilePic.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "images/defaultProfilePicture.png"));
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("The Phone Number already exist.", "Already Exit");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The Email already exist.", "Already Exit");
                        }
                    }
                    catch
                    {
                        MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid email.", "Invalid");
                }                
            }
            else
            {
                MessageBox.Show("Please fill up the all field.", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void apiRegister()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://app.healthtechbd.com/admin/users/apiRegistration");
            request.Method = "POST";

            string postData = string.Format(
                                            "first_name=" + FirstName.Text  + "&last_name=" + LastName.Text +
                                            "&email=" + EmailAddress.Text + "&phone=" + Phone.Text +
                                            "&password=" + Password.Password
                                           );

            byte[] data = Encoding.UTF8.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "application/json";
            request.ContentLength = data.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    // Do something with response

                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                // Handle error
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
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
