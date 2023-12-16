using OMS.Entities;
using OMS.Exceptions;
using OMS.Utility;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Transactions;

namespace OMS.Repository
{
    internal class OrderProcessorRepository : IOrderManagementRepository
    {
        string connectionstring;
        SqlCommand cmd;
        public OrderProcessorRepository()
        {
            connectionstring = DbConnUtil.GetConnectionString();
            cmd = new SqlCommand();
        }
        public void cancelOrder(int userId, int orderId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    // Check if the userId and orderId exist in the Orders table
                    if (isOrderExists(conn, userId, orderId))
                    {
                        // If the order exists, proceed to cancel it
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Orders WHERE userId = @uid AND orderId = @oid", conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.Parameters.AddWithValue("@oid", orderId);
                            cmd.ExecuteNonQuery();

                            Console.WriteLine("Order is canceled successfully.");
                        }
                    }
                    else
                    {
                        if (!isUserExists(conn, userId))
                        {
                            throw new UserNotFound($"User with userId {userId} is  not found.");
                        }

                        throw new OrderNotFound($"Order with orderId {orderId} not found for userId {userId}.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

// Helper method to check if the userId exists in the Users table
        private bool isUserExists(SqlConnection conn, int userId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT 1 FROM Users WHERE userId = @uid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();
                return (result != null);
            }
        }

// Helper method to check if the orderId and userId exist in the Orders table
        private bool isOrderExists(SqlConnection conn, int userId, int orderId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT 1 FROM Orders WHERE userId = @uid AND orderId = @oid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@oid", orderId);
                object result = cmd.ExecuteScalar();
                return (result != null);
            }
        }



        public void createProduct(Users user, Products product)
        {
            try
            {
                if (user != null && user.UserID > 0) // Ensure user object exists and has a valid UserID
                {
                    using (SqlConnection conn = new SqlConnection(connectionstring))
                    {
                        conn.Open();

                        using (SqlCommand isAdminCmd = new SqlCommand("SELECT 1 FROM Users WHERE userId = @uid AND role = 'Admin'", conn))
                        {
                            isAdminCmd.Parameters.AddWithValue("@uid", user.UserID);
                            object isAdminResult = isAdminCmd.ExecuteScalar();

                            if (isAdminResult != null) // Check if the user has admin privileges
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT INTO Products (productId, productName, description, price, quantityInStock, type) VALUES (@id, @pname, @desc, @price, @quantity, @type)", conn))
                                {
                                    cmd.Parameters.AddWithValue("@id", product.productId);
                                    cmd.Parameters.AddWithValue("@pname", product.productName);
                                    cmd.Parameters.AddWithValue("@desc", product.description);
                                    cmd.Parameters.AddWithValue("@price", product.price);
                                    cmd.Parameters.AddWithValue("@quantity", product.quantityInStock);
                                    cmd.Parameters.AddWithValue("@type", product.type);

                                    int productId = Convert.ToInt32(cmd.ExecuteScalar());

                                    Console.WriteLine($"Product added successfully with productId: {productId}");

                                    if (productId > 0)
                                    {
                                        if (product is Electronics ele)
                                        {
                                            using (SqlCommand elecmd = new SqlCommand("INSERT INTO Electronics (productId, brand, warrantyPeriod) VALUES (@pid, @brand, @warrantyperiod)", conn))
                                            {
                                                elecmd.Parameters.AddWithValue("@pid", productId);
                                                elecmd.Parameters.AddWithValue("@brand", ele.Brand);
                                                elecmd.Parameters.AddWithValue("@warrantyperiod", ele.WarrantyPeriod);

                                                elecmd.ExecuteNonQuery();
                                            }
                                        }
                                        else if (product is Clothing clt)
                                        {
                                            using (SqlCommand cltcmd = new SqlCommand("INSERT INTO Clothing(productId, size, color) VALUES(@pid, @size, @color)", conn))
                                            {
                                                cltcmd.Parameters.AddWithValue("@pid", productId);
                                                cltcmd.Parameters.AddWithValue("@size", clt.Size);
                                                cltcmd.Parameters.AddWithValue("@color", clt.Color);

                                                cltcmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Failed to add the product.");
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("User is not authorized to create products. Admin access required.");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid user.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }



        public int createUser(Users user)
        {
            int createstatus = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("insert into Users (userId, username, password, role) VALUES (@userId, @username, @password, @role)", conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", user.UserID);
                        cmd.Parameters.AddWithValue("@username", user.UserName);
                        cmd.Parameters.AddWithValue("@password", user.Password);
                        cmd.Parameters.AddWithValue("@role", user.Role);
                        createstatus = cmd.ExecuteNonQuery();
                        if (createstatus >= 1) { Console.WriteLine("User Added Successfully"); }
                    }

                }
                
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            return createstatus;
        }

        public List<Products> getAllProducts()
        {
            List<Products> products = new List<Products>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    // Retrieve all products from the Products table
                    using (SqlCommand cmd = new SqlCommand("select * from Products", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Products product = new Products
                                {
                                    productId = Convert.ToInt32(reader["productId"]),
                                    productName = Convert.ToString(reader["productName"]),
                                    description = Convert.ToString(reader["description"]),
                                    price = Convert.ToDecimal(reader["price"]),
                                    quantityInStock = Convert.ToInt32(reader["quantityInStock"]),
                                    type = Convert.ToString(reader["type"])
                                };

                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return products;
        }


        public List<Products> getOrderByUser(Users user)
        {
            List<Products> orderedProducts = new List<Products>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    if (user != null && user.UserID > 0)
                    {
                        string query = "SELECT p.* FROM Products p JOIN Orders o ON p.productId = o.productId WHERE o.userId = @uid";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", user.UserID);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Products product = new Products
                                    {
                                        productId = Convert.ToInt32(reader["productId"]),
                                        productName = Convert.ToString(reader["productName"]),
                                        description = Convert.ToString(reader["description"]),
                                        price = Convert.ToDecimal(reader["price"]),
                                        quantityInStock = Convert.ToInt32(reader["quantityInStock"]),
                                        type = Convert.ToString(reader["type"])
                                    };

                                    orderedProducts.Add(product);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid user or user ID.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return orderedProducts;
        }

        //public void createOrder(Users user, List<Products> products)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(connectionstring))
        //        {
        //            conn.Open();

        //            // Check if the user already exists in the Users table
        //            using (SqlCommand cmd = new SqlCommand("SELECT userId FROM Users WHERE userId = @uid", conn))
        //            {
        //                cmd.Parameters.AddWithValue("@uid", user.UserID);
        //                object uId = cmd.ExecuteScalar();

        //                if (uId == null)
        //                {
        //                    createUser(user); // Create the user if not found
        //                }
        //            }

        //            // Now, get the userId (whether it already existed or was just added)
        //            int userId = getUserId(conn, user.UserID);

        //            // Check if the userId is valid before proceeding
        //            if (userId != -1)
        //            {
        //                // Insert records into the Orders table
        //                using (SqlCommand cmd = new SqlCommand("INSERT INTO Orders (userId, productId) VALUES (@userid, @productid)", conn))
        //                {
        //                    foreach (Products product in products)
        //                    {
        //                        cmd.Parameters.Clear(); // Clear previous parameters
        //                        cmd.Parameters.AddWithValue("@userid", userId); // Use the obtained userId
        //                        cmd.Parameters.AddWithValue("@productid", product.ProductID);
        //                        cmd.ExecuteNonQuery();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("Failed to get a valid userId. Order creation aborted.");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //}














        public void CreateOrder(Users user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    int userId = GetUserId(conn, user.UserID);

                    if (userId == -1)
                    {
                        createUser(conn, user);
                        userId = GetUserId(conn, user.UserID);
                    }

                    if (userId != -1)
                    {
                        Console.WriteLine("Enter Product ID:");
                        int productId = int.Parse(Console.ReadLine());

                        if (ProductExists(conn, productId))
                        {
                            InsertOrder(conn, userId, productId);
                            Console.WriteLine("Order created successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Product does not exist.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to create order. User not found.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Message);
            }
        }

        private int GetUserId(SqlConnection conn, int userId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT userId FROM Users WHERE userId = @uid", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                object result = cmd.ExecuteScalar();

                return (result != null) ? (int)result : -1;
            }
        }

        private bool ProductExists(SqlConnection conn, int productId)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Products WHERE productId = @productId", conn))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

        private void InsertOrder(SqlConnection conn, int userId, int productId)
        {
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Orders (userId, productId) VALUES (@userid, @productid)", conn))
            {
                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@productid", productId);
                cmd.ExecuteNonQuery();
            }
        }



        private void createUser(SqlConnection conn, Users user)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Users (userId, username, password, role) VALUES (@userid, @username, @pass, @role)", conn))
                {
                    cmd.Parameters.AddWithValue("@userid", user.UserID);
                    cmd.Parameters.AddWithValue("@username", user.UserName);

                    cmd.Parameters.AddWithValue("@pass", user.Password);
                    cmd.Parameters.AddWithValue("@role", user.Role);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error creating user: {e.Message}");
            }
        }
    }
}