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
    /// Interaction logic for ViewPrescription.xaml
    /// </summary>
    public partial class ViewPrescription : Page
    {
        public ViewPrescription()
        {
            InitializeComponent();
            LoadViewPrescriptionInfo();
        }

        private void ButtonPrintPrescription_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();           

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(PrintArea, "My First Print Job");
            }
        }

        contextd_db db = new contextd_db();
        user doctor = new user();
        prescription prescription = new prescription();

        public void LoadViewPrescriptionInfo()
        {                    
            doctor = db.users.Where(x => x.id == MainWindow.Session.doctorId).FirstOrDefault();
            prescription = db.presceiptions.Where(x => x.id == MainWindow.Session.editRecordId).FirstOrDefault();

            DoctorQualification.Visibility = (doctor.educational_qualification == "") ? Visibility.Collapsed : Visibility.Visible;
            DoctorAddress.Visibility = (doctor.address_line1 == "") ? Visibility.Collapsed : Visibility.Visible;
            DoctorClinicName.Visibility = (doctor.clinic_name == "") ? Visibility.Collapsed : Visibility.Visible;
            DoctorWebsite.Visibility = (doctor.website == "") ? Visibility.Collapsed : Visibility.Visible;

            //Doctor Info
            DoctorName.Text          = doctor.first_name +" "+ doctor.last_name;
            DoctorQualification.Text = doctor.educational_qualification;
            DoctorAddress.Text       = doctor.address_line1 +" "+ doctor.address_line2;
            DoctorClinicName.Text    = doctor.clinic_name; 
            DoctorWebsite.Text       = doctor.website; 
            DoctorPhone.Text         = doctor.phone;

            //Patient Info
            PatientName.Text = prescription.user.first_name;
            PatientAge.Text = prescription.user.age;
            PatientPhone.Text = prescription.user.phone;
            PatientAddress.Text = prescription.user.address_line1;

            //Singnature & Date
            DoctorSingnature.Text = doctor.first_name + " " + doctor.last_name;
            CreatedDate.Text = prescription.created.ToString();
        }
    }
}
