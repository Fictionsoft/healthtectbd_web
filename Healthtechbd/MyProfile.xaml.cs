﻿using System;
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

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for MyProfile.xaml
    /// </summary>
    public partial class MyProfile : Page
    {

        contextd_db db = new contextd_db();
        user doctor = new user();

        public MyProfile()
        {
            InitializeComponent();

            LoadMyProfile();
        }       
        
        void LoadMyProfile()
        {
            try
            {
                doctor = db.users.FirstOrDefault(x => x.id == MainWindow.Session.doctor_id);

                FirstName.Text = doctor.first_name;
                LastName.Text = doctor.last_name;
                Email.Text = doctor.email;
                Phone.Text = doctor.phone;
                AddressLine1.Text = doctor.address_line1;
                AddressLine2.Text = doctor.address_line2;
                EducationalQualification.Text = doctor.educational_qualification;
                Specialist.Text = doctor.specialist;
                ChamberName.Text = doctor.cember_name;
                ChamberAddress.Text = doctor.cember_address;
                OffDay.Text = doctor.off_day;
                VisitingTime.Text = doctor.visiting_time;
                ClinicName.Text = doctor.clinic_name;
                Website.Text = doctor.website;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void SubmitMyProfile_Click(object sender, RoutedEventArgs e)
        {   
            if(FirstName.Text != "" && LastName.Text != "" && Phone.Text != "")
            {
                //try
                //{
                    NavigationService.Navigate(new Uri("MyProfile.xaml", UriKind.Relative));

                    var havePhone = db.users.FirstOrDefault(x => x.phone == Phone.Text && x.id != MainWindow.Session.doctor_id);

                    if (havePhone == null)
                    {
                        doctor = db.users.FirstOrDefault(x => x.id == MainWindow.Session.doctor_id);                        
                        doctor.first_name = FirstName.Text;
                        doctor.last_name = LastName.Text;
                        //doctor.email = Email.Text;
                        doctor.phone = Phone.Text;
                        doctor.address_line1 = AddressLine1.Text;
                        doctor.address_line2 = AddressLine2.Text;
                        doctor.educational_qualification = EducationalQualification.Text;
                        doctor.specialist = Specialist.Text;
                        doctor.cember_name = ChamberName.Text;
                        doctor.cember_address = ChamberAddress.Text;
                        doctor.off_day = OffDay.Text;
                        doctor.visiting_time = VisitingTime.Text;
                        doctor.clinic_name = ClinicName.Text;
                        doctor.website = Website.Text;

                        db.SaveChanges();

                        //Change Session Info.................
                        MainWindow.Session.doctor_first_name = FirstName.Text;
                        MainWindow.Session.doctor_last_name = LastName.Text;
                        MainWindow.Session.doctor_phone = Phone.Text;
                        MainWindow.Session.doctor_email = Email.Text;

                        if (MessageBox.Show("Profile has been updated", "Success") == MessageBoxResult.OK)
                        {
                            TextBlock UserName = AdminPanelWindow.userName;
                            UserName.Text = MainWindow.Session.doctor_first_name + " " + MainWindow.Session.doctor_last_name;
                        }                        
                    }
                    else
                    {
                        MessageBox.Show("The Phone Number already exist", "Already Exit");
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

        private void CancelMyProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }
    }
}
