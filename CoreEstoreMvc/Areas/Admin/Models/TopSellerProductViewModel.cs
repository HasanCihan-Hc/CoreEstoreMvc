using CoreEstoreMvc.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Areas.Admin.Models
{
    public class TopSellerProductViewModel
    {
        public Product Product { get; set; }
        public int TotalSales { get; set; }


    }
}
