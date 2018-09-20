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
    /// Interaction logic for AddPrescription.xaml
    /// </summary>
    public partial class AddPrescription : Page
    {
        private DiagnosisMedicineChosenControl diagnosisMedicineChosenControl = null;
        private DiagnosisTestChosenControl diagnosisTestChosenControl = null;

        public AddPrescription()
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

        //Patient Combobox DropDown Closed..........
        private void PatientComboBox_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                patient = db.users.FirstOrDefault(x => x.first_name == PatientComboBox.Text);

                PatientPhone.Text = patient.phone;
                PatientAddress.Text = patient.address_line1;
                PatientAge.Text = patient.age;
            }
            catch
            {

            }            
        }

        private void NewPatientCheck(object sender, RoutedEventArgs e)
        {
            NewPatientName.Visibility = Visibility.Visible;
            PatientComboBox.Visibility = Visibility.Hidden;
        }

        private void NewPatientUncheck(object sender, RoutedEventArgs e)
        {
            NewPatientName.Visibility = Visibility.Hidden;
            PatientComboBox.Visibility = Visibility.Visible;
        }

        private void PatientComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PatientComboBox.IsDropDownOpen = true;
        }

        private void PatientComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PatientComboBox.IsDropDownOpen = true;

            ComboBox obj = sender as ComboBox;

            var searchBy = obj.Text;

            var patients = db.users.Where(x => (x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId) && 
                           (x.first_name.StartsWith(searchBy))).OrderByDescending(x => x.created).Take(10).ToList(); //patient_id 3

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

                checkbox.AddHandler(CheckBox.ClickEvent, new RoutedEventHandler(DiagnosIsClick));
            }
        }

        public static List<long> diagnosisTemplateIds = new List<long>();
        private void DiagnosIsClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
          
            var diagnosisTemplateId = (int)checkBox.DataContext;

            if(checkBox.IsChecked == true)
            {
                diagnosisTemplateIds.Add(diagnosisTemplateId);
            }

            if(checkBox.IsChecked == false)
            {
                diagnosisTemplateIds.Remove(diagnosisTemplateId);
            }

            var diagnosisTemplates = db.diagnosisTemplates
                .Where(x => diagnosisTemplateIds.Contains(x.id))
                .Select(x => x.instructions).ToList();

            var instructions = "";
            foreach (var instruction in diagnosisTemplates)
            {
                instructions += instruction + (instruction.Equals(diagnosisTemplates.Last()) ?".":", ");
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
            MedicineChosenControl.selectedIds.Clear();
            foreach (var diagnosisMedicine in diagnosisMedicines)
            {
                MedicineChosenControl.selectedIds.Add(diagnosisMedicine.Id);
            }

            // store test ids to save into database
            TestChosenControl.selectedIds.Clear();
            foreach (var diagnosisTest in diagnosisTests)
            {
                TestChosenControl.selectedIds.Add(diagnosisTest.Id);
            }
        }        

        private void SaveAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            if(PatientComboBox.Text != "" && PatientPhone.Text != "")
            {
                Grid sidebar = AdminPanelWindow.sidebar;
                sidebar.Visibility = Visibility.Visible;

                AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigationbar).            

                NavigationService.Navigate(new Uri("Prescriptions.xaml", UriKind.Relative));

                patient = db.users.FirstOrDefault(x => x.first_name == PatientComboBox.Text);

                prescription.user_id = patient.id;
                prescription.doctor_id = MainWindow.Session.doctorId; //doctorId = doctor_id
                prescription.blood_pressure = BloodPresure.Text;
                prescription.temperature = Temperature.Text;
                prescription.doctores_notes = DoctorsNotes.Text;
                prescription.other_instructions = OtherInstructions.Text;
                prescription.status = true;
                prescription.created = DateTime.Now;

                db.presceiptions.Add(prescription);
                int result_add_prescription = db.SaveChanges();  
                
                if(result_add_prescription > 0)
                {
                    //Add Prescription Diagnosis
                    AddPrescriptionDiagnosis(prescription.id);

                    //Add Prescription Medicines
                    AddPrescriptionMedicines(prescription.id);

                    //Add Prescription Tests
                    AddPrescriptionTests(prescription.id);
                }

                MessageBox.Show("Prescription has been saved", "Success");
            }
            else
            {
                MessageBox.Show("Please fill in the required fields", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        void AddPrescriptionDiagnosis(int PrescriptionId)
        {
            //prescription diagnosis delete
            var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == PrescriptionId);
            if (prescriptions_diagnosis.Count() > 0)
            {
                db.prescriptions_diagnosis.RemoveRange(prescriptions_diagnosis);
                int delete_result = db.SaveChanges();
            }

            //prescription diagnosis add            
            foreach (int diagnosisTemplateId in diagnosisTemplateIds)
            {
                prescriptions_diagnosi.prescription_id = PrescriptionId;
                prescriptions_diagnosi.diagnosis_id = diagnosisTemplateId;
                prescriptions_diagnosi.status = true;
                prescriptions_diagnosi.created = DateTime.Now;
                db.prescriptions_diagnosis.Add(prescriptions_diagnosi);
                int retult_prescription_diagnosis = db.SaveChanges();
            }

            diagnosisTemplateIds.Clear();
            MedicineChosenControl.selectedIds.Clear();
        }

        void AddPrescriptionMedicines(int PrescriptionId)
        {
            //prescription medicines delete
            var prescriptions_medicines = db.prescriptions_medicines.Where(x => x.prescription_id == PrescriptionId);
            if (prescriptions_medicines.Count() > 0)
            {
                db.prescriptions_medicines.RemoveRange(prescriptions_medicines);
                int delete_result = db.SaveChanges();
            }

            //prescription medicines add
            var medicinesIds = MedicineChosenControl.selectedIds;
            foreach (int medicine_id in medicinesIds)
            {
                prescriptions_medicine.prescription_id = PrescriptionId;
                prescriptions_medicine.medicine_id = medicine_id;
                prescriptions_medicine.status = true;
                prescriptions_medicine.created = DateTime.Now;
                db.prescriptions_medicines.Add(prescriptions_medicine);
                int retult_prescription_medecines = db.SaveChanges();
            }

            diagnosisTemplateIds.Clear();
            MedicineChosenControl.selectedIds.Clear();
        }

        void AddPrescriptionTests(int PrescriptionId)
        {
            //prescription tests delete
            var prescriptions_tests = db.prescriptions_tests.Where(x => x.prescription_id == PrescriptionId);
            if (prescriptions_tests.Count() > 0)
            {
                db.prescriptions_tests.RemoveRange(prescriptions_tests);
                int delete_result = db.SaveChanges();
            }

            //prescription tests add
            var testsIds = TestChosenControl.selectedIds;
            foreach (int test_id in testsIds)
            {
                prescriptions_test.prescription_id = PrescriptionId;
                prescriptions_test.test_id = test_id;
                prescriptions_test.status = true;
                prescriptions_test.created = DateTime.Now;
                db.prescriptions_tests.Add(prescriptions_test);
                int retult_prescription_tests = db.SaveChanges();
            }

            diagnosisTemplateIds.Clear();
            TestChosenControl.selectedIds.Clear();
        }

        private void CancelAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Visible;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigationbar).            

            NavigationService.Navigate(new Uri("Prescriptions.xaml", UriKind.Relative));
        }
    }
}
