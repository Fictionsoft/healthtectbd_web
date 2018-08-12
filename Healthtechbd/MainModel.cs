using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Healthtechbd
{
    public class MainModel : ViewModelBase
    {        
        private ObservableCollection<Student> _items;
        private List<Student> _selectedItems;


        public ObservableCollection<Student> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                //NotifyPropertyChanged("Items");
            }
        }

        public List<Student> Students
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                //NotifyPropertyChanged("SelectedItems");
            }
        }

        model.ContextDb db = new model.ContextDb();
        model.medicine medicine = new model.medicine();        

        public MainModel()
        {
            var items = new ObservableCollection<Student>();

            var medicines = db.medicines.OrderByDescending(x => x.created).ToList();

            foreach (var data in medicines)
            {
                items.Add(new Student() { Id = data.id, Name = data.name });
            }

            Items = items;
            var students = new List<Student>();
            students.Add(Items.First());
            students.Add(Items.LastOrDefault());

            this.Students = students;
        }

        public int test()
        {
            return 1;
        }

        private void Submit()
        {

        }
    }


    public class Student : ViewModelBase
    {

        private int _Id;
        public int Id
        {
            get { return _Id; }

            set
            {
                _Id = value;
                NotifyPropertyChanged("Id");
            }
        }
        private string _Name;
        public string Name
        {
            get { return _Name; }

            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

    }
}

