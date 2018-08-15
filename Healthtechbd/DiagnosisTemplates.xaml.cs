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

        ContextDb db = new ContextDb();
        //diagnosis diagnosis = new diagnosis();
        diagnosis_templates diagnosis_template = new diagnosis_templates();

        void loadDiagnosisTemplates()
        {
            try
            {
                var diagnosisTemplates = db.diagnosis_templates
                    .Include(x => x.diagnosis)
                    .OrderByDescending(x => x.created)
                    .Take(10)
                    .ToList();

                dataGridDiagnosisTemplates.ItemsSource = diagnosisTemplates;
        }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
}

        private void ButtonAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddDiagnosisTemplate.xaml", UriKind.Relative));
        }

        private void btnDeleteDiagnosisTemplateRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int diagnosisTemplateId = (dataGridDiagnosisTemplates.SelectedItem as model.diagnosis_templates).id;

                try
                {
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosisTemplateId);

                    db.diagnosis_templates.Remove(diagnosis_template);
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
            int diagnosisTemplateId = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_templates).id;
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
                var diagnosisTemplates = db.diagnosis_templates.Where(x => x.diagnosis.name.Trim().StartsWith(searchBy)).OrderByDescending(x => x.created).Take(10).ToList();

                if (diagnosisTemplates.Count == 0)
                {
                    MessageBox.Show("DiagnosisTemplate not found", "Wraning");
                }
                else
                {
                    dataGridDiagnosisTemplates.ItemsSource = diagnosisTemplates;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }
    }
}
