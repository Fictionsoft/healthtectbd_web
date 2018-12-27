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
    /// Interaction logic for EditPatient.xaml
    /// </summary>
    public partial class EditPatient : Page
    {
        public EditPatient()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        user patient = new user();
        prescription prescription = new prescription();

        public EditPatient(int id) : this()
        {
            PatientId.Text = id.ToString(); //Patient = User

            try
            {
                patient = db.users.FirstOrDefault(x => x.id == id);
                PatientName.Text = patient.first_name;
                PatientPhone.Text = patient.phone;
                PatientAge.Text = patient.age;
                PatientEmail.Text = patient.email;
                PatientAddress.Text = patient.address_line1;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void CancelUpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Patients.xaml", UriKind.Relative));
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

        private void SubmitUpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            Regex pattern = new Regex("[-]");
            var patientName = pattern.Replace(PatientName.Text, " ");
            var patientPhone = pattern.Replace(PatientPhone.Text, " ");

            if (patientName != "" && patientPhone != "" && PatientAge.Text != "")
            {
                if ((PatientEmail.Text != "" && IsValidEmail(PatientEmail.Text) == true) || PatientEmail.Text == "")
                {
                    int patientId = int.Parse(PatientId.Text);
                  
                    var havePhone = db.users.FirstOrDefault(x => x.first_name == PatientName.Text && x.phone == PatientPhone.Text && x.id != patientId && x.doctor_id == MainWindow.Session.doctor_id);

                    if (havePhone == null)
                    {
                        patient = db.users.FirstOrDefault(x => x.id == patientId);

                        patient.first_name = patientName;
                        patient.phone = patientPhone;
                        patient.email = PatientEmail.Text.Trim();
                        patient.age = PatientAge.Text.Trim();
                        patient.address_line1 = PatientAddress.Text.Trim();

                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                        NavigationService.Navigate(new Uri("Patients.xaml", UriKind.Relative));
                        MessageBox.Show("Update Successfully", "Success");                       
                    }
                    else
                    {
                        MessageBox.Show("The Phone already exist.", "Already Exit");
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
    }
}
