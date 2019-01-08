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

        public static int offline_total;
        public static int offline_success;
        public static int offline_duplicate;
        public static int online_total;
        public static int online_success;
        public static int online_duplicate;
        public static List<long> will_sync_true_diagnosis_template_ids = new List<long>();

        contextd_db db = new contextd_db();
        prescription prescription = new prescription();
        prescriptions_diagnosis prescriptions_diagnosi = new prescriptions_diagnosis();
        prescriptions_medicine prescriptions_medicine = new prescriptions_medicine();
        prescriptions_test prescriptions_test = new prescriptions_test();

        AddPrescription addPrescription = new AddPrescription();

        void loadPrescriptions()
        {            
            try
            {
                if (MainWindow.Session.set_patient_id > 0)
                {
                    var prescriptions = db.prescriptions.Where(x => x.doctor_id == MainWindow.Session.doctor_id && x.user_id == MainWindow.Session.set_patient_id)
                                  .OrderByDescending(x => x.id).Take(40).ToList();

                    dataGridPrescriptions.ItemsSource = prescriptions;
                }
                else
                {
                    var prescriptions = db.prescriptions.Where(x => x.doctor_id == MainWindow.Session.doctor_id)
                                 .OrderByDescending(x => x.id).Take(40).ToList();                    

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
            MainWindow.Session.edit_record_id = (dataGridPrescriptions.SelectedItem as prescription).id; //prescription id save to session for get prescription info from view prescription by this id

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
            DiagnosisTestChosenControl.selectedIds.Clear();
            
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
                                                        ).OrderByDescending(x => x.id)
                                                        .Take(40).ToList();
                }
                else
                {
                    prescriptions = db.prescriptions.Where(x => (x.user.first_name.Contains(searchBy) ||
                                                               x.user.phone.Contains(searchBy)) &&
                                                               x.doctor_id == MainWindow.Session.doctor_id
                                                         ).OrderByDescending(x => x.id)
                                                         .Take(40).ToList();

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
            if(MessageBox.Show("Are you sure Diagnosis Template and Patient has been already synced", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (MainWindow.Internet.CheckForInternetConnection() == true)
                {
                    GetOnlinePrescriptions();
                    GetLocalPrescriptions();
                    loadPrescriptions();
                    MessageBox.Show("Online to offline: \n Total : " + offline_total + "\n Success : " + offline_success 
                                 + "\n \n Offline to online: \n Total : " + online_total + "\n Success : " + online_success,
                                 "Prescriptions sync report", MessageBoxButton.OK);

                    offline_total = offline_success = offline_duplicate = online_total = online_success = online_duplicate = 0; // Clear Value
                    will_sync_true_diagnosis_template_ids.Clear(); // Clear Value
                }
                else
                {
                    MessageBox.Show("Check your internet connection, Please try again", "Warning");
                }
            }            
        }

        public async void GetOnlinePrescriptions()
        {
            //try
            //{
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(MainWindow.Session.api_base_url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("admin/prescriptions/get-online-prescriptions?doctor_email=" + MainWindow.Session.doctor_email).Result;
                if (response.IsSuccessStatusCode)
                {
                    var prescriptions = response.Content.ReadAsStringAsync();
                    prescriptions.Wait();
                    var online_prescriptions = JsonConvert.DeserializeObject<List<ViewPrescriptions>>(prescriptions.Result);

                    //count total sync prescriptions from online
                    offline_total = online_prescriptions.Count();

                    if (offline_total > 0)
                    {
                        if (SaveOnlinePrescriptionsToLocal(online_prescriptions))
                        {
                            HttpResponseMessage change_is_sync_response = client.PostAsJsonAsync("admin/prescriptions/get-online-prescriptions", will_sync_true_diagnosis_template_ids).Result;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            //}
            //catch
            //{
            //    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}            
        }

        public bool SaveOnlinePrescriptionsToLocal(List<ViewPrescriptions> online_prescriptions)
        {
            foreach (var online_prescription in online_prescriptions)
            {
                var patientName = online_prescription.user.first_name;
                var patientPhone = online_prescription.user.phone;

                var have_patient = db.users.Where(x => x.first_name == patientName && x.phone == patientPhone && x.doctor_id == MainWindow.Session.doctor_id).FirstOrDefault();

                if (have_patient != null)
                {
                    will_sync_true_diagnosis_template_ids.Add(online_prescription.id);

                    //save online prescription to local
                    int prescription_id = addPrescription.CreatePrescription(have_patient.id, online_prescription.blood_pressure, online_prescription.temperature, online_prescription.doctores_notes, online_prescription.other_instructions, 1);

                    if (prescription_id > 0) //prescription Save
                    {
                        online_prescription.id = prescription_id;
                    }

                    //count success sync prescriptions from online
                    offline_success++;
                }
            }
           
            foreach (var online_prescription in online_prescriptions)
            {
                //save online prescription diagnosis to local
                if (online_prescription.diagnosis.Count() > 0)
                {
                    SaveOnlinePrescriptionsDiagnosisToLocal(online_prescription.diagnosis, online_prescription.id); //diagnosis == diagnosis_templates
                }

                //save online prescription medicines to local
                if (online_prescription.medicines.Count() > 0)
                {
                    SaveOnlinePrescriptionsMedicinesToLocal(online_prescription.medicines, online_prescription.id);
                }

                //save online prescription tests to local
                if (online_prescription.tests.Count() > 0)
                {
                    SaveOnlinePrescriptionsTestsToLocal(online_prescription.tests, online_prescription.id);
                }
            } 

            if(will_sync_true_diagnosis_template_ids.Count() > 0)
            {
                return true;
            }        
            return false;
        }

        public void SaveOnlinePrescriptionsDiagnosisToLocal(List<ViewDiagnosis> online_prescription_diagnosis, int prescription_id)
        {
            foreach (var online_prescription_diagnosi in online_prescription_diagnosis)
            {
                var have_diagnosis_list = db.diagnosis.FirstOrDefault(x => x.name == online_prescription_diagnosi.diagnosis_list.name);

                if(have_diagnosis_list != null && have_diagnosis_list.diagnosis_template.FirstOrDefault(x => x.doctor_id == MainWindow.Session.doctor_id) != null) //have diagnosis_list and template
                {
                    addPrescription.CreatePrescriptionDiagnosis(prescription_id, have_diagnosis_list.diagnosis_template.Last().id);
                }
            }           
        }

        public void SaveOnlinePrescriptionsMedicinesToLocal(List<ViewNameRule> online_prescription_medicines, int prescription_id)
        {
            foreach (var online_prescription_medicine in online_prescription_medicines)
            {
                var have_medicine = db.medicines.FirstOrDefault(x => x.name == online_prescription_medicine.name);

                if(have_medicine != null)
                {
                   addPrescription.CreatePrescriptionMedicine(prescription_id, have_medicine.id, online_prescription_medicine._joinData.rule);
                }
            }
        }

        public void SaveOnlinePrescriptionsTestsToLocal(List<ViewName> online_prescription_tests, int prescription_id)
        {
            foreach (var online_prescription_test in online_prescription_tests)
            {
                var have_test = db.tests.FirstOrDefault(x => x.name == online_prescription_test.name);

                if (have_test != null)
                {
                    addPrescription.CreatePrescription(prescription_id, have_test.id);
                }
            }
        }

        public async void GetLocalPrescriptions()
        {
            //try
            //{
                var prescriptions = db.prescriptions
                    .Where(x => x.doctor_id == MainWindow.Session.doctor_id && x.is_sync == 0)
                    .Take(100)
                    .ToList();

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(MainWindow.Session.api_base_url);

                //count total sync prescriptions from offline
                online_total = prescriptions.Count();

                if (online_total > 0)
                {
                    var formated_prescriptions = FormatedPrescriptions(prescriptions);

                    HttpResponseMessage response = client.PostAsJsonAsync("admin/prescriptions/get-local-prescriptions?doctor_email=" + MainWindow.Session.doctor_email, formated_prescriptions).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var response_local_prescriptions_save_to_online = response.Content.ReadAsStringAsync();
                        response_local_prescriptions_save_to_online.Wait();

                        var online_response = JsonConvert.DeserializeObject<DiagnosisTemplateSucessMessage>(response_local_prescriptions_save_to_online.Result);

                        if (online_response.status == "success")
                        {
                            online_success = online_response.online_success;

                            ChangeIsSyncLocalPrescriptions(online_response.will_sync_ids);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    }
                }
            //}
            //catch
            //{
            //    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}            
        }

        public List<ViewPrescriptions> FormatedPrescriptions(List<prescription> prescriptions)
        {
            List<ViewPrescriptions> formated_prescriptions = new List<ViewPrescriptions>();

            foreach (var prescription in prescriptions)
            {
                ViewNamePhone user = new ViewNamePhone
                {
                    first_name = prescription.user.first_name,
                    phone = prescription.user.phone
                };


                //Diagnosis list
                List<ViewName> formated_diagnosis = new List<ViewName>();

                foreach (var prescriptions_diagnosis in prescription.prescriptions_diagnosis)
                {
                    ViewName name = new ViewName
                    {
                        name = prescriptions_diagnosis.diagnosis_template.diagnosis.name
                    };

                    formated_diagnosis.Add(name);
                }

                // Medicines
                List<ViewNameRule> formated_medicines = new List<ViewNameRule>();

                foreach (var prescriptions_medicine in prescription.prescriptions_medicine)
                {
                    ViewNameRule nameRule = new ViewNameRule
                    {
                        name = prescriptions_medicine.medicine.name,
                        rule = prescriptions_medicine.rule
                    };

                    formated_medicines.Add(nameRule);
                }


                // Tests
                List<ViewName> tests = new List<ViewName>();

                foreach (var prescriptions_test in prescription.prescriptions_test)
                {
                    ViewName name = new ViewName
                    {
                        name = prescriptions_test.test.name
                    };

                    tests.Add(name);
                }

                ViewPrescriptions formated_prescription = new ViewPrescriptions
                {
                    id = prescription.id,
                    user = user,
                    temperature = prescription.temperature,
                    blood_pressure = prescription.blood_pressure,
                    doctores_notes = prescription.doctores_notes,
                    other_instructions = prescription.other_instructions,
                    formated_diagnosis = formated_diagnosis,
                    formated_medicines = formated_medicines,
                    tests = tests
                };

                formated_prescriptions.Add(formated_prescription);
            }
            return formated_prescriptions;
        }

        public void ChangeIsSyncLocalPrescriptions(List<long> will_sync_ids)
        {
            foreach (var will_sync_id in will_sync_ids)
            {
                var prescription = db.prescriptions.FirstOrDefault(x => x.id == will_sync_id);
                prescription.is_sync = 1;
                db.SaveChanges();
            }
        }
    }
}
