using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfChosenControl;
using WpfChosenControl.model;

namespace Healthtechbd.Model
{
    class DiagnosisMedicineModel : ViewModelBase
    {
        private ObservableCollection<IdNameModel> _medicinesLists;
        public ObservableCollection<IdNameModel> MedicinesLists
        {
            get
            {
                return _medicinesLists;
            }
            set
            {
                _medicinesLists = value;
                NotifyPropertyChanged("MedicinesLists");
            }
        }

        private List<IdNameModel> _selectedMedicines;
        public List<IdNameModel> SelectedMedicines
        {
            get
            {
                return _selectedMedicines;
            }
            set
            {
                _selectedMedicines = value;
                NotifyPropertyChanged("SelectedMedicines");
            }
        }

        //private string _itemId;
        //public string ItemId
        //{
        //    get
        //    {
        //        return _itemId;
        //    }
        //    set
        //    {
        //        _itemId = value;
        //        NotifyPropertyChanged("ItemId");
        //    }
        //}

        contextd_db db = new contextd_db();
        public DiagnosisMedicineModel()
        {
            var medicines = new ObservableCollection<IdNameModel>();

            var medicinesLists = db.medicines.OrderByDescending(x => x.created).Take(10).ToList();

            foreach (var medicine in medicinesLists)
            {
                medicines.Add(new IdNameModel() { Id = medicine.id, Name = medicine.name });
            }

            MedicinesLists = medicines;

            int ItemId = MainWindow.Session.editRecordId;
            if (ItemId > 0)
            {
                LoadExistingItems(ItemId);
                StoreExitingIdsIntoSelectedIds(ItemId);
            }
            else
            {
                var selected_medicines = new List<IdNameModel>();
                SelectedMedicines = selected_medicines;
            }
        }

        public void LoadExistingItems(int ItemId)
        {
            var existing_items = new ObservableCollection<IdNameModel>();
            var existing_medicines = db.prescriptions_medicines
            .Where(x => x.prescription_id == ItemId)
            .Select(x => new
            {
                Id = x.medicine.id,
                Name = x.medicine.name
            })
            .ToList();

            if (existing_medicines.Count() > 0)
            {
                foreach (var existing_medicine in existing_medicines)
                {
                    existing_items.Add(new IdNameModel() { Id = existing_medicine.Id, Name = existing_medicine.Name });
                }
            }
            MedicinesLists = existing_items;
            var medecines = new List<IdNameModel>();
            medecines.AddRange(existing_items);
            this.SelectedMedicines = medecines;
        }

        public void StoreExitingIdsIntoSelectedIds(int ItemId)
        {
            var existing_medicines = db.prescriptions_medicines
            .Where(x => x.prescription_id == ItemId)
            .Select(x => new
            {
                Id = x.medicine.id,
                Name = x.medicine.name
            })
            .ToList();

            if (existing_medicines.Count() > 0)
            {
                foreach (var existing_medicine in existing_medicines)
                {
                    DiagnosisMedicineChosenControl.selectedIds.Add(existing_medicine.Id);
                }
            }
        }
    }
}
