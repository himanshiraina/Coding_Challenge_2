using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Entities
{
    internal class Users
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }



        public Users() {  }




        public Users(int userID, string userName, string password, string role)
        {
            UserID = userID;
            UserName = userName;
            Password = password;
            Role = role;
        }
    }


}
