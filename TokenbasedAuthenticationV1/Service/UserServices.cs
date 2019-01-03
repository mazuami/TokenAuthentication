using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TokenbasedAuthenticationV1.Models;

namespace TokenbasedAuthenticationV1.Service
{
    public class UserServices
    {
        public User GetUserByCredentials(string email, string password)
        {
            User user = new User() { Id = "1", Email = "email@domain.com", Password = "password", Name = "Ole Petter Dahlmann" };
            if (user != null)
            {
                user.Password = string.Empty;
            }
            return user;
        }
    }
}