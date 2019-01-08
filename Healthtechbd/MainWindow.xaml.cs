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
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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
            public static int doctor_id;
            public static string doctor_first_name;                      
            public static string doctor_last_name;                      
            public static string doctor_phone;                      
            public static string doctor_email;
            public static int doctorPrescriptionTemId;
            public static int edit_record_id;

            public static string image_name;
            public static string image_path;

            public static int set_patient_id;
            public static string api_base_url = "http://app.healthtechbd.com/";
            //public static string api_base_url = "http://localhost/pms/";
            
            #endregion
        }

        public class Internet
        {
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
                        if (user.is_sync == 0 && CheckForInternetConnection() == true)
                        {
                            ApiRegister(user.id, user.first_name, user.last_name, user.email, user.phone, user.password, user.expire_date);
                        }

                        DateTime expireDate = DateTime.ParseExact(user.expire_date, "dd/MM/yyyy", null);

                        if (DateTime.Now < expireDate)
                        {
                            this.Hide();
                            AdminPanelWindow adminpanelwindow = new AdminPanelWindow(this);
                            adminpanelwindow.Show();

                            //Doctor Info Save to Session....
                            Session.doctor_id = user.id;
                            Session.doctor_first_name = user.first_name;
                            Session.doctor_last_name = user.last_name;
                            Session.doctor_email = user.email;
                            Session.doctor_phone = user.phone;
                            Session.doctorPrescriptionTemId = user.prescription_template_id;

                            if (MessageBox.Show("Login successfully", "Success") == MessageBoxResult.OK)
                            {
                                TextBlock UserName = AdminPanelWindow.userName;
                                UserName.Text = Session.doctor_first_name + " " + Session.doctor_last_name;

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

        public void ApiRegister(int id, string first_name, string last_name, string phone, string email, string password, string expire_date)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.api_base_url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var doctor = db.users.FirstOrDefault(x => x.id == id);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("first_name", first_name),
                new KeyValuePair<string, string>("last_name", last_name),
                new KeyValuePair<string, string>("phone", phone),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("expire_date", expire_date)
            });

            HttpResponseMessage response = client.PostAsync("admin/users/api-register", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var patients = response.Content.ReadAsStringAsync();
                patients.Wait();

                dynamic status = JsonConvert.DeserializeObject(patients.Result);

                if (status["status"] == "success")
                {
                    user = db.users.FirstOrDefault(x => x.id == id);
                    user.is_sync = 1;
                    db.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
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
