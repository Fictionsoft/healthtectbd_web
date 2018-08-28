using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfChosenControl;
using WpfChosenControl.model;

namespace Healthtechbd
{
    public class MainModel : ViewModelBase
    {
        private ObservableCollection<IdNameModel> _medicinesLists;
        private List<IdNameModel> _selectedMedicines;


        public ObservableCollection<IdNameModel> MedicinesLists
        {
            get
            {
                return _medicinesLists;
            }
            set
            {
                _medicinesLists = value;
                //NotifyPropertyChanged("MedicinesLists");
            }
        }

        public List<IdNameModel> SelectedMedicines
        {
            get
            {
                return _selectedMedicines;
            }
            set
            {
                _selectedMedicines = value;
                //NotifyPropertyChanged("SelectedItems");
            }
        }

        ContextDb db = new ContextDb();
        public MainModel()
        {
            var medicines = new ObservableCollection<IdNameModel>();

            var medicinesLists = db.medicines.OrderByDescending(x => x.created).Take(10).ToList();

            foreach (var medicine in medicinesLists)
            {
                medicines.Add(new IdNameModel() { Id = medicine.id, Name = medicine.name });
            }

            MedicinesLists = medicines;

            var selected_medicines = new List<IdNameModel>();
            SelectedMedicines = selected_medicines;


        }

        public MainModel(int diagonosis_template_id) : this()
        {
            
            //this.SelectedMedicines = students;

            //var existing_items = new ObservableCollection<IdNameModel>();
            //var existing_medicines = db.diagnosis_medicines
            //.Where(x => x.diagnosis_id == diagonosis_template_id)
            //.Select(x => new
            //{
            //    Id = x.medicine.id,
            //    Name = x.medicine.name
            //})
            //.ToList();

            //if (existing_medicines.Count() > 0)
            //{
            //    foreach (var existing_medicine in existing_medicines)
            //    {
            //        existing_items.Add(new IdNameModel() { Id = existing_medicine.Id, Name = existing_medicine.Name });
            //    }
            //}
            //MedicinesLists = existing_items;
            //var medecines = new List<IdNameModel>();
            //medecines.AddRange(existing_items);
            //this.SelectedMedicines = medecines;
        }
    }
}

