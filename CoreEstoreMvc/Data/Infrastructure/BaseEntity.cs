using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public abstract  class BaseEntity: AppEntity
    {
        public int Id { get; set; }
        [Display(Name = "Yayında")]
        public bool Enabled { get; set; }
        [Display(Name ="Eklenme T.")]
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }

    }
}
