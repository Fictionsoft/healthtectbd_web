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

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for EditDiagnosisTemplate.xaml
    /// </summary>
    public partial class EditDiagnosisTemplate : Page
    {
        public EditDiagnosisTemplate()
        {
            InitializeComponent();
            loadDiagnosisCombobox();
        }
        
        model.diagnosis_templates diagnosis_template = new model.diagnosis_templates();
        model.ContextDb db = new model.ContextDb();
        model.diagnosis diagnosis = new model.diagnosis();

        void loadDiagnosisCombobox()
        {
            try
            {
                var diagnosis = db.diagnosis.OrderByDescending(x => x.created).ToList();

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

        public EditDiagnosisTemplate(int id) : this()
        {
            DiagnosisTemplateId.Text = id.ToString();

            try
            {
                var diagnosisTemplate = db.diagnosis_templates.FirstOrDefault(x => x.id == id);
                DiagnosisComboBox.SelectedItem = diagnosisTemplate.diagnosis.name;

                Instruction.Text = diagnosisTemplate.instructions;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                  
        }

        private void SubmitAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            if(DiagnosisComboBox.SelectedIndex != 0)
            {
                int diagnosisTemplateId = int.Parse(DiagnosisTemplateId.Text);

                try
                {
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosisTemplateId);
                    diagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisComboBox.SelectedItem.ToString());

                    diagnosis_template.diagnosis_id = diagnosis.id;
                    diagnosis_template.instructions = Instruction.Text;
                    db.SaveChanges();

                    NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));
                    MessageBox.Show("Update Successfully");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
            else
            {
                MessageBox.Show("Diagnosis name is required", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
        }
    }
}
