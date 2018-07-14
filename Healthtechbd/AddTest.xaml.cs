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
    /// Interaction logic for AddTest.xaml
    /// </summary>
    public partial class AddTest : Page
    {
        public AddTest()
        {
            InitializeComponent();
        }

        ContextDb db = new ContextDb();
        test tbl = new test();

        private void SubmitAddTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Tests.xaml", UriKind.Relative));

            tbl.name = txtTestName.Text;
            db.tests.Add(tbl);
            txtTestName.Clear();
            db.SaveChanges();

            MessageBox.Show("Test Save Successfully");            
        }
    }
}
