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
using System.Windows.Shapes;
using WpfChosenControl.model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        public ResetPasswordWindow()
        {
            InitializeComponent();
        }

        public ResetPasswordWindow(string email) : this()
        {
            EmailAddress.Text = email;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void registrationLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            RegistrationWindow registrationWindow = new RegistrationWindow(this);
            registrationWindow.Show();
        }

        private void forgotPasswordLink_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow(this);
            mainWindow.Show();
        }

        contextd_db db = new contextd_db();
        user user = new user();

        private void ButtonResetPassword_Click(object sender, RoutedEventArgs e)
        {
            if(Password.Password != "" && ConfirmPassword.Password != "")
            {
                if(Password.Password == ConfirmPassword.Password)
                {
                    user = db.users.FirstOrDefault(x => x.email == EmailAddress.Text);

                    user.password = Password.Password;
                    db.SaveChanges();

                    this.Hide();
                    AdminPanelWindow adminPanelWindow = new AdminPanelWindow(this);
                    adminPanelWindow.Show();

                    MainWindow.Session.doctor_id = user.id;
                    MainWindow.Session.doctor_first_name = user.first_name;
                    MainWindow.Session.doctor_last_name = user.last_name;
                    MainWindow.Session.doctor_email = user.email;
                    MainWindow.Session.doctor_phone = user.phone;
                    MainWindow.Session.doctorPrescriptionTemId = user.prescription_template_id;

                    if (MessageBox.Show("Password has been reset and login successfully", "Success") == MessageBoxResult.OK)
                    {
                        TextBlock UserName = AdminPanelWindow.userName;
                        UserName.Text = MainWindow.Session.doctor_first_name + " " + MainWindow.Session.doctor_last_name;

                        Image ProfilePic = AdminPanelWindow.profilePic;

                        if (user.profile_picture != null)
                        {
                            ProfilePic.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "images/" + user.profile_picture));
                        }
                        else
                        {
                            ProfilePic.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "images/defaultProfilePicture.png"));
                        }                        
                    }                    
                }
                else
                {
                    MessageBox.Show("Confirm Password didn't match with Password.", "Discrepancy", MessageBoxButton.OK, MessageBoxImage.Warning);
                }               
            }
            else
            {
                MessageBox.Show("Please fill the all field", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
