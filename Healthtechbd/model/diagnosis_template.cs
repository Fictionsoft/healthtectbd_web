namespace Healthtechbd.model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wpf_healthtechbd.diagnosis_templates")]
    public partial class diagnosis_templates
    {
        public int id { get; set; }

        public int doctor_id { get; set; }

        public int diagnosis_id { get; set; }
        [ForeignKey("diagnosis_id")]
        public virtual diagnosis diagnosis { get; set; }

        [StringLength(222)]
        public string instructions { get; set; }

        public bool status { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime created { get; set; }
    }
}
