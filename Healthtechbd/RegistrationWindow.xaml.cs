using Healthtechbd.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

            if (FirstName.Text != "First Name" && LastName.Text != "Last Name" && Phone.Text != "Phone Number" && EmailAddress.Text != "Email Address" && Password.Password != "Password")
            {
                if (IsValidEmail(EmailAddress.Text) == true)
                {
                    try
                    {
                        var haveEmail = db.users.FirstOrDefault(x => x.email == EmailAddress.Text && x.role_id == 2); // doctor
                        var havePhone = db.users.FirstOrDefault(x => x.phone == Phone.Text);

                        if (haveEmail == null)
                        {
                            if (havePhone == null)
                            {                                
                                user.role_id = 2; //Doctor role_id = 2
                                user.first_name = FirstName.Text;
                                user.last_name = LastName.Text;
                                user.email = EmailAddress.Text;
                                user.phone = Phone.Text;
                                user.password = Password.Password;
                                user.prescription_template_id = 1; //Default Prescription Template id
                                user.is_localhost = 1;
                                user.created = DateTime.Now;

                                user.expire_date = DateTime.Now.AddMonths(12).ToString("dd/MM/yyyy");

                                db.users.Add(user);
                                var status = db.SaveChanges();

                                if(status == 1) // Save Success
                                {
                                    if (CheckForInternetConnection() == true)
                                    {
                                        ApiRegistration(user.id);
                                    }
                                }

                                MainWindow.Session.doctor_id = user.id;
                                MainWindow.Session.doctor_first_name = FirstName.Text;
                                MainWindow.Session.doctor_last_name = LastName.Text;
                                MainWindow.Session.doctor_phone = Phone.Text;
                                MainWindow.Session.doctor_email = EmailAddress.Text;
                                MainWindow.Session.doctorPrescriptionTemId = user.prescription_template_id;

                                this.Hide();
                                AdminPanelWindow adminpanelWindow = new AdminPanelWindow(this);
                                adminpanelWindow.Show();

                                if (MessageBox.Show("Registration and Login is successful", "Success") == MessageBoxResult.OK)
                                {
                                    TextBlock UserName = AdminPanelWindow.userName;
                                    UserName.Text = MainWindow.Session.doctor_first_name + " " + MainWindow.Session.doctor_last_name;

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
        
        public void ApiRegistration(int id)
        {                                                                                                                                                               
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.api_base_url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var doctor = db.users.FirstOrDefault(x => x.id == id);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("first_name", doctor.first_name),
                new KeyValuePair<string, string>("last_name", doctor.last_name),
                new KeyValuePair<string, string>("phone", doctor.phone),
                new KeyValuePair<string, string>("email", doctor.email),
                new KeyValuePair<string, string>("password", doctor.password),
                new KeyValuePair<string, string>("expire_date", doctor.expire_date)
            });

            HttpResponseMessage response = client.PostAsync("admin/users/api-registration", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var patients = response.Content.ReadAsStringAsync();
                patients.Wait();

                dynamic status = JsonConvert.DeserializeObject(patients.Result);

                if(status["status"] == "success")
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