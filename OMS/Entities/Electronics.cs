using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Entities
{
    internal class Electronics : Products
    {
        public string Brand { get; set; }
        public int WarrantyPeriod { get; set; }


        public Electronics() {}

        public Electronics(int productid, string productname, string description, decimal price, int quantitystock, string type, string brand, int wp) :  base(productid, productname, description, price, quantitystock, type)
        {
            
        
            Brand = brand;
            WarrantyPeriod = wp;

        }



    }
}
