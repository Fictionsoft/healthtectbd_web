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
using WpfChosenControl.model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Page
    {
        public ChangePassword()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        user user = new user();

        private void SubmitChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if(Password.Password != "" && ConfirmPassword.Password != "")
            {
                if(Password.Password == ConfirmPassword.Password)
                {
                    try
                    {
                        user = db.users.FirstOrDefault(x => x.id == MainWindow.Session.doctor_id);

                        user.password = Password.Password;
                        db.SaveChanges();

                        NavigationService.Navigate(new Uri("ChangePassword.xaml", UriKind.Relative));
                        MessageBox.Show("Password has been changed", "Success");
                    }
                    catch
                    {
                        MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Confirm Password didn't match with Password.", "Discrepancy", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                              
            }
            else
            {
                MessageBox.Show("Please fill up the all field.", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                                  
        }

        private void CancelChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }
    }
}
