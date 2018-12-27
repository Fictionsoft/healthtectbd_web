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
using WpfChosenControl.model;
using WpfChosenControl;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Healthtechbd.Model.ApiModel;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for Prescriptions.xaml
    /// </summary>
    public partial class Prescriptions : Page
    {
        public Prescriptions()
        {
            InitializeComponent();

            loadPrescriptions();
        }

        contextd_db db = new contextd_db();
        prescription prescription = new prescription();
        prescriptions_diagnosis prescriptions_diagnosi = new prescriptions_diagnosis();

        void loadPrescriptions()
        {            
            try
            {
                if (MainWindow.Session.set_patient_id > 0)
                {
                    var prescriptions = db.prescriptions.Where(x => x.doctor_id == MainWindow.Session.doctor_id && x.user_id == MainWindow.Session.set_patient_id)
                                  .OrderByDescending(x => x.created).Take(40).ToList();

                    dataGridPrescriptions.ItemsSource = prescriptions;
                }
                else
                {
                    var prescriptions = db.prescriptions.Where(x => x.doctor_id == MainWindow.Session.doctor_id)
                                 .OrderByDescending(x => x.created).Take(40).ToList();                    

                    dataGridPrescriptions.ItemsSource = prescriptions;
                }              
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Session.edit_record_id = 0;
            AddPrescription.diagnosis_template_ids.Clear();
            DiagnosisTestChosenControl.selectedIds.Clear();

            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            NavigationService.Navigate(new Uri("AddPrescription.xaml", UriKind.Relative));
        }

        private void btnDeletePrescriptionRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int prescription_id = (dataGridPrescriptions.SelectedItem as prescription).id;
                try
                {
                    prescription = db.prescriptions.FirstOrDefault(x => x.id == prescription_id);

                    db.prescriptions.Remove(prescription);

                    //prescription diagnosis delete
                    var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescription_id);
                    if (prescriptions_diagnosis.Count() > 0)
                    {
                        db.prescriptions_diagnosis.RemoveRange(prescriptions_diagnosis);
                        int delete_result = db.SaveChanges();
                    }

                    //prescription medicines delete
                    var prescriptions_medicines = db.prescriptions_medicines.Where(x => x.prescription_id == prescription_id);
                    if (prescriptions_medicines.Count() > 0)
                    {
                        db.prescriptions_medicines.RemoveRange(prescriptions_medicines);
                        int delete_result = db.SaveChanges();
                    }

                    //prescription tests delete
                    var prescriptions_tests = db.prescriptions_tests.Where(x => x.prescription_id == prescription_id);
                    if (prescriptions_tests.Count() > 0)
                    {
                        db.prescriptions_tests.RemoveRange(prescriptions_tests);
                        int delete_result = db.SaveChanges();
                    }

                    db.SaveChanges();
                    loadPrescriptions();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if(searchField.Text != "")
            {
                search();
            }           
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchField.Clear();
            MainWindow.Session.set_patient_id = 0;
            loadPrescriptions();
        }

        string PrescriptionTem;
        private void btnViewPrescriptionRow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Session.edit_record_id = (dataGridPrescriptions.SelectedItem as prescription).id;

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

        private void btnEditPrescriptionRow_Click(object sender, RoutedEventArgs e)
        {            
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            int prescription_id = (dataGridPrescriptions.SelectedItem as prescription).id;
            MainWindow.Session.edit_record_id = prescription_id;
            EditPrescription editPrescription = new EditPrescription(prescription_id);

            EditPrescription.diagnosis_template_ids.Clear();
            var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescription_id).ToList();
            foreach (var prescriptions_diagnosi in prescriptions_diagnosis)
            {
                EditPrescription.diagnosis_template_ids.Add(prescriptions_diagnosi.diagnosis_id);
            }

            NavigationService.Navigate(editPrescription);                      
        }

        public void search()
        {
            string searchBy = searchField.Text.ToString();
            List<prescription> prescriptions = new List<prescription>();
            try
            {
                if(MainWindow.Session.set_patient_id > 0)
                {
                    prescriptions = db.prescriptions.Where(x => (x.user.first_name.Contains(searchBy) ||
                                                               x.user.phone.Contains(searchBy)) &&
                                                               x.user_id == MainWindow.Session.set_patient_id &&
                                                               x.doctor_id == MainWindow.Session.doctor_id
                                                        ).Take(40).ToList();
                }
                else
                {
                    prescriptions = db.prescriptions.Where(x => (x.user.first_name.Contains(searchBy) ||
                                                               x.user.phone.Contains(searchBy)) &&
                                                               x.doctor_id == MainWindow.Session.doctor_id
                                                         ).Take(40).ToList();

                }
                dataGridPrescriptions.ItemsSource = prescriptions;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonSyncPrescription_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.Internet.CheckForInternetConnection() == true)
            {
                GetOnlinePrescriptions();
            }
            else
            {
                MessageBox.Show("Check your Internet connection", "Warning");
            }
        }

        public async void GetOnlinePrescriptions()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.api_base_url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("admin/prescriptions/get-online-prescriptions?doctor_email=" + MainWindow.Session.doctor_email).Result;
            if (response.IsSuccessStatusCode)
            {
                var prescriptions = response.Content.ReadAsStringAsync();
                prescriptions.Wait();
                var onlinePrescriptions = JsonConvert.DeserializeObject<List<ViewPrescriptions>>(prescriptions.Result); 
                
                if(onlinePrescriptions.Count() > 0)
                {
                    SaveOnlinePrescriptionsToLocal(onlinePrescriptions);
                }
            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }

        public void SaveOnlinePrescriptionsToLocal(List<ViewPrescriptions> onlinePrescriptions)
        {
            foreach (var onlinePrescription in onlinePrescriptions)
            {
                var patientName = onlinePrescription.user.first_name;
                var patientPhone = onlinePrescription.user.phone;

                var have_patient = db.users.Where(x => x.first_name == patientName && x.phone == patientPhone && x.doctor_id == MainWindow.Session.doctor_id).FirstOrDefault();

                if (have_patient != null)
                {
                    prescription.user_id = have_patient.id;
                    prescription.doctor_id = MainWindow.Session.doctor_id; //doctor_id = doctor_id
                    prescription.blood_pressure = onlinePrescription.blood_pressure;
                    prescription.temperature = onlinePrescription.temperature;
                    prescription.doctores_notes = onlinePrescription.doctores_notes;
                    prescription.other_instructions = onlinePrescription.other_instructions;
                    prescription.status = true;
                    prescription.created = DateTime.Now;
                    prescription.is_sync = 1;

                    db.prescriptions.Add(prescription);
                    int result = db.SaveChanges();

                    if (result == 1) // Prescription Save
                    {
                        onlinePrescription.id = prescription.id;
                    }
                }
            }

            //Save online prescription diagnosis to local
            foreach (var onlinePrescription in onlinePrescriptions)
            {
                if (onlinePrescription.diagnosis.Count() > 0)
                {
                    SaveOnlinePrescriptionsDiagnosisToLocal(onlinePrescription.diagnosis, onlinePrescription.id); // diagnosis == diagnosis_templates
                }
            } 
        }

        public void SaveOnlinePrescriptionsDiagnosisToLocal(List<ViewDiagnosis> online_prescription_diagnosis, int prescription_id)
        {
            foreach (var online_prescription_diagnosi in online_prescription_diagnosis)
            {
                var have_diagnosis_list = db.diagnosis.FirstOrDefault(x => x.name == online_prescription_diagnosi.diagnosis_list.name);

                if(have_diagnosis_list != null && have_diagnosis_list.diagnosis_template.Count() > 0) // have diagnosis_list and template
                {
                    prescriptions_diagnosi.prescription_id = prescription_id;
                    prescriptions_diagnosi.diagnosis_id = have_diagnosis_list.diagnosis_template.Last().id;
                    prescriptions_diagnosi.status = true;
                    prescriptions_diagnosi.created = DateTime.Now;
                    db.prescriptions_diagnosis.Add(prescriptions_diagnosi);
                    int retult_prescription_diagnosis = db.SaveChanges();
                }
            }           
        }
    }
}
