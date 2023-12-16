using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Entities
{
    internal class Products
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public int quantityInStock { get; set; }
        public string type { get; set; }


        public Products() { }

        public Products(int productid, string productname, string des, decimal pri, int quantitystock, string pe  ) 
        {

            productId = productid;
            productName = productname;
            description = des;
            price = pri;
            quantityInStock = quantitystock;
            type = pe;

        }



        public override string ToString()
        {
            return $"ProductName:{productName},Description:{description}";
        }
    }
}
