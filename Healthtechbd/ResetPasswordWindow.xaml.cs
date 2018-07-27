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

        model.ContextDb db = new model.ContextDb();
        model.user user = new model.user();

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

                    MainWindow.Session.userId = user.id;
                    MainWindow.Session.userFirstName = user.first_name;
                    MainWindow.Session.userLastName = user.last_name;
                    MainWindow.Session.userEmail = user.email;

                    if (MessageBox.Show("Password has been reset and Login Successfully", "Success") == MessageBoxResult.OK)
                    {
                        TextBlock UserName = AdminPanelWindow.userName;
                        UserName.Text = MainWindow.Session.userFirstName + " " + MainWindow.Session.userLastName;
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
