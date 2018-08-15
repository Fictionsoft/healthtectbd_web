namespace Healthtechbd.model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wpf_healthtechbd.diagnosis_lists")]
    public partial class diagnosis
    {
        //public diagnosis()
        //{
        //    this.diagnosis_templates = new HashSet<diagnosis_templates>();
        //}

        public int id { get; set; }        

        [Required]
        [StringLength(222)]
        public string name { get; set; }

        public bool status { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created { get; set; }      
        
        public virtual ICollection<diagnosis_templates> diagnosis_templates { get; set; }
    }
}
