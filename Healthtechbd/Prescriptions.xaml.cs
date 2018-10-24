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
using WpfChosenControl;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for Prescriptions.xaml
    /// </summary>
    public partial class Prescriptions : Page
    {
        public Prescriptions()
        {
            InitializeComponent();

            loadPrescriptions();
        }

        contextd_db db = new contextd_db();
        prescription prescription = new prescription();

        void loadPrescriptions()
        {
            try
            {
                var prescriptions = db.presceiptions.Where(x => x.doctor_id  == MainWindow.Session.doctorId)
                                   .OrderByDescending(x => x.created).Take(10).ToList();

               dataGridPrescriptions.ItemsSource = prescriptions;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Session.editRecordId = 0;
            AddPrescription.diagnosisTemplateIds.Clear();
            DiagnosisMedicineChosenControl.selectedIds.Clear();
            DiagnosisTestChosenControl.selectedIds.Clear();

            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            NavigationService.Navigate(new Uri("AddPrescription.xaml", UriKind.Relative));
        }

        private void btnDeletePrescriptionRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int prescriptionId = (dataGridPrescriptions.SelectedItem as prescription).id;
                try
                {
                    prescription = db.presceiptions.FirstOrDefault(x => x.id == prescriptionId);

                    db.presceiptions.Remove(prescription);

                    //prescription diagnosis delete
                    var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescriptionId);
                    if (prescriptions_diagnosis.Count() > 0)
                    {
                        db.prescriptions_diagnosis.RemoveRange(prescriptions_diagnosis);
                        int delete_result = db.SaveChanges();
                    }

                    //prescription medicines delete
                    var prescriptions_medicines = db.prescriptions_medicines.Where(x => x.prescription_id == prescriptionId);
                    if (prescriptions_medicines.Count() > 0)
                    {
                        db.prescriptions_medicines.RemoveRange(prescriptions_medicines);
                        int delete_result = db.SaveChanges();
                    }

                    //prescription tests delete
                    var prescriptions_tests = db.prescriptions_tests.Where(x => x.prescription_id == prescriptionId);
                    if (prescriptions_tests.Count() > 0)
                    {
                        db.prescriptions_tests.RemoveRange(prescriptions_tests);
                        int delete_result = db.SaveChanges();
                    }

                    db.SaveChanges();
                    loadPrescriptions();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var prescriptions = db.presceiptions.Where(x => x.user.first_name.Trim().Contains(searchBy) ||
                                    x.user.phone.Trim().Contains(searchBy))
                                    .OrderByDescending(x => x.created).ToList();

                if (prescriptions.Count == 0)
                {
                    MessageBox.Show("Prescription not found");
                }
                else
                {
                    dataGridPrescriptions.ItemsSource = prescriptions;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchField.Clear();
            loadPrescriptions();
        }

        string PrescriptionTem;
        private void btnViewPrescriptionRow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Session.editRecordId = (dataGridPrescriptions.SelectedItem as prescription).id;

            int doctorPrescriptionTemId = MainWindow.Session.doctorPrescriptionTemId;
            
            if (doctorPrescriptionTemId == 1)
            {
                PrescriptionTem = "StandardTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 2)
            {
                PrescriptionTem = "ClassicTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 3)
            {
                PrescriptionTem = "CustomTemplate.xaml";
            }
            else
            {
                PrescriptionTem = "GeneralTemplate.xaml";
            }

            NavigationService.Navigate(new Uri("prescriptionTemplates/" + PrescriptionTem, UriKind.Relative));
        }

        private void btnEditPrescriptionRow_Click(object sender, RoutedEventArgs e)
        {            
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            int prescriptionId = (dataGridPrescriptions.SelectedItem as prescription).id;
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
    }
}
