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
    /// Interaction logic for MyProfile.xaml
    /// </summary>
    public partial class MyProfile : Page
    {

        contextd_db db = new contextd_db();
        user user = new user();

        public MyProfile()
        {
            InitializeComponent();

            LoadMyProfile();
        }       
        
        void LoadMyProfile()
        {
            try
            {
                user = db.users.FirstOrDefault(x => x.id == MainWindow.Session.userId);

                FirstName.Text = user.first_name;
                LastName.Text = user.last_name;
                Email.Text = user.email;
                Phone.Text = user.phone;
                AddressLine1.Text = user.address_line1;
                AddressLine2.Text = user.address_line2;
                EducationalQualification.Text = user.educational_qualification;
                ClinicName.Text = user.clinic_name;
                Website.Text = user.website;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void SubmitMyProfile_Click(object sender, RoutedEventArgs e)
        {           
            try
            {
                NavigationService.Navigate(new Uri("MyProfile.xaml", UriKind.Relative));

                var havePhone = db.users.FirstOrDefault(x => x.phone == Phone.Text && x.id != MainWindow.Session.userId);

                if(havePhone == null)
                {
                    user = db.users.FirstOrDefault(x => x.id == MainWindow.Session.userId);

                    user.first_name = FirstName.Text;
                    user.last_name = LastName.Text;
                    //user.email = Email.Text;
                    user.phone = Phone.Text;
                    user.address_line1 = AddressLine1.Text;
                    user.address_line2 = AddressLine2.Text;
                    user.educational_qualification = EducationalQualification.Text;
                    user.clinic_name = ClinicName.Text;
                    user.website = Website.Text;

                    db.SaveChanges();

                    MessageBox.Show("Profile has been updated", "Success");
                }
                else
                {
                    MessageBox.Show("The Phone Number already exist", "Already Exit");
                }               
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void CancelMyProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }
    }
}
