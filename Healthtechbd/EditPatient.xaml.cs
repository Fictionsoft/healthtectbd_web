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
    /// Interaction logic for EditPatient.xaml
    /// </summary>
    public partial class EditPatient : Page
    {
        public EditPatient()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        user user = new user();

        public EditPatient(int id) : this()
        {
            PatientId.Text = id.ToString(); //Patient = User

            try
            {
                user = db.users.FirstOrDefault(x => x.id == id);
                PatientName.Text = user.first_name;
                PatientPhone.Text = user.phone;
                PatientAge.Text = user.age;
                PatientEmail.Text = user.email;
                PatientAddress.Text = user.address_line1;
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

        private void SubmitUpdatePatient_Click(object sender, RoutedEventArgs e)
        {
            if (PatientName.Text != "" && PatientPhone.Text != "" && PatientAge.Text != "")
            {
                int patientId = int.Parse(PatientId.Text);

                try
                {
                    var havePhone = db.users.FirstOrDefault(x => x.phone == PatientPhone.Text && x.id != patientId);

                    if(havePhone == null)
                    {                                        
                        user = db.users.FirstOrDefault(x => x.id == patientId);

                        user.first_name = PatientName.Text.Trim();
                        user.phone = PatientPhone.Text.Trim();
                        user.email = PatientEmail.Text.Trim();
                        user.age = PatientAge.Text.Trim();
                        user.address_line1 = PatientAddress.Text.Trim();

                        db.SaveChanges();

                        NavigationService.Navigate(new Uri("Patients.xaml", UriKind.Relative));
                        MessageBox.Show("Update Successfully", "Success");                   
                    }
                    else
                    {
                        MessageBox.Show("The Phone already exist.", "Already Exit");
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please fill in the required fields", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
