using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Entities
{
    internal class Clothing : Products
    {
        public int Size { get; set; }
        public string Color { get; set; }


        public Clothing() { }


        public Clothing(int productid, string productname, string description, decimal price, int quantitystock, string type, int size, string color) : base(productid, productname, description, price, quantitystock, type) 
        { 
        
            Size = size;
            Color = color;


        }

    }
}
