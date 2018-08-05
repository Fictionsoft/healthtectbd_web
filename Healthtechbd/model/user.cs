namespace Healthtechbd.model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wpf_healthtechbd.users")]
    public partial class user
    {
        public int id { get; set; }
       
        public int role_id { get; set; }

        public int doctor_id { get; set; }

        [StringLength(255)]
        public string first_name { get; set; }

        [StringLength(255)]
        public string last_name { get; set; }        

        [StringLength(255)]
        public string email { get; set; }

        [StringLength(255)]
        public string phone { get; set; }

        [StringLength(255)]
        public string address_line1 { get; set; }

        [StringLength(255)]
        public string address_line2 { get; set; }

        [StringLength(255)]
        public string clinic_name { get; set; }

        [StringLength(255)]
        public string website { get; set; }

        public string educational_qualification { get; set; }

        [StringLength(10)]
        public string age { get; set; }

        [StringLength(255)]
        public string password { get; set; }      

        [Column(TypeName = "timestamp")]
        public DateTime created { get; set; }
    }
}
