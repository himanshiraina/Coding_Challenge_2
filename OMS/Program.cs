using OMS.Entities;
using OMS.Repository;
using System.Runtime.Intrinsics.X86;


IOrderManagementRepository orderManagementRepository = new OrderProcessorRepository();
Users user1 = new Users();


Electronics ele = new Electronics();

Clothing clt = new Clothing();

List<Products> pp1 = new List<Products>();

Console.WriteLine("----Enter User details----\n\n");


Console.WriteLine("Enter User ID");
user1.UserID = int.Parse(Console.ReadLine());
Console.WriteLine("Username");
user1.UserName = Console.ReadLine();
Console.WriteLine("Password");
user1.Password = Console.ReadLine();
Console.WriteLine("Role");
user1.Role = Console.ReadLine();

int status = orderManagementRepository.createUser(user1);


List<Products> allproducts = new();
List<Products> productsbyuser = new();

if (status >= 1)
{
    Console.WriteLine("User Created Successfully ........");
}
Console.Clear();
string ch = "y";
do
{
    Console.Clear();
    Console.WriteLine("--- Order Management System ---\n\n");
    Console.WriteLine("Enter the Choice .... \n 1 for Create Product\n 2 For Create Order\n 3 For Get all Products\n 4 for Get Order by User\n 5 for Cancel Order\n 6 For Exit ...");
    int option = int.Parse(Console.ReadLine());
    switch (option)
    {
        
        case 1:
            Console.WriteLine("\n\nEnter number of products");
            int count = int.Parse(Console.ReadLine());
           ;
            while (count > 0)
            {
                Console.Clear();
                Products product1 = new Products();

                Console.WriteLine("Enter Product ID :");
                int ProductID = int.Parse(Console.ReadLine());
                Console.WriteLine("Product Name");
                string ProductName = Console.ReadLine();
                Console.WriteLine("Description");
                string Description = Console.ReadLine();
                Console.WriteLine("Price");
                decimal Price = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Quantity in stock");
                int QuantityInStock = int.Parse(Console.ReadLine());
                Console.WriteLine("Select the option : \n 1 for Product Type = Electronics \n 2 for Product Type = Clothing\n ");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Welcome in Electronics Product Type\n");
                        Console.WriteLine("Enter the brand: \n");
                        string brand = Console.ReadLine();
                        Console.WriteLine("Enter the warrantyPeriod: \n");
                        int wp = int.Parse(Console.ReadLine());
                        ele = new Electronics
                        {
                            productId = ProductID,
                            productName = ProductName,
                            description = Description,
                            price = Price,
                            quantityInStock = QuantityInStock,
                            type = "Electronics",
                            Brand = brand,
                            WarrantyPeriod = wp
                        };
                        orderManagementRepository.createProduct(user1, ele);
                        break;
                    case 2:

                        Console.WriteLine("Welcome in Clothing Product Type\n");
                        Console.WriteLine("Enter the size: \n");
                        int size1 = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter the colour: \n");
                        string cl1 = Console.ReadLine();
                        clt = new Clothing
                        {
                            productId = ProductID,
                            productName = ProductName,
                            description = Description,
                            price = Price,
                            quantityInStock = QuantityInStock,
                            type = "Clothing",
                            Size = size1,
                            Color = cl1
                        };
                        orderManagementRepository.createProduct(user1, clt);
                        break;
                }

                count--;
            }
            break;


        case 2:

            orderManagementRepository.CreateOrder(user1);

            break;

        case 3:

            Console.WriteLine("All Products ....");
            allproducts = orderManagementRepository.getAllProducts();
            foreach (Products product2 in allproducts)
            {

                Console.WriteLine($"\nProduct ID : {product2.productId}" + $"Product Name : {product2.productName}" + $"Product Description : {product2.description}" + $"Product Price : {product2.price}" + $"Product Quantity_in_Stock : {product2.quantityInStock}" + $"Product Type : {product2.type}");

            }

            break;
        case 4:

            Console.WriteLine("Get Order By User ....");
            productsbyuser = orderManagementRepository.getOrderByUser(user1);
            foreach (Products product3 in productsbyuser)
            {

                Console.WriteLine($"Product ID : {product3.productId}");

                Console.WriteLine($"Product Name : {product3.productName}");

                Console.WriteLine($"Product Description : {product3.description}");

                Console.WriteLine($"Product Price : {product3.price}");

                Console.WriteLine($"Product Quantity_in_Stock : {product3.quantityInStock}");

                Console.WriteLine($"Product Type : {product3.type}");

            }

            break;

        case 5:

            Console.WriteLine("\nEnter user id to delete\n");
            int uuiidd = int.Parse(Console.ReadLine());
            Console.WriteLine("\nEnter order id to delete\n");
            int ooiidd = int.Parse(Console.ReadLine()); 

            orderManagementRepository.cancelOrder(uuiidd, ooiidd);

            break;

            case 6:
            Console.Clear();
            Console.WriteLine("Thankyou for using my OMS application");
            Environment.Exit(0);
            break;

    }

    Console.WriteLine(" \n------- Do you want to continue ... (y/n) ------\n ");
    ch = Console.ReadLine();
    Console.Clear();

} while (ch == "y");
