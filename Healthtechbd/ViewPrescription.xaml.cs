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

            var test = MainWindow.Session.editRecordId;
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

        public void LoadViewPrescriptionInfo()
        {                    
            doctor = db.users.Where(x => x.id == MainWindow.Session.doctorId).FirstOrDefault();

            if(doctor.clinic_name == "")
            {
                DoctorClinicName.Visibility = Visibility.Collapsed;                
            }

            DoctorName.Text          = doctor.first_name +" "+ doctor.last_name;
            DoctorQualification.Text = doctor.educational_qualification;
            DoctorAddress.Text       = doctor.address_line1 +" "+ doctor.address_line2;
            DoctorPhone.Text         = doctor.phone;
        }
    }
}
