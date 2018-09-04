using Healthtechbd.model;
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
            LoadDiagnosisCombobox();

            DiagnosisComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                    new System.Windows.Controls.TextChangedEventHandler(DiagnosisComboBox_TextChanged));
        }        

        diagnosis_templates diagnosis_template = new diagnosis_templates();
        contextd_db db = new contextd_db();
        diagnosis diagnosis = new diagnosis();

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

        public EditDiagnosisTemplate(int id) : this()
        {
            DiagnosisTemplateId.Text = id.ToString();

            MainWindow.Session.editRecordId = id;
            MainModel mainModel = new MainModel();
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
            if(DiagnosisComboBox.SelectedItem != "Type here...")
            {
                int diagnosisTemplateId = int.Parse(DiagnosisTemplateId.Text);

                try
                {
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosisTemplateId);
                    diagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisComboBox.SelectedItem.ToString());

                    diagnosis_template.diagnosis_list_id = diagnosis.id;
                    diagnosis_template.instructions = Instruction.Text;
                    db.SaveChanges();

                    NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));
                    MessageBox.Show("Update Successfully", "Success");
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
