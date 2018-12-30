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
        private DiagnosisTestChosenControl diagnosisTestChosenControl = null;

        public EditPrescription()
        {
            InitializeComponent();

            diagnosisTestChosenControl = (DiagnosisTestChosenControl)FindName("testChosen");

            LoadPatientCombobox();
            LoadDiagnosisCheckbox();

            LoadMedicines();

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
            prescription_id.Text = id.ToString();

            try
            {
                //To Selected this prescription User 
                var prescription = db.prescriptions.FirstOrDefault(x => x.id == id);
                PatientComboBox.SelectedItem = prescription.user.first_name + "-" + prescription.user.phone;                

                PatientPhone.Text = prescription.user.phone;
                PatientAddress.Text = prescription.user.address_line1;
                PatientAge.Text = prescription.user.age;

                BloodPresure.Text = prescription.blood_pressure;
                Temperature.Text = prescription.temperature;                       
                DoctorsNotes.Text = prescription.doctores_notes;
                OtherInstructions.Text = prescription.other_instructions;

                //All prescription link
                AllPrescription.Children.Clear();
                if (prescription.user.prescription.Count() > 0)
                {
                    PatientLastVisit.Text = prescription.user.prescription.Last().created.ToString("dd MMM yyyy");

                    foreach (var item in prescription.user.prescription)
                    {
                        TextBlock textBlock = new TextBlock();
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                        textBlock.Padding = new Thickness(10, 5, 10, 5);
                        textBlock.DataContext = item.id;
                        textBlock.Text = item.created.ToString("dd MMM yyyy");

                        textBlock.AddHandler(TextBlock.MouseDownEvent, new RoutedEventHandler(AllPrescriptionClick));

                        AllPrescription.Children.Add(textBlock);
                    }
                }
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

            var patients = db.users.Where(x => (x.role_id == 3 && x.doctor_id == MainWindow.Session.doctor_id) && 
                           (x.first_name.StartsWith(searchBy))).Take(10).ToList(); //patient_id 3

            PatientComboBox.Items.Clear();

            foreach (var patient in patients)
            {
                PatientComboBox.Items.Add(patient.first_name + "-" + patient.phone);
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
            if (PatientComboBox.Text != "")
            {
                try
                {
                    string[] words = PatientComboBox.Text.Split('-');
                    string patientName = words[0];
                    string patientPhone = words[1];

                    int patientId = db.users.Where(x => x.first_name == patientName && x.phone == patientPhone && x.role_id == 3 && x.doctor_id == MainWindow.Session.doctor_id).Select(x => x.id).FirstOrDefault();

                    FillUpPatientInfo(patientId);
                }
                catch
                {

                }
            }
        }

        private void AllPrescriptionClick(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            MainWindow.Session.edit_record_id = int.Parse(textBlock.DataContext.ToString());

            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Visible;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigation bar).                           
            SelectPrescriptionViewtemp();
        }

        //Load Patient Combobox.....
        void LoadPatientCombobox()
        {
            try
            {
                var patients = db.users.Where(x => x.role_id == 3 && x.doctor_id == MainWindow.Session.doctor_id).OrderByDescending(x => x.created).Take(10).ToList(); //patient_id 3

                foreach (var patient in patients)
                {
                    PatientComboBox.Items.Add(patient.first_name + "-" + patient.phone);
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
            var diagnosis_templates = db.diagnosis_templates.Where(x => x.doctor_id == MainWindow.Session.doctor_id).OrderByDescending(x => x.created).ToList();
            foreach (var diagnosis_template in diagnosis_templates)
            {
                CheckBox checkbox = new CheckBox();
                checkbox.Content = diagnosis_template.diagnosis.name.ToString();
                checkbox.DataContext = diagnosis_template.id;
                DiagnosisCheckbox.Children.Add(checkbox);

                var exitingDiagnosisTemplate = db.prescriptions_diagnosis
                           .Where(x => x.diagnosis_id == diagnosis_template.id && x.prescription_id == MainWindow.Session.edit_record_id)
                           .FirstOrDefault();

                if (exitingDiagnosisTemplate != null)
                {
                    checkbox.IsChecked = true;
                }
               
                checkbox.AddHandler(CheckBox.ClickEvent, new RoutedEventHandler(DiagnosIsClick));
            }
        }

        int clickCount = 0;

        public static List<long> diagnosis_template_ids = new List<long>();
        private void DiagnosIsClick(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            var diagnosis_template_id = (int)checkBox.DataContext;

            if (checkBox.IsChecked == true)
            {
                diagnosis_template_ids.Add(diagnosis_template_id);
            }

            if (checkBox.IsChecked == false)
            {
                diagnosis_template_ids.Remove(diagnosis_template_id);
            }

            var diagnosis_templates_instructions = db.diagnosis_templates
                 .Where(x => diagnosis_template_ids.Contains(x.id) && x.instructions != "")
                 .Select(x => x.instructions).ToList();

            var instructions = "";
            foreach (var instruction in diagnosis_templates_instructions)
            {
                if (instruction != "")
                {
                    instructions += instruction + (instruction.Equals(diagnosis_templates_instructions.Last()) ? "." : ", ");
                }
            }

            DoctorsNotes.Text = instructions;

            var diagnosisMedicines = db.diagnosis_medicines.Where(x => diagnosis_template_ids.Contains(x.diagnosis_id))                
                .Select(x => new IdNameModel
                {
                    Id = x.medicine_id,
                    Name = x.medicine.name
                })
                .ToList();

            //Create Medicine Section
            RemoveMedicineSectionName(); //Fully Remove Medicine Section

            foreach (var diagnosisMedicine in diagnosisMedicines)
            {
                CreateMedicineSection(diagnosisMedicine.Name, "");
            }

            var diagnosisTests = db.diagnosis_tests.Where(x => diagnosis_template_ids.Contains(x.diagnosis_id))
                .Select(x => new IdNameModel
                {
                    Id = x.test_id,
                    Name = x.test.name
                })
                .ToList();

            ((DiagnosisTestModel)diagnosisTestChosenControl.DataContext).SelectedTests = diagnosisTests;

            // store test ids to save into database
            DiagnosisTestChosenControl.selectedIds.Clear();
            foreach (var diagnosisTest in diagnosisTests)
            {
                DiagnosisTestChosenControl.selectedIds.Add(diagnosisTest.Id);
                diagnosisTestChosenControl._nodeList.Add(new Node(new IdNameModel() { Id = diagnosisTest.Id, Name = diagnosisTest.Name }));
            }
        }

        string PrescriptionTem;
        private void UpdatePrescription_Click(object sender, RoutedEventArgs e)
        {
            if (PatientComboBox.Text != "" && PatientPhone.Text != "" && PatientAge.Text != "")
            {
                try
                {
                    string[] words = PatientComboBox.Text.Split('-');
                    string patientName = words[0];
                    string patientPhone = words[1];

                    var patient = db.users.FirstOrDefault(x => x.first_name == patientName && x.phone == patientPhone && x.doctor_id == MainWindow.Session.doctor_id);

                    if (patient != null)
                    {
                        Grid sidebar = AdminPanelWindow.sidebar;

                        sidebar.Visibility = Visibility.Visible;

                        AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigation bar).            

                        if (((Button)sender).Name == "UpdatePrescription")
                        {
                            NavigationService.Navigate(new Uri("Prescriptions.xaml", UriKind.Relative));
                        }
                        else
                        {
                            SelectPrescriptionViewtemp();
                        }

                        var phone = db.users.FirstOrDefault(x => x.phone == PatientPhone.Text && x.first_name == patientName && x.id != patient.id && x.doctor_id == MainWindow.Session.doctor_id);

                        if (phone == null)
                        {
                            patient.first_name = patientName;
                            patient.phone = PatientPhone.Text.Trim();
                            patient.age = PatientAge.Text.Trim();
                            patient.address_line1 = PatientAddress.Text.Trim();

                            db.SaveChanges();

                            prescription = db.prescriptions.FirstOrDefault(x => x.id == MainWindow.Session.edit_record_id);

                            prescription.user_id = patient.id;
                            prescription.doctor_id = MainWindow.Session.doctor_id; //doctor_id = doctor_id
                            prescription.blood_pressure = BloodPresure.Text;
                            prescription.temperature = Temperature.Text;
                            prescription.doctores_notes = DoctorsNotes.Text;
                            prescription.other_instructions = OtherInstructions.Text;

                            db.SaveChanges();

                            //Add Prescription Diagnosis
                            AddPrescriptionDiagnosis(MainWindow.Session.edit_record_id);

                            //Add Prescription Medicines
                            AddPrescriptionMedicines(MainWindow.Session.edit_record_id);

                            //Add Prescription Tests
                            AddPrescriptionTests(MainWindow.Session.edit_record_id);

                            diagnosis_template_ids.Clear();
                            DiagnosisTestChosenControl.selectedIds.Clear();

                            MessageBox.Show("Prescription has been saved", "Success");
                        }
                        else
                        {
                            MessageBox.Show("The Phone already exist.", "Already Exit");
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Please select a valid patient", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch
                {
                    MessageBox.Show("Please select a valid patient", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please fill in the required fields", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        void SelectPrescriptionViewtemp()
        {
            int doctorPrescriptionTemId = MainWindow.Session.doctorPrescriptionTemId;

            if (doctorPrescriptionTemId == 1)
            {
                PrescriptionTem = "DefaultTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 2)
            {
                PrescriptionTem = "StandardTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 3)
            {
                PrescriptionTem = "ClassicTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 4)
            {
                PrescriptionTem = "CustomTemplate.xaml";
            }
            else
            {
                PrescriptionTem = "GeneralTemplate.xaml";
            }

            NavigationService.Navigate(new Uri("prescriptionTemplates/" + PrescriptionTem, UriKind.Relative));
        }

        void AddPrescriptionDiagnosis(int prescription_id)
        {
            //prescription diagnosis delete
            var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescription_id);
            if (prescriptions_diagnosis.Count() > 0)
            {
                db.prescriptions_diagnosis.RemoveRange(prescriptions_diagnosis);
                int delete_result = db.SaveChanges();
            }

            //prescription diagnosis add            
            foreach (int diagnosis_template_id in diagnosis_template_ids)
            {
                prescriptions_diagnosi.prescription_id = prescription_id;
                prescriptions_diagnosi.diagnosis_id = diagnosis_template_id;
                prescriptions_diagnosi.status = true;
                prescriptions_diagnosi.created = DateTime.Now;
                db.prescriptions_diagnosis.Add(prescriptions_diagnosi);
                int retult_prescription_diagnosis = db.SaveChanges();
            }
        }

        void AddPrescriptionMedicines(int prescription_id)
        {
            //prescription medicines delete
            var prescriptions_medicines = db.prescriptions_medicines.Where(x => x.prescription_id == prescription_id);
            if (prescriptions_medicines.Count() > 0)
            {
                db.prescriptions_medicines.RemoveRange(prescriptions_medicines);
                int delete_result = db.SaveChanges();
            }


            //prescription medicines add
            int i = 0;            
            foreach (Border SingleMedicine in MedicineSection.Children)
            {
                if (SingleMedicine.Visibility != Visibility.Collapsed)
                {
                    var medicineField = (ComboBox)SingleMedicine.FindName("MedicineCombobox_" + i);

                    if (medicineField.Text != "")
                    {
                        var medicine = db.medicines.FirstOrDefault(x => x.name == medicineField.Text);
                        if (medicine != null)
                        {
                            var dosField = (ComboBox)SingleMedicine.FindName("Dos_" + i);

                            prescriptions_medicine.prescription_id = prescription_id;
                            prescriptions_medicine.medicine_id = medicine.id;
                            prescriptions_medicine.rule = dosField.Text;
                            prescriptions_medicine.status = true;
                            prescriptions_medicine.created = DateTime.Now;
                            db.prescriptions_medicines.Add(prescriptions_medicine);
                            int retult_prescription_medecines = db.SaveChanges();
                        }
                    }
                }

                i++;
            }
        }

        void AddPrescriptionTests(int prescription_id)
        {
            //prescription tests delete
            var prescriptions_tests = db.prescriptions_tests.Where(x => x.prescription_id == prescription_id);
            if (prescriptions_tests.Count() > 0)
            {
                db.prescriptions_tests.RemoveRange(prescriptions_tests);
                int delete_result = db.SaveChanges();
            }

            //prescription tests add
            var testsIds = DiagnosisTestChosenControl.selectedIds;
            foreach (int test_id in testsIds)
            {
                prescriptions_test.prescription_id = prescription_id;
                prescriptions_test.test_id = test_id;
                prescriptions_test.status = true;
                prescriptions_test.created = DateTime.Now;
                db.prescriptions_tests.Add(prescriptions_test);
                int retult_prescription_tests = db.SaveChanges();
            }
        }


        private void CancelUpdatePrescription_Click(object sender, RoutedEventArgs e)
        {
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Visible;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(242); // To set width 242 cause when I press AddPresscription it's Width set 0 (to remove sidebar/navigation bar).            

            NavigationService.Navigate(new Uri("Prescriptions.xaml", UriKind.Relative));
        }

        public void LoadMedicines()
        {
           var medicines = db.prescriptions_medicines.Where(x => x.prescription_id == MainWindow.Session.edit_record_id).ToList();

            foreach(var medicine in medicines)
            {
                CreateMedicineSection(medicine.medicine.name, medicine.rule);
            }
        }
        
        //Add More Button
        private void AddMedicine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateMedicineSection("", "");
        }

        public void CreateMedicineSection(String medicineName, string rule)
        {
            //Main Div
            Border border = new Border();

            border.Name = "SingleMedicine";
            BrushConverter bc = new BrushConverter();
            border.Margin = new Thickness(5, 5, 5, 0);
            border.CornerRadius = new CornerRadius(3);
            border.BorderBrush = (Brush)bc.ConvertFrom("#eee");
            border.BorderThickness = new Thickness(1);
            border.Height = 50;

            MedicineSection.Children.Add(border);

            //Grid 
            Grid grid = new Grid();

            grid.Background = (Brush)bc.ConvertFrom("#EBEBEB");
            grid.Height = 50;
            grid.VerticalAlignment = VerticalAlignment.Top;

            border.Child = grid;

            ColumnDefinition columnDefinition0 = new ColumnDefinition();
            columnDefinition0.Width = new GridLength(50, GridUnitType.Star);

            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(30, GridUnitType.Star);

            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition2.Width = new GridLength(20, GridUnitType.Star);

            grid.ColumnDefinitions.Add(columnDefinition0);
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);

            //Combobox Box
            ComboBox comboBox = new ComboBox();

            Grid.SetColumn(comboBox, 0);
            RegisterName("MedicineCombobox_" + clickCount, comboBox);
            comboBox.Text = medicineName;

            comboBox.Style = this.FindResource("DosMedicineComboBox") as Style;

            comboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                new System.Windows.Controls.TextChangedEventHandler(MedicineComboBox_TextChanged));

            comboBox.GotFocus += new RoutedEventHandler(MedicineCombobox_Gotfocus);

            grid.Children.Add(comboBox);

            //Doc Combobox
            ComboBox dosInput = new ComboBox();

            Grid.SetColumn(dosInput, 1);
            RegisterName("Dos_" + clickCount, dosInput);
            dosInput.Text = rule;
            dosInput.Style = this.FindResource("DosMedicineComboBox") as Style;

            dosInput.GotFocus += new RoutedEventHandler(DosCombobox_Gotfocus);

            grid.Children.Add(dosInput);

            //Increment Click 
            clickCount++;

            //Delete Button
            Button button = new Button();

            Grid.SetColumn(button, 2);
            button.Padding = new Thickness(0);
            button.MinWidth = 20;
            button.Height = 25;
            button.Margin = new Thickness(10, 0, 10, 0);
            button.Background = (Brush)bc.ConvertFrom("#B6B6B6");
            button.BorderBrush = (Brush)bc.ConvertFrom("#B6B6B6");
            button.ToolTip = "Remove Medicine";

            button.AddHandler(Button.ClickEvent, new RoutedEventHandler(DelBtnClick));

            grid.Children.Add(button);

            Image image = new Image();

            var SourceUri = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "images/minus.png"));
            image.Source = SourceUri;

            button.Content = image;
        }

        private void DelBtnClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement parent = (FrameworkElement)((Button)sender).Parent;
            FrameworkElement parent1 = (FrameworkElement)parent.Parent;
            parent1.Visibility = Visibility.Collapsed;

        }

        private void DosCombobox_Gotfocus(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            comboBox.Items.Clear();
            comboBox.IsDropDownOpen = true;

            comboBox.Items.Add("1 + 1 + 1");
            comboBox.Items.Add("1 + 1 + 0");
            comboBox.Items.Add("1 + 0 + 1");
            comboBox.Items.Add("1 + 0 + 0");
            comboBox.Items.Add("0 + 1 + 1");
            comboBox.Items.Add("0 + 0 + 1");
            comboBox.Items.Add("0 + 1 + 0");

        }

        private void MedicineCombobox_Gotfocus(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            comboBox.IsDropDownOpen = true;

            var medicines = db.medicines.OrderByDescending(x => x.created).Take(10).ToList(); //

            foreach (var medicine in medicines)
            {
                comboBox.Items.Add(medicine.name);
            }
        }

        private void MedicineComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            var searchBy = comboBox.Text;

            var medicines = db.medicines.Where(x => x.name.StartsWith(searchBy)).Take(10).ToList(); //

            comboBox.Items.Clear();

            foreach (var medicine in medicines)
            {
                comboBox.Items.Add(medicine.name);
            }

            if (medicines.Count() == 0)
            {
                comboBox.Items.Add("No results mached with " + searchBy);
            }
        }

        public void RemoveMedicineSectionName()
        {
            foreach (Border SingleMedicine in MedicineSection.Children)
            {
                SingleMedicine.Visibility = Visibility.Collapsed;
            }
        }

        public void FillUpPatientInfo(int patientId)
        {
            try
            {
                ClearPatientInfo();

                var patient = db.users.FirstOrDefault(x => x.id == patientId);

                if (patient != null)
                {
                    PatientComboBox.SelectedItem = patient.first_name + "-" + patient.phone;
                    PatientPhone.Text = patient.phone;
                    PatientAddress.Text = patient.address_line1;
                    PatientAge.Text = patient.age;

                    if (patient.prescription.Count() > 0)
                    {
                        PatientLastVisit.Text = patient.prescription.Last().created.ToString("dd MMM yyyy");

                        foreach (var prescription in patient.prescription)
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.VerticalAlignment = VerticalAlignment.Center;
                            textBlock.Padding = new Thickness(10, 5, 10, 5);
                            textBlock.DataContext = prescription.id;
                            textBlock.Text = prescription.created.ToString("dd MMM yyyy");

                            textBlock.AddHandler(TextBlock.MouseDownEvent, new RoutedEventHandler(AllPrescriptionClick));

                            AllPrescription.Children.Add(textBlock);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ClearPatientInfo()
        {
            PatientPhone.Text = PatientAddress.Text = PatientAge.Text = PatientLastVisit.Text = "";
            AllPrescription.Children.Clear();
        }
    }
}
