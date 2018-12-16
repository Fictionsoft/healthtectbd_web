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
using WpfChosenControl;
using WpfChosenControl.model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {           
            if(patientPhone.Text != "Phone number")
            {
                var patients = db.users.Where(x => x.phone == patientPhone.Text && x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId).OrderByDescending(x => x.created).Take(40).ToList();// role_id 3 = Patient

                if (patients.Count() > 0)
                {
                    //Grid sidebar = AdminPanelWindow.sidebar;
                    //sidebar.Visibility = Visibility.Hidden;

                    //AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

                    //int prescriptionId = prescription.id;
                    //MainWindow.Session.editRecordId = prescriptionId;
                    //EditPrescription editPrescription = new EditPrescription(prescriptionId);

                    //EditPrescription.diagnosisTemplateIds.Clear();
                    //var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescriptionId).ToList();
                    //foreach (var prescriptions_diagnosi in prescriptions_diagnosis)
                    //{
                    //    EditPrescription.diagnosisTemplateIds.Add(prescriptions_diagnosi.diagnosis_id);
                    //}
                    Patients patientIndex = new Patients(patients);

                    NavigationService.Navigate(patientIndex);
                }
                else
                {                    
                    NavigationService.Navigate(new Uri("AddPatient.xaml", UriKind.Relative));

                    MessageBox.Show("Patient not found, Please create a patient", "Not Found");
                }
            }                                    
        }

        private void patientPhone_GotFocus(object sender, RoutedEventArgs e)
        {
            if (patientPhone.Text == "Phone number")
            {
                patientPhone.Text = "";
            }
        }

        private void patientPhone_LostFocus(object sender, RoutedEventArgs e)
        {
            if (patientPhone.Text == "")
            {
                patientPhone.Text = "Phone number";
            }
        }
    }
}
