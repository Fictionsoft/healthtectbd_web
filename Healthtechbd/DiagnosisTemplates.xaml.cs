using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Interaction logic for DiagnosisTemplates.xaml
    /// </summary>
    public partial class DiagnosisTemplates : Page
    {
        public DiagnosisTemplates()
        {
            InitializeComponent();
            loadDiagnosisTemplates();
        }

        contextd_db db = new contextd_db();        
        diagnosis_template diagnosis_template = new diagnosis_template();        

        void loadDiagnosisTemplates()
        {
            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.doctor_id == MainWindow.Session.doctorId)
                    .Include(x => x.diagnosis)
                    .OrderByDescending(x => x.created)
                    .Take(10)
                    .ToList();

                dataGridDiagnosisTemplates.ItemsSource = diagnosis_templates;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddDiagnosisTemplate.xaml", UriKind.Relative));

            MainWindow.Session.editRecordId = 0;
            MedicineChosenControl.selectedIds.Clear();
            TestChosenControl.selectedIds.Clear();
        }

        private void btnDeleteDiagnosisTemplateRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int diagnosisTemplateId = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;

                try
                {
                    //Remove Diagnosis Template
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosisTemplateId);
                    db.diagnosis_templates.Remove(diagnosis_template);

                    //Remove Diagnosis_Tests
                    var diagnosis_tests = db.diagnosis_tests.Where(x => x.diagnosis_id == diagnosisTemplateId).ToList();
                    db.diagnosis_tests.RemoveRange(diagnosis_tests);

                    //Remove Diagnosis_Medicines
                    var diagnosis_medicines = db.diagnosis_medicines.Where(x => x.diagnosis_id == diagnosisTemplateId).ToList();
                    db.diagnosis_medicines.RemoveRange(diagnosis_medicines);

                    db.SaveChanges();
                    loadDiagnosisTemplates();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }               
            }
        }

        private void btnEditDiagnosisTemplateRow_Click(object sender, RoutedEventArgs e)
        {
            MedicineChosenControl.selectedIds.Clear();
            TestChosenControl.selectedIds.Clear();

            int diagnosisTemplateId = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;
            MainWindow.Session.editRecordId = diagnosisTemplateId;
            EditDiagnosisTemplate editDiagnosisTemplate = new EditDiagnosisTemplate(diagnosisTemplateId);
            NavigationService.Navigate(editDiagnosisTemplate);           
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            loadDiagnosisTemplates();
            searchField.Clear();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.diagnosis.name.Trim().Contains(searchBy)).OrderByDescending(x => x.created).Take(10).ToList();

                if (diagnosis_templates.Count == 0)
                {
                    MessageBox.Show("DiagnosisTemplate not found", "Wraning");
                }
                else
                {
                    dataGridDiagnosisTemplates.ItemsSource = diagnosis_templates;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void searchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.diagnosis.name.Trim().Contains(searchBy)).OrderByDescending(x => x.created).Take(10).ToList();

                dataGridDiagnosisTemplates.ItemsSource = diagnosis_templates;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
