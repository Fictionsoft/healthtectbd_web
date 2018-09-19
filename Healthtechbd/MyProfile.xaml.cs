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
        user doctor = new user();

        public MyProfile()
        {
            InitializeComponent();

            LoadMyProfile();
        }       
        
        void LoadMyProfile()
        {
            try
            {
                doctor = db.users.FirstOrDefault(x => x.id == MainWindow.Session.doctorId);

                FirstName.Text = doctor.first_name;
                LastName.Text = doctor.last_name;
                Email.Text = doctor.email;
                Phone.Text = doctor.phone;
                AddressLine1.Text = doctor.address_line1;
                AddressLine2.Text = doctor.address_line2;
                EducationalQualification.Text = doctor.educational_qualification;
                ClinicName.Text = doctor.clinic_name;
                Website.Text = doctor.website;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void SubmitMyProfile_Click(object sender, RoutedEventArgs e)
        {   
            if(FirstName.Text != "" && LastName.Text != "" && Phone.Text != "")
            {
                try
                {
                    NavigationService.Navigate(new Uri("MyProfile.xaml", UriKind.Relative));

                    var havePhone = db.users.FirstOrDefault(x => x.phone == Phone.Text && x.id != MainWindow.Session.doctorId);

                    if (havePhone == null)
                    {
                        doctor = db.users.FirstOrDefault(x => x.id == MainWindow.Session.doctorId);

                        doctor.first_name = FirstName.Text;
                        doctor.last_name = LastName.Text;
                        //doctor.email = Email.Text;
                        doctor.phone = Phone.Text;
                        doctor.address_line1 = AddressLine1.Text;
                        doctor.address_line2 = AddressLine2.Text;
                        doctor.educational_qualification = EducationalQualification.Text;
                        doctor.clinic_name = ClinicName.Text;
                        doctor.website = Website.Text;

                        db.SaveChanges();

                        //Change Session Info.................
                        MainWindow.Session.doctorFirstName = FirstName.Text;
                        MainWindow.Session.doctorLastName = LastName.Text;
                        MainWindow.Session.doctorPhone = Phone.Text;
                        MainWindow.Session.doctorEmail = Email.Text;

                        if (MessageBox.Show("Profile has been updated", "Success") == MessageBoxResult.OK)
                        {
                            TextBlock UserName = AdminPanelWindow.userName;
                            UserName.Text = MainWindow.Session.doctorFirstName + " " + MainWindow.Session.doctorLastName;
                        }                        
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
            else
            {
                MessageBox.Show("Please fill in the required fields", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }        
        }

        private void CancelMyProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }
    }
}
