namespace Healthtechbd.model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wpf_healthtechbd.diagnosis")]
    public partial class diagnosis_templates
    {
        public int id { get; set; }

        public int doctor_id { get; set; }

        public int diagnosis_list_id { get; set; }
        [ForeignKey("diagnosis_list_id")]
        public virtual diagnosis diagnosis { get; set; }

        [StringLength(222)]
        public string instructions { get; set; }

        public bool status { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created { get; set; }
    }
}
