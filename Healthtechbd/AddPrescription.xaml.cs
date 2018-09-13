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
        public AddPrescription()
        {
            InitializeComponent();

            LoadPatientCombobox();
            LoadDiagnosisCheckbox();

            PatientComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                   new System.Windows.Controls.TextChangedEventHandler(PatientComboBox_TextChanged));           
        }        

        contextd_db db = new contextd_db();
        prescription prescription = new prescription();
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
            patient = db.users.FirstOrDefault(x => x.first_name == PatientComboBox.Text);

            PatientPhone.Text = patient.phone;
            PatientAddress.Text = patient.address_line1;
            PatientAge.Text = patient.age;
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

            var patients = db.users.Where(x => (x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId) && (x.first_name.StartsWith(searchBy) || x.last_name.StartsWith(searchBy))).OrderByDescending(x => x.created).Take(10).ToList(); //patient_id 3
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
            var diagnosisTemplates = db.diagnosis_templates.ToList();
            foreach (var diagnosisTemplate in diagnosisTemplates)
            {
                CheckBox checkbox = new CheckBox();                                                    
                checkbox.Content = diagnosisTemplate.diagnosis.name.ToString();
                checkbox.DataContext = diagnosisTemplate.id;
                DiagnosisCheckbox.Children.Add(checkbox);

                checkbox.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(DiagnosIsChecked));
            }
        }

        private void DiagnosIsChecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox; 
            var diagnosisTemplateId = (int)checkBox.DataContext;

            var diagnosisTemplate = db.diagnosis_templates.Where(x => x.id == diagnosisTemplateId).FirstOrDefault();
            DoctorsNotes.Text = diagnosisTemplate.instructions;

            var diagnosisMedicines = db.diagnosis_medicines.Where(x => x.diagnosis_id == diagnosisTemplateId).ToList();
            var diagnosisTests = db.diagnosis_tests.Where(x => x.diagnosis_id == diagnosisTemplateId).ToList();

            DiagnosisMedicineChosenControl diagnosisMedicineChosenControl = new DiagnosisMedicineChosenControl();

            //diagnosisMedicineChosenControl.CheckBox_Click(sender, e);
            //diagnosisMedicineChosenControl._SelectedMedicines.Add(new IdNameModel() { Id = 1000, Name = "Default 1000" });
        }
        

        private void SaveAddPrescription_Click(object sender, RoutedEventArgs e)
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
            db.SaveChanges();

            MessageBox.Show("Prescription has been saved", "Success");
        }       
    }
}
