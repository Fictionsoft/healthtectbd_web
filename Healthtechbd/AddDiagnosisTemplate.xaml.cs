using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for AddDiagnosisTemplate.xaml
    /// </summary>
    public partial class AddDiagnosisTemplate : Page
    {
        public AddDiagnosisTemplate()
        {
            InitializeComponent();
            LoadDiagnosisCombobox();

            DiagnosisComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                    new System.Windows.Controls.TextChangedEventHandler(DiagnosisComboBox_TextChanged));
        }

        contextd_db db = new contextd_db();
        diagnosis diagnosis = new diagnosis();
        diagnosis_template diagnosis_template = new diagnosis_template();
        diagnosis_medicine diagnosis_medicine = new diagnosis_medicine();
        diagnosis_test diagnosis_test = new diagnosis_test();
        medicine medicine = new medicine();

        private void SubmitAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (DiagnosisComboBox.SelectedItem != "Type here...")
            {
                try
                {
                    diagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisComboBox.Text);

                    var haveDiagnosisTemplate = db.diagnosis_templates.FirstOrDefault(x => x.diagnosis_list_id == diagnosis.id && x.doctor_id == MainWindow.Session.doctor_id);

                    if (haveDiagnosisTemplate == null)
                    {
                        NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));

                        if (CreateDiagnosisTemplate(diagnosis.id, Instruction.Text, 0) > 0)
                        {
                            //diagnosis medicines add
                            CreateDiagnosisMedicine(diagnosis_template.id);

                            //diagnosis tests add                            
                            CreateDiagnosisTest(diagnosis_template.id);

                            MedicineChosenControl.selectedIds.Clear();
                            TestChosenControl.selectedIds.Clear();
                        }

                        MessageBox.Show("Diagnosis Tempalte has been saved", "Save");
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

        public int CreateDiagnosisTemplate(int diagnosis_id, string instructions, int is_sync)
        {
            diagnosis_template.diagnosis_list_id = diagnosis_id;
            diagnosis_template.doctor_id = MainWindow.Session.doctor_id;
            diagnosis_template.instructions = instructions;
            diagnosis_template.status = true;
            diagnosis_template.is_sync = is_sync;
            diagnosis_template.created = DateTime.Now;
            db.diagnosis_templates.Add(diagnosis_template);
            int result = db.SaveChanges();

            if (result == 1)
            {
                return diagnosis_template.id;
            }
            return 0;
        }

        public void CreateDiagnosisMedicine(int diagnosis_template_id)
        {
            var medicines_ids = MedicineChosenControl.selectedIds;
            foreach (int medicine_id in medicines_ids)
            {
                diagnosis_medicine.medicine_id = medicine_id;
                diagnosis_medicine.diagnosis_id = diagnosis_template_id;
                diagnosis_medicine.status = true;
                diagnosis_medicine.created = DateTime.Now;
                db.diagnosis_medicines.Add(diagnosis_medicine);
                db.SaveChanges();
            }
        }

        public void CreateDiagnosisTest(int diagnosis_template_id)
        {
            var tests_ids = TestChosenControl.selectedIds;
            foreach (int test_id in tests_ids)
            {
                diagnosis_test.diagnosis_id = diagnosis_template_id;
                diagnosis_test.test_id = test_id;
                diagnosis_test.status = true;
                diagnosis_test.created = DateTime.Now;
                db.diagnosis_tests.Add(diagnosis_test);
                db.SaveChanges();
            }
        }

        void LoadDiagnosisCombobox()
        {
            try
            {
                var diagnosis = db.diagnosis.OrderByDescending(x => x.created).Take(10).ToList();

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

        private void CancelAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));
        }

        void DiagnosisComboBox_TextChanged(object sender, RoutedEventArgs e)
        {
            DiagnosisComboBox.IsDropDownOpen = true;

            ComboBox obj = sender as ComboBox;

            var searchBy = obj.Text;

            var diagnosis = db.diagnosis.Where(x => x.name.StartsWith(searchBy)).Take(10).ToList();
            DiagnosisComboBox.Items.Clear();

            foreach (var data in diagnosis)
            {
                DiagnosisComboBox.Items.Add(data.name);
            }

            if (diagnosis.Count == 0)
            {
                DiagnosisComboBox.Items.Add("No results mached with " + searchBy);
            }
        }

        private void DiagnosisComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DiagnosisComboBox.IsDropDownOpen = true;
        }
    }
}
