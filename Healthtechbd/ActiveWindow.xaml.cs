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
namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for ActiveWindow.xaml
    /// </summary>
    public partial class ActiveWindow : Window
    {
        private MainWindow mainWindow;

        public ActiveWindow()
        {
            InitializeComponent();
        }

        public ActiveWindow(MainWindow mainWindow) : this()
        {
            this.mainWindow = mainWindow;
        }

        private void loginLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }       

        contextd_db db = new contextd_db();
        user user = new user();

        private void ButtonActiveApplication_Click(object sender, RoutedEventArgs e)
        {
            if (Token.Text != "Token" && EmailAddress.Text != "Email Address")
            {
                try
                {
                    user = db.users.FirstOrDefault(x => x.email == EmailAddress.Text);

                    if (user != null) //User = Doctor
                    {
                        var key = System.Convert.FromBase64String(Token.Text);
                        var emailAndDate = System.Text.Encoding.UTF8.GetString(key);

                        string[] words = emailAndDate.Split('|');
                        string email = words[0];
                        string expireDate = words[1];

                        if (email == EmailAddress.Text)
                        {
                            try
                            {
                                DateTime loadedDate = DateTime.ParseExact(expireDate, "dd/MM/yyyy", null);

                                user.expire_date = expireDate; // words[1] = Expire Date
                                db.SaveChanges();

                                Token.Text = EmailAddress.Text = "";// Clear Fields

                                if (MessageBox.Show("Application has been activated. You can login now.", "Success") == MessageBoxResult.OK)
                                {
                                    this.Hide();
                                    MainWindow mainWindow = new MainWindow(this);
                                    mainWindow.Show();
                                }
                            }
                            catch
                            {
                                MessageBox.Show("Your key is Invalid", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Your email and key didn't not match", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Your email not found", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch
                {
                    MessageBox.Show("Your key is Invalid", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please fill the all field", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Token_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Token.Text == "Token")
            {
                Token.Text = "";
            }
        }

        private void Token_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Token.Text == "")
            {
                Token.Text = "Token";
            }
        }

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
    }
}
