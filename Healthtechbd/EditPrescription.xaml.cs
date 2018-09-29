using Healthtechbd.Model;
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
    /// Interaction logic for EditPrescription.xaml
    /// </summary>
    public partial class EditPrescription : Page
    {
        private DiagnosisMedicineChosenControl diagnosisMedicineChosenControl = null;
        private DiagnosisTestChosenControl diagnosisTestChosenControl = null;

        public EditPrescription()
        {
            InitializeComponent();

            diagnosisMedicineChosenControl = (DiagnosisMedicineChosenControl)FindName("medicineChosen");
            diagnosisTestChosenControl = (DiagnosisTestChosenControl)FindName("testChosen");

            LoadPatientCombobox();
            LoadDiagnosisCheckbox();

            PatientComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                   new System.Windows.Controls.TextChangedEventHandler(PatientComboBox_TextChanged));
        }

        contextd_db db = new contextd_db();
        prescription prescription = new prescription();
        prescriptions_diagnosis prescriptions_diagnosi = new prescriptions_diagnosis();
        prescriptions_medicine prescriptions_medicine = new prescriptions_medicine();
        prescriptions_test prescriptions_test = new prescriptions_test();
        user patient = new user();

        public EditPrescription(int id) : this()
        {
            PrescriptionId.Text = id.ToString();

            try
            {
                var prescription = db.presceiptions.FirstOrDefault(x => x.id == id);
                PatientComboBox.SelectedItem = prescription.user.first_name;
                PatientPhone.Text = prescription.user.phone;
                PatientAddress.Text = prescription.user.address_line1;
                PatientAge.Text = prescription.user.age;

                BloodPresure.Text = prescription.blood_pressure;
                Temperature.Text = prescription.temperature;

                DoctorsNotes.Text = prescription.doctores_notes;
                OtherInstructions.Text = prescription.other_instructions;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PatientComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComboBox obj = sender as ComboBox;

            var searchBy = obj.Text;

            var patients = db.users.Where(x => (x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId) &&
                           (x.first_name.Contains(searchBy))).OrderByDescending(x => x.created).Take(10).ToList(); //patient_id 3

            PatientComboBox.Items.Clear();

            foreach (var patient in patients)
            {
                PatientComboBox.Items.Add(patient.first_name);
            }

            if (patients.Count == 0)
            {
                PatientComboBox.Items.Add("No results mached with " + searchBy);
            }
        }

        private void PatientComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PatientComboBox.IsDropDownOpen = true;
        }

        //Patient Combobox DropDown Closed..........
        private void PatientComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                patient = db.users.FirstOrDefault(x => x.first_name == PatientComboBox.Text);

                if (patient != null)
                {
                    PatientPhone.Text = patient.phone;
                    PatientAddress.Text = patient.address_line1;
                    PatientAge.Text = patient.age;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //Load Patient Combobox.....
        void LoadPatientCombobox()
        {
            try
            {
                var patients = db.users.Where(x => x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId).OrderByDescending(x => x.created).Take(10).ToList(); //patient_id 3

                foreach (var patient in patients)
                {
                    PatientComboBox.Items.Add(patient.first_name);
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //Load Diagnosis CheckBox.....
        void LoadDiagnosisCheckbox()
        {
            var diagnosisTemplates = db.diagnosisTemplates.ToList();
            foreach (var diagnosisTemplate in diagnosisTemplates)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = diagnosisTemplate.diagnosis.name.ToString();
                checkbox.DataContext = diagnosisTemplate.id;
                DiagnosisCheckbox.Children.Add(checkbox);

                var exitingDiagnosisTemplate = db.prescriptions_diagnosis
                           .Where(x => x.diagnosis_id == diagnosisTemplate.id && x.prescription_id == MainWindow.Session.editRecordId)
                           .FirstOrDefault();

                if (exitingDiagnosisTemplate != null)
                {
                    checkbox.IsChecked = true;
                }
               
                checkbox.AddHandler(CheckBox.ClickEvent, new RoutedEventHandler(DiagnosIsClick));
            }
        }

        public static List<long> diagnosisTemplateIds = new List<long>();
        private void DiagnosIsClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            var diagnosisTemplateId = (int)checkBox.DataContext;

            if (checkBox.IsChecked == true)
            {
                diagnosisTemplateIds.Add(diagnosisTemplateId);
            }

            if (checkBox.IsChecked == false)
            {
                diagnosisTemplateIds.Remove(diagnosisTemplateId);
            }

            var diagnosisTemplates = db.diagnosisTemplates
                .Where(x => diagnosisTemplateIds.Contains(x.id))
                .Select(x => x.instructions).ToList();

            var instructions = "";
            foreach (var instruction in diagnosisTemplates)
            {
                instructions += instruction + (instruction.Equals(diagnosisTemplates.Last()) ? "." : ", ");
            }

            DoctorsNotes.Text = instructions;

            var diagnosisMedicines = db.diagnosis_medicines.Where(x => diagnosisTemplateIds.Contains(x.diagnosis_id))
                .Select(x => new IdNameModel
                {
                    Id = x.medicine_id,
                    Name = x.medicine.name
                })
                .ToList();

            var diagnosisTests = db.diagnosis_tests.Where(x => diagnosisTemplateIds.Contains(x.diagnosis_id))
                .Select(x => new IdNameModel
                {
                    Id = x.test_id,
                    Name = x.test.name
                })
                .ToList();

            ((DiagnosisMedicineModel)diagnosisMedicineChosenControl.DataContext).SelectedMedicines = diagnosisMedicines;
            ((DiagnosisTestModel)diagnosisTestChosenControl.DataContext).SelectedTests = diagnosisTests;

            // store mediciene ids to save into database
            DiagnosisMedicineChosenControl.selectedIds.Clear();
            foreach (var diagnosisMedicine in diagnosisMedicines)
            {
                DiagnosisMedicineChosenControl.selectedIds.Add(diagnosisMedicine.Id);
                diagnosisMedicineChosenControl._nodeList.Add(new Node(new IdNameModel() { Id = diagnosisMedicine.Id, Name = diagnosisMedicine.Name }));
            }

            // store test ids to save into database
            DiagnosisTestChosenControl.selectedIds.Clear();
            foreach (var diagnosisTest in diagnosisTests)
            {
                DiagnosisTestChosenControl.selectedIds.Add(diagnosisTest.Id);
                diagnosisTestChosenControl._nodeList.Add(new Node(new IdNameModel() { Id = diagnosisTest.Id, Name = diagnosisTest.Name }));
            }
        }
    }
}
