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
    /// Interaction logic for Tests.xaml
    /// </summary>
    public partial class Tests : Page
    {
        public Tests()
        {
            InitializeComponent();

            loadData();
        }

        ContextDb db = new ContextDb();
        test tbl = new test();

        private void ButtonAddTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddTest.xaml", UriKind.Relative));
        }

        void loadData()
        {
            var data = from x in db.tests select x;
            dataGridTests.ItemsSource = data.ToList();
        }

        private void btnDeleteTestRow_Click(object sender, RoutedEventArgs e)
        {
            int test_id = (dataGridTests.SelectedItem as test).id;
            tbl = (from x in db.tests where x.id == test_id select x).First();

            db.tests.Remove(tbl);
            db.SaveChanges();
            loadData();
            MessageBox.Show("Delete Successfully");
        }

        private void btnEditTestRow_Click(object sender, RoutedEventArgs e)
        {
            int test_id = (dataGridTests.SelectedItem as test).id;
            NavigationService.Navigate(new Uri("AddTest.xaml", UriKind.Relative));

            MessageBox.Show(test_id.ToString());
        }
    }
}
