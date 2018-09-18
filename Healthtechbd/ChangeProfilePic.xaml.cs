﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
    /// Interaction logic for ChangeProfilePic.xaml
    /// </summary>
    public partial class ChangeProfilePic : Page
    {
        public ChangeProfilePic()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        user user = new user();             

        private void ButtonBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();

            op.Title = "Select a picture";
            op.Filter = "Image Files (*.gif,*.jpg,*.jpeg,*.bmp,*.png)|*.gif;*.jpg;*.jpeg;*.bmp;*.png";

            if (op.ShowDialog() == true)
            {
                ReviewProfilePic.Source = new BitmapImage(new Uri(op.FileName)); //FilePath               
                string imageName = System.IO.Path.GetFileName(op.FileName);

                FilePath.Text = op.FileName; //FileName = FilePath
                CorrectImageName.Text = imageName;
            }
        }

        private void SubmitUpdateProPic_Click(object sender, RoutedEventArgs e)
        {           
            OpenFileDialog op = new OpenFileDialog();
            Random rnd = new Random();
            string fullImageName = rnd.Next() + "_" + CorrectImageName.Text;

            string destination = System.AppDomain.CurrentDomain.BaseDirectory + "images//";            
            System.IO.File.Copy(FilePath.Text, destination + fullImageName);

            user = db.users.Where(x => x.id == MainWindow.Session.doctorId).FirstOrDefault();

            user.profile_picture = fullImageName;
            var uploadProfilePic = db.SaveChanges();

            if (uploadProfilePic > 0)
            {
                MessageBox.Show("Profile picture update successfully", "Success");
                Image ProfilePic = AdminPanelWindow.profilePic;

                ProfilePic.Source = new BitmapImage(new Uri(destination + user.profile_picture));                
            }
            else
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }        
    }
}