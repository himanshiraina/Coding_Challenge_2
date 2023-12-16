using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Entities
{
    internal class Orders
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }


        public Orders() { }

        public Orders(int orderID, int userID, int productID)
        {
            OrderID = orderID;
            UserID = userID;
            ProductID = productID;
        }
    }
}
