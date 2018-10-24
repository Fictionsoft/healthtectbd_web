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
    /// Interaction logic for StandardTemplate.xaml
    /// </summary>
    public partial class StandardTemplate : Page
    {
        public static Button buttonPrintPrescription;

        public StandardTemplate()
        {
            InitializeComponent();
            LoadViewPrescriptionInfo();

            buttonPrintPrescription = ButtonPrintPrescription;
        }

        private void ButtonPrintPrescription_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(PrintArea, "Print Your Prescription");
            }
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
            PatientAddressArea.Visibility = (prescription.user.address_line1 == "") ? Visibility.Collapsed : Visibility.Visible;

            //More Prescriptions
            if (prescription.user.prescription.Count() > 1)
            {
                foreach (var prescription in prescription.user.prescription.OrderBy(x => x.created))
                {
                    TextBlock textBlock = new TextBlock();

                    if(MainWindow.Session.editRecordId == prescription.id)
                    {
                        textBlock.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#3cc2bb"));
                        textBlock.TextDecorations = System.Windows.TextDecorations.Underline;
                    }

                    textBlock.Style = this.FindResource("BreadcrumbItem") as Style;
                    textBlock.DataContext = prescription.id;
                    textBlock.Text = prescription.created.ToString("dd MMM yyyy") + 
                                    (prescription.Equals(prescription.user.prescription.OrderBy(x => x.created).Last()) ? "." : ", ");

                    textBlock.AddHandler(TextBlock.MouseDownEvent, new RoutedEventHandler(MorePrescriptionClick));

                    MorePrescriptions.Children.Add(textBlock);
                }
            }


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

            PatientLastVisit.Text = db.presceiptions.Where(x => x.user_id == prescription.user.id).OrderByDescending(x => x.created).Select(x => x.created).FirstOrDefault().ToString("dd MMM yyyy");

            var prescriptions_medicines = db.prescriptions_medicines.Where(x => x.prescription_id == MainWindow.Session.editRecordId).ToList();

            //Prescription Medicines 
            foreach (var prescriptions_medicine in prescriptions_medicines)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Style = this.FindResource("Level") as Style;
                textBlock.Text = prescriptions_medicine.medicine.name +
                                (prescriptions_medicine.Equals(prescriptions_medicines.Last()) ? "." : ", ");

                PrescriptioMedicines.Children.Add(textBlock);
            }

            var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == MainWindow.Session.editRecordId).ToList();

            //Prescription Diagnosis 
            foreach (var prescriptions_diagnosi in prescriptions_diagnosis)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontWeight = FontWeights.Normal;
                textBlock.Style = this.FindResource("Level") as Style;
                textBlock.Text = prescriptions_diagnosi.diagnosis_template.diagnosis.name +
                                (prescriptions_diagnosi.Equals(prescriptions_diagnosis.Last()) ? "." : ", ");

                PatientDiagnosis.Children.Add(textBlock);
            }

            var prescriptions_tests = db.prescriptions_tests.Where(x => x.prescription_id == MainWindow.Session.editRecordId).ToList();

            //Prescription Examinations
            foreach (var prescriptions_test in prescriptions_tests)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontWeight = FontWeights.Normal;                
                textBlock.Style = this.FindResource("Level") as Style;
                textBlock.Text = prescriptions_test.test.name +
                                (prescriptions_test.Equals(prescriptions_tests.Last()) ? "." : ", ");

                PatientExamination.Children.Add(textBlock);
            }

            //Doctros Note
            DoctorsNote.Text = prescription.doctores_notes;

            //Others Note
            OthersIns.Text = prescription.other_instructions;

            PrescriptioMedicinesArea.Visibility = (prescriptions_medicines.Count() < 0) ? Visibility.Collapsed : Visibility.Visible;
            PatientDiagnosisArea.Visibility = (prescriptions_diagnosis.Count() < 0) ? Visibility.Collapsed : Visibility.Visible;
            DoctorsNoteArea.Visibility = (prescription.doctores_notes == "") ? Visibility.Collapsed : Visibility.Visible;
            OthersInsArea.Visibility = (prescription.other_instructions == "") ? Visibility.Collapsed : Visibility.Visible;


            //Singnature & Date
            DoctorSingnature.Text = doctor.first_name + " " + doctor.last_name;
            CreatedDate.Text = prescription.created.ToString("dd MMM yyyy");
        }

        private void MorePrescriptionClick(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            
            MainWindow.Session.editRecordId = int.Parse(textBlock.DataContext.ToString());

            int doctorPrescriptionTemId = MainWindow.Session.doctorPrescriptionTemId;

            Frame mainContent = AdminPanelWindow.mainContent;

            if (doctorPrescriptionTemId == 1)
            {
                mainContent.Content = new StandardTemplate();
            }
            else if (doctorPrescriptionTemId == 2)
            {
                mainContent.Content = new ClassicTemplate();
            }
            else if (doctorPrescriptionTemId == 3)
            {
                mainContent.Content = new CustomTemplate();
            }
            else
            {
                mainContent.Content = new GeneralTemplate();
            }                      
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("Prescriptions.xaml", UriKind.Relative));
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
    }
}