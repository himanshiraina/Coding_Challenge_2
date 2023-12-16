using OMS.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repository
{
    internal interface IOrderManagementRepository
    {
        void CreateOrder(Users user);
        void cancelOrder(int userId, int orderId);
        void createProduct(Users user, Products product);
        int createUser(Users user);
        List<Products> getAllProducts();
        List<Products> getOrderByUser(Users user);
    }
}
