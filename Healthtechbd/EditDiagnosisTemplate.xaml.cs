using WpfChosenControl.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Healthtechbd.Model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for EditDiagnosisTemplate.xaml
    /// </summary>
    public partial class EditDiagnosisTemplate : Page
    {
        public EditDiagnosisTemplate()
        {
            //MainWindow.Session.editRecordId = int.Parse(DiagnosisTemplateId.Text);
            InitializeComponent();
            LoadDiagnosisCombobox();

            DiagnosisComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                    new System.Windows.Controls.TextChangedEventHandler(DiagnosisComboBox_TextChanged));
           
        }        

        diagnosis_template diagnosis_template = new diagnosis_template();
        contextd_db db = new contextd_db();
        diagnosis diagnosis = new diagnosis();
        diagnosis_medicine diagnosis_medecine = new diagnosis_medicine();
        diagnosis_test diagnosis_test = new diagnosis_test();        

        private void CancelAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));
        }

        public EditDiagnosisTemplate(int id) : this()
        {
            DiagnosisTemplateId.Text = id.ToString();

            try
            {
                //To Selected this Diagnosis Template Diagnosis 
                var diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == id);
                DiagnosisComboBox.SelectedItem = diagnosis_template.diagnosis.name;

                Instruction.Text = diagnosis_template.instructions;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                  
        }

        //Load Diagnosis Combobox.......
        void LoadDiagnosisCombobox()
        {
            try
            {
                var diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == MainWindow.Session.editRecordId);

                var diagnosis = db.diagnosis.Where(x => x.id == diagnosis_template.diagnosis_list_id).OrderByDescending(x => x.created).ToList();

                foreach (var item in diagnosis)
                {
                    DiagnosisComboBox.Items.Add(item.name);
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SubmitUpdateDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            var medicinesIds = MedicineChosenControl.selectedIds;
            var tetsIds = TestChosenControl.selectedIds;

            if (DiagnosisComboBox.SelectedItem != "Type here...")
            {
                int diagnosisTemplateId = int.Parse(DiagnosisTemplateId.Text);

                try
                {
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosisTemplateId);
                    diagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisComboBox.Text);

                    var haveDiagnosisTemplate = db.diagnosis_templates.Where(x => x.diagnosis_list_id == diagnosis.id && x.doctor_id == MainWindow.Session.doctorId && diagnosis_template.diagnosis_list_id != diagnosis.id).FirstOrDefault();

                    if (haveDiagnosisTemplate == null)
                    {
                        diagnosis_template.diagnosis_list_id = diagnosis.id;
                        diagnosis_template.instructions = Instruction.Text;

                        //Save Diagnosis Medicine......
                        SaveDiagnosisMedicine(diagnosisTemplateId);

                        //Save Diagnosis Test.......
                        SaveDiagnosisTest(diagnosisTemplateId);

                        db.SaveChanges();

                        NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));
                        MessageBox.Show("Update Successfully", "Success");
                    }
                    else
                    {
                        MessageBox.Show("The Diagnosis Template already exist", "Already Exit");
                    }
                }
                catch
                {
                    MessageBox.Show("Please select a diagnosis", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a diagnosis", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
        }

        public void SaveDiagnosisMedicine(int diagnosisTemplateId)
        {
            //medicines delete
            var diagnosis_medicines = db.diagnosis_medicines.Where(x => x.diagnosis_id == diagnosisTemplateId);
            if (diagnosis_medicines.Count() > 0)
            {
                db.diagnosis_medicines.RemoveRange(diagnosis_medicines);
                int delete_result = db.SaveChanges();
            }

            //medicines add
            var medicinesIds = MedicineChosenControl.selectedIds;
            foreach (int medicine_id in medicinesIds)
            {
                diagnosis_medecine.diagnosis_id = diagnosisTemplateId;
                diagnosis_medecine.medicine_id = medicine_id;
                diagnosis_medecine.status = true;
                diagnosis_medecine.created = DateTime.Now;
                db.diagnosis_medicines.Add(diagnosis_medecine);
                int retult_diagnosis_medecines = db.SaveChanges();
            }

            MedicineChosenControl.selectedIds.Clear();
        }

        public void SaveDiagnosisTest(int diagnosisTemplateId)
        {
            //test delete
            var diagnosis_tests = db.diagnosis_tests.Where(x => x.diagnosis_id == diagnosisTemplateId);
            if (diagnosis_tests.Count() > 0)
            {
                db.diagnosis_tests.RemoveRange(diagnosis_tests);
                int delete_result = db.SaveChanges();
            }

            //tets add
            var testsIds = TestChosenControl.selectedIds;
            foreach (int test_id in testsIds)
            {
                diagnosis_test.diagnosis_id = diagnosisTemplateId;
                diagnosis_test.test_id = test_id;
                diagnosis_test.status = true;
                diagnosis_test.created = DateTime.Now;
                db.diagnosis_tests.Add(diagnosis_test);
                int retult_diagnosis_tests = db.SaveChanges();
            }

            TestChosenControl.selectedIds.Clear();
        }

        void DiagnosisComboBox_TextChanged(object sender, RoutedEventArgs e)
        {
            ComboBox obj = sender as ComboBox;

            var item = obj.Text;

            var diagnosis = db.diagnosis.Where(x => x.name.Contains(item)).OrderByDescending(x => x.created).Take(10).ToList();
            DiagnosisComboBox.Items.Clear();

            foreach (var data in diagnosis)
            {
                DiagnosisComboBox.Items.Add(data.name);
            }

            if (diagnosis.Count == 0)
            {
                DiagnosisComboBox.Items.Add("No results mached with " + item);
            }
        }

        private void DiagnosisComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DiagnosisComboBox.IsDropDownOpen = true;
        }
        
    }   
}
