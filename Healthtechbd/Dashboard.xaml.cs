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
            if(patientPhone.Text.Trim() != "")
            {
                var prescription = db.presceiptions.Where(x => x.user.phone == patientPhone.Text && x.user.role_id == 3 && x.user.doctor_id == MainWindow.Session.doctorId).OrderByDescending(x => x.created).FirstOrDefault();

                if (prescription != null)
                {
                    Grid sidebar = AdminPanelWindow.sidebar;
                    sidebar.Visibility = Visibility.Hidden;

                    AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

                    int prescriptionId = prescription.id;
                    MainWindow.Session.editRecordId = prescriptionId;
                    EditPrescription editPrescription = new EditPrescription(prescriptionId);

                    EditPrescription.diagnosisTemplateIds.Clear();
                    var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescriptionId).ToList();
                    foreach (var prescriptions_diagnosi in prescriptions_diagnosis)
                    {
                        EditPrescription.diagnosisTemplateIds.Add(prescriptions_diagnosi.diagnosis_id);
                    }


                    NavigationService.Navigate(editPrescription);
                }
                else
                {
                    MainWindow.Session.editRecordId = 0;
                    AddPrescription.diagnosisTemplateIds.Clear();
                    DiagnosisTestChosenControl.selectedIds.Clear();

                    Grid sidebar = AdminPanelWindow.sidebar;
                    sidebar.Visibility = Visibility.Hidden;

                    AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

                    NavigationService.Navigate(new Uri("AddPrescription.xaml", UriKind.Relative));

                    MessageBox.Show("Patient not found, Please select a patient", "Not Found");
                }
            }                                    
        }
    }
}
