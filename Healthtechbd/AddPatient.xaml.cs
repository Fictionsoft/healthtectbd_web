using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        user patient = new user();

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

        private void SubmitAddPatient_Click(object sender, RoutedEventArgs e)
        {
            Regex pattern = new Regex("[-]");
            var patientName = pattern.Replace(PatientName.Text, " ");
            var patientPhone = pattern.Replace(PatientPhone.Text, " ");

            if (patientName != "" && patientPhone != "" && PatientAge.Text != "")
            {                                
                if ((PatientEmail.Text != "" && IsValidEmail(PatientEmail.Text) == true) || PatientEmail.Text == "")
                {
                    var havePhone = db.users.FirstOrDefault(x => x.first_name == PatientName.Text && x.phone == PatientPhone.Text && x.doctor_id == MainWindow.Session.doctor_id);

                    if (havePhone == null)
                    {                       
                        NavigationService.Navigate(new Uri("Patients.xaml", UriKind.Relative));
                        patient.doctor_id = MainWindow.Session.doctor_id;
                        patient.role_id = 3; // role_id 3 = Patient
                        patient.first_name = patientName;
                        patient.phone = patientPhone;
                        patient.email = PatientEmail.Text;
                        patient.age = PatientAge.Text;
                        patient.address_line1 = PatientAddress.Text;
                        patient.created = DateTime.Now;                      
                        patient.expire_date = "00/00/0000";
                        patient.is_sync = 1;

                        db.users.Add(patient);
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
                        MessageBox.Show("The patient already exit.", "Already Exit");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid email.", "Invalid");
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
