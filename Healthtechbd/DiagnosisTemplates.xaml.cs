﻿using Healthtechbd.Model.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            //Getonline_diagnosis_templates();
        }

        contextd_db db = new contextd_db();
        diagnosis_template diagnosis_template = new diagnosis_template();
        diagnosis diagnosis = new diagnosis();
        medicine medicine = new medicine();
        test test = new test();
        diagnosis_medicine diagnosis_medicine = new diagnosis_medicine();
        diagnosis_test diagnosis_test = new diagnosis_test();

        AddDiagnosisTemplate addDiagnosisTemplate = new AddDiagnosisTemplate();

        void loadDiagnosisTemplates()
        {
            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.doctor_id == MainWindow.Session.doctor_id)
                    .OrderByDescending(x => x.created)
                    .Take(40)
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

            MainWindow.Session.edit_record_id = 0;
            MedicineChosenControl.selectedIds.Clear();
            TestChosenControl.selectedIds.Clear();
        }

        private void btnDeleteDiagnosisTemplateRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int diagnosis_template_id = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;

                try
                {
                    //Remove Diagnosis Template
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosis_template_id);
                    db.diagnosis_templates.Remove(diagnosis_template);

                    //Remove Diagnosis_Tests
                    var diagnosis_tests = db.diagnosis_tests.Where(x => x.diagnosis_id == diagnosis_template_id).ToList();
                    db.diagnosis_tests.RemoveRange(diagnosis_tests);

                    //Remove Diagnosis_Medicines
                    var diagnosis_medicines = db.diagnosis_medicines.Where(x => x.diagnosis_id == diagnosis_template_id).ToList();
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

            int diagnosis_template_id = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;
            MainWindow.Session.edit_record_id = diagnosis_template_id;
            EditDiagnosisTemplate editDiagnosisTemplate = new EditDiagnosisTemplate(diagnosis_template_id);
            NavigationService.Navigate(editDiagnosisTemplate);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            loadDiagnosisTemplates();
            searchField.Clear();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        public void search()
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.diagnosis.name.Contains(searchBy) && x.doctor_id == MainWindow.Session.doctor_id).OrderByDescending(x => x.created).Take(10).ToList();

                dataGridDiagnosisTemplates.ItemsSource = diagnosis_templates;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SyncDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.Internet.CheckForInternetConnection() == true)
            {
                Getonline_diagnosis_templates();
                loadDiagnosisTemplates();
                MessageBox.Show("Diagnosis templates sync successfully", "Success", MessageBoxButton.OK);            
                //GetLocalDiagnosisTemplates();
            }
            else
            {
                MessageBox.Show("Check your Internet connection", "Warning");
            }
        }

        public void Getonline_diagnosis_templates()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.api_base_url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("admin/diagnosis/get-online-diagnosis-templates?doctor_email=" + MainWindow.Session.doctor_email).Result;
            if (response.IsSuccessStatusCode)
            {
                var diagnosisTemplates = response.Content.ReadAsStringAsync();
                diagnosisTemplates.Wait();
                var online_diagnosis_templates = JsonConvert.DeserializeObject<List<ViewDiagnosisTemplates>>(diagnosisTemplates.Result);

                if(online_diagnosis_templates.Count() > 0)
                {
                    SaveOnlineDiagnosisTemplatesToLocal(online_diagnosis_templates);
                }
            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }

        public void SaveOnlineDiagnosisTemplatesToLocal(List<ViewDiagnosisTemplates> online_diagnosis_templates)
        {
            foreach (var online_diagnosis_template in online_diagnosis_templates)
            {
                var haveDiagnosis = db.diagnosis.FirstOrDefault(x => x.name == online_diagnosis_template.diagnosis_list.name);

                if (haveDiagnosis != null && haveDiagnosis.diagnosis_template.Count() == 0)
                {
                    //Create diagnosis template
                    int diagnosis_template_id = addDiagnosisTemplate.CreateDiagnosisTemplate(haveDiagnosis.id, online_diagnosis_template.instructions, 1);

                    if (diagnosis_template_id > 0)
                    {
                        online_diagnosis_template.id = diagnosis_template_id;                       
                    }
                }
            }

            foreach (var online_diagnosis_template in online_diagnosis_templates)
            {
                if(online_diagnosis_template.medicines.Count() > 0)
                {
                    SaveOnlineDiagnosisMedicinesToLocal(online_diagnosis_template.medicines, online_diagnosis_template.id);
                }

                if (online_diagnosis_template.tests.Count() > 0)
                {
                    SaveOnlineDiagnosisTestsToLocal(online_diagnosis_template.tests, online_diagnosis_template.id);
                }
            }
        }

        public void SaveOnlineDiagnosisMedicinesToLocal(List<ViewName> online_diagnosis_medicines, int online_diagnosis_template_id)
        {
            foreach (var online_diagnosis_medicine in online_diagnosis_medicines)
            {                
                var have_medicine = db.medicines.FirstOrDefault(x => x.name == online_diagnosis_medicine.name);

                if (have_medicine != null) // have 
                {
                    MedicineChosenControl.selectedIds.Add(have_medicine.id);
                }   
            }
            addDiagnosisTemplate.CreateDiagnosisMedicine(online_diagnosis_template_id);
            MedicineChosenControl.selectedIds.Clear();
        }

        public void SaveOnlineDiagnosisTestsToLocal(List<ViewName> online_diagnosis_tests, int online_diagnosis_template_id)
        {
            foreach (var online_diagnosis_test in online_diagnosis_tests)
            {
                var haveTests = db.tests.FirstOrDefault(x => x.name == online_diagnosis_test.name);

                if (haveTests != null)
                {
                    diagnosis_test.test_id = haveTests.id;
                    diagnosis_test.diagnosis_id = online_diagnosis_template_id;
                    diagnosis_test.status = true;
                    diagnosis_test.created = DateTime.Now;
                    db.diagnosis_tests.Add(diagnosis_test);
                    db.SaveChanges();
                }
            }
        }

        public async void GetLocalDiagnosisTemplates()
        {
            var diagnosis_templates = db.diagnosis_templates
                    .Where(x => x.doctor_id == MainWindow.Session.doctor_id && x.is_sync == 0)
                    .OrderByDescending(x => x.created)
                    .Take(40)
                    .ToList();            

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.api_base_url);

            var json_iagnosis_templates = JsonConvert.SerializeObject(diagnosis_templates, Formatting.Indented,
            new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
                
            });

            HttpResponseMessage response = client.PostAsJsonAsync("admin/diagnosis/get-local-diagnosis-templates?doctor_email=" + MainWindow.Session.doctor_email, json_iagnosis_templates).Result;
            if (response.IsSuccessStatusCode)
            {
                var diagnosisTemplates = response.Content.ReadAsStringAsync();
                diagnosisTemplates.Wait();
                var online_diagnosis_templates = JsonConvert.DeserializeObject(diagnosisTemplates.Result);

                foreach (var diagnosis_template in diagnosis_templates)
                {
                    diagnosis_template.is_sync = 1;
                    int result = db.SaveChanges();
                }

            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }
    }
}
