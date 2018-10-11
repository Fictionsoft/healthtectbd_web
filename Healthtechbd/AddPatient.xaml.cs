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
    /// Interaction logic for AddPatient.xaml
    /// </summary>
    public partial class AddPatient : Page
    {
        public AddPatient()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        user user = new user();

        private void SubmitAddPatient_Click(object sender, RoutedEventArgs e)
        {
            if(PatientName.Text != "" && PatientPhone.Text != "" && PatientAge.Text != "")
            {
                var havePhone = db.users.FirstOrDefault(x => x.phone == PatientPhone.Text && x.doctor_id == MainWindow.Session.doctorId);

                if(havePhone == null)
                {
                    NavigationService.Navigate(new Uri("Patients.xaml", UriKind.Relative));
                    user.first_name = PatientName.Text.Trim();
                    user.phone = PatientPhone.Text.Trim();
                    user.email = PatientEmail.Text.Trim();
                    user.age = PatientAge.Text.Trim();
                    user.address_line1 = PatientAddress.Text.Trim();
                    user.created = DateTime.Now;
                    user.doctor_id = MainWindow.Session.doctorId;
                    user.role_id = 3; // role_id 3 = Patient

                    db.users.Add(user);
                    try
                    {
                        db.SaveChanges();
                        clear();
                        MessageBox.Show("Patient has been saved", "Success");
                    }
                    catch
                    {
                        MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("The Phone Number already exist", "Already Exit");
                }                             
            }
            else
            {
               MessageBox.Show("Please fill in the required fields", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                      
        }

        private void CancelAddPatient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Patients.xaml", UriKind.Relative));
        }  

       void clear()
        {
            PatientName.Text = PatientPhone.Text = PatientEmail.Text = PatientAge.Text = PatientAddress.Text = "";
        }
    }
}
