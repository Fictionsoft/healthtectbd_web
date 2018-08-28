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

        ContextDb db = new ContextDb();
        diagnosis diagnosis = new diagnosis();
        diagnosis_templates diagnosis_template = new diagnosis_templates();
        diagnosis_medicines diagnosis_medecine = new diagnosis_medicines();
        medicine medicine = new medicine();

        private void SubmitAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (DiagnosisComboBox.SelectedItem != "Type here..." && Instruction.Text != "")
            {
                //try
                //{
                diagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisComboBox.Text);

                var haveDiagnosisTemplate = db.diagnosis_templates.FirstOrDefault(x => x.diagnosis_list_id == diagnosis.id);

                if (haveDiagnosisTemplate == null)
                {
                    NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));

                    diagnosis_template.diagnosis_list_id = diagnosis.id;
                    diagnosis_template.instructions = Instruction.Text;
                    diagnosis_template.status = true;
                    diagnosis_template.created = DateTime.Now;
                    db.diagnosis_templates.Add(diagnosis_template);

                    int result_diagnosis_template = db.SaveChanges();
                    if (result_diagnosis_template > 0)
                    {
                        int diagnosis_template_id = diagnosis_template.id;

                        // delete
                        var diagnosis_medicines = db.diagnosis_medicines.Where(x => x.diagnosis_id == diagnosis_template_id);
                        if (diagnosis_medicines.Count() > 0)
                        {
                            db.diagnosis_medicines.RemoveRange(diagnosis_medicines);
                            int delete_result = db.SaveChanges();
                        }
                        
                        // add
                        var ids = ChosenControl.selectedIds;
                        foreach (int medicine_id in ids)
                        {
                            diagnosis_medecine.diagnosis_id = diagnosis_template_id;
                            diagnosis_medecine.medicine_id = medicine_id;
                            diagnosis_medecine.status = true;
                            diagnosis_medecine.created = DateTime.Now;
                            db.diagnosis_medicines.Add(diagnosis_medecine);
                            int retult_diagnosis_medecines = db.SaveChanges();
                        }
                        
                    }

                    MessageBox.Show("Diagnosis Tempalte has been saved", "Save");
                }
                else
                {
                    MessageBox.Show("The Diagnosis Template already exist", "Already Exit");
                }
                //}
                //catch
                //{
                //    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
            }
            else
            {
                MessageBox.Show("Please fill in the required fields", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            var item = obj.Text;

            var diagnosis = db.diagnosis.Where(x => x.name.StartsWith(item)).OrderByDescending(x => x.created).Take(10).ToList();
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
