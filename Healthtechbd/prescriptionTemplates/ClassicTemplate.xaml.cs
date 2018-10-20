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

namespace Healthtechbd.prescriptionTemplates
{
    /// <summary>
    /// Interaction logic for ClassicTemplate.xaml
    /// </summary>
    public partial class ClassicTemplate : Page
    {
        public ClassicTemplate()
        {
            InitializeComponent();
            LoadViewPrescriptionInfo();
        }

        contextd_db db = new contextd_db();
        user doctor = new user();
        prescription prescription = new prescription();
        prescriptions_medicine prescriptions_medicine = new prescriptions_medicine();
        prescriptions_test prescriptions_test = new prescriptions_test();

        public void LoadViewPrescriptionInfo()
        {
            doctor = db.users.Where(x => x.id == MainWindow.Session.doctorId).FirstOrDefault();
            prescription = db.presceiptions.Where(x => x.id == MainWindow.Session.editRecordId).FirstOrDefault();

            DoctorQualification.Visibility = (doctor.educational_qualification == "") ? Visibility.Collapsed : Visibility.Visible;
            DoctorAddress.Visibility = (doctor.address_line1 == "") ? Visibility.Collapsed : Visibility.Visible;
            DoctorClinicName.Visibility = (doctor.clinic_name == "") ? Visibility.Collapsed : Visibility.Visible;
            DoctorWebsite.Visibility = (doctor.website == "") ? Visibility.Collapsed : Visibility.Visible;

            //Doctor Info
            DoctorName.Text = doctor.first_name + " " + doctor.last_name;
            DoctorQualification.Text = doctor.educational_qualification;
            DoctorAddress.Text = doctor.address_line1 + " " + doctor.address_line2;
            DoctorClinicName.Text = doctor.clinic_name;
            DoctorWebsite.Text = doctor.website;
            DoctorPhone.Text = doctor.phone;

            //Patient Info
            PatientName.Text = prescription.user.first_name;
            PatientAge.Text = prescription.user.age;
            PatientPhone.Text = prescription.user.phone;
            PatientAddress.Text = prescription.user.address_line1;

            var prescriptions_medicines = db.prescriptions_medicines.Where(x => x.prescription_id == MainWindow.Session.editRecordId).ToList();

            //Prescription Medicines 
            var i = 1;
            foreach (var prescriptions_medicine in prescriptions_medicines)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontWeight = FontWeights.Normal;
                textBlock.Style = this.FindResource("Level") as Style;
                textBlock.Text = i + ". " + prescriptions_medicine.medicine.name;

                PrescriptioMedicines.Children.Add(textBlock);
                i++;
            }

            var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == MainWindow.Session.editRecordId).ToList();

            //Prescription Diagnosis 
            var j = 1;
            foreach (var prescriptions_diagnosi in prescriptions_diagnosis)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontWeight = FontWeights.Normal;
                textBlock.Style = this.FindResource("Level") as Style;
                textBlock.Text = j + ". " + prescriptions_diagnosi.diagnosis_template.diagnosis.name;

                PatientDiagnosis.Children.Add(textBlock);
                j++;
            }

            var prescriptions_tests = db.prescriptions_tests.Where(x => x.prescription_id == MainWindow.Session.editRecordId).ToList();

            //Prescription Examinations
            var k = 1;
            foreach (var prescriptions_test in prescriptions_tests)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontWeight = FontWeights.Normal;
                textBlock.Style = this.FindResource("Level") as Style;
                textBlock.Text = k + ". " + prescriptions_test.test.name;

                PatientExamination.Children.Add(textBlock);
                k++;
            }

            //Doctros Note
            DoctorsNote.Text = prescription.doctores_notes;
          
            PrescriptioMedicinesArea.Visibility = (prescriptions_medicines.Count() < 0) ? Visibility.Collapsed : Visibility.Visible;
            DoctorsNoteArea.Visibility = (prescription.doctores_notes == "") ? Visibility.Collapsed : Visibility.Visible;


            //Singnature & Date
            CreatedDate.Text = prescription.created.ToString("dd MMM yyyy");
        }


        private void ButtonPrintPrescription_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(PrintArea, "Print Your Prescription");
            }
        }

        private void ButtonEditPrescription_Click(object sender, RoutedEventArgs e)
        {
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            int prescriptionId = MainWindow.Session.editRecordId;
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

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("Prescriptions.xaml", UriKind.Relative));
        }
    }

}