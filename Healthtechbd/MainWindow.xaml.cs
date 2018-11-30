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
using System.Security.Cryptography;
using WpfChosenControl.model;
using System.Net;
using System.IO;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Custom Session.............................
        public class Session
        {
            #region Doctor Info
            public static int doctorId;
            public static string doctorFirstName;                      
            public static string doctorLastName;                      
            public static string doctorPhone;                      
            public static string doctorEmail;
            public static int doctorPrescriptionTemId;
            public static int editRecordId;

            public static string imageName;
            public static string imagePath;

            public static int setPatientId;
            #endregion
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(ResetPasswordWindow resetPasswordWindow) : this()
        {
            this.resetPasswordWindow = resetPasswordWindow;
        }

        public MainWindow(ActiveWindow activeWindow) : this()
        {
            this.activeWindow = activeWindow;
        }

        contextd_db db = new contextd_db();
        user user = new user();
        private ResetPasswordWindow resetPasswordWindow;
        private ActiveWindow activeWindow;

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
            if (EmailAddress.Text != "Email Address" && Password.Password != "Password")
            {
                try
                {
                    user = db.users.FirstOrDefault(x => x.email == EmailAddress.Text && x.password == Password.Password && x.role_id == 2); // Doctor role_id = 2 

                    if (user != null) //User = Doctor
                    {
                        if(user.is_sync == 0 && CheckForInternetConnection() == true)
                        {
                            apiRegister(user.first_name, user.last_name, user.email, user.phone, user.password);

                            user.is_sync = 1;
                            db.SaveChanges();
                        }

                        DateTime expireDate = DateTime.ParseExact(user.expire_date, "dd/MM/yyyy", null);

                        if (DateTime.Now < expireDate)
                        {
                            this.Hide();
                            AdminPanelWindow adminpanelwindow = new AdminPanelWindow(this);
                            adminpanelwindow.Show();

                            //Doctor Info Save to Session....
                            Session.doctorId = user.id;
                            Session.doctorFirstName = user.first_name;
                            Session.doctorLastName = user.last_name;
                            Session.doctorEmail = user.email;
                            Session.doctorPhone = user.phone;
                            Session.doctorPrescriptionTemId = user.prescription_template_id;

                            if (MessageBox.Show("Login successfully", "Success") == MessageBoxResult.OK)
                            {
                                TextBlock UserName = AdminPanelWindow.userName;
                                UserName.Text = Session.doctorFirstName + " " + Session.doctorLastName;

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
                            if (MessageBox.Show("Your registration has been expired, Please contact with Admin", "Expired") == MessageBoxResult.OK)
                            {
                                activeSection.Visibility = Visibility.Visible;                         
                            }
                        }                       
                    }
                    else
                    {
                        MessageBox.Show("Email or Password are invalid", "Invalid User", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please fill the all field", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void apiRegister(string firstName, string lastName, string email, string phone, string password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://app.healthtechbd.com/admin/users/apiRegistration");
            request.Method = "POST";

            string postData = string.Format(
                                            "first_name=" + firstName + "&last_name=" + lastName +
                                            "&email=" + email + "&phone=" + phone +
                                            "&password=" + password
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

        //Placeholder.................................................
        private void EmailAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if(EmailAddress.Text == "Email Address")
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
            if(Password.Password == "Password")
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

        private void activeLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            ActiveWindow activeWindow = new ActiveWindow(this);
            activeWindow.Show();
        }
    }
}
