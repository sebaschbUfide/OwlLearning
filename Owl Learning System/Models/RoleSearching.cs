using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mysqltest.Models
{
    public class RoleSearching
    {
        owldbEntities01 owldb = new owldbEntities01();
        
        public int GetRole(String email)
        {
            var query2 = owldb.role_user.Where(a => a.users.email.Equals(email)).FirstOrDefault();

            return query2.role_id;
        }

        public int GetUser(String email)
        {
            var query2 = owldb.users.Where(a => a.email.Equals(email)).FirstOrDefault();

            return query2.user_id;
        }

        public users GetUserInfo(String email)
        {

            var query2 = owldb.users.Where(a => a.email.Equals(email)).FirstOrDefault();

            return query2;
        }

    }
}