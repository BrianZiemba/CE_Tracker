using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE_Tracker.Model
{
    public class UsersModel : ModelBase
    {

        
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }


        public UsersModel(int Id, string UserName, string Pwd)
        {
           this.Id = Id;
           this.UserName = UserName;   
           this.Pwd = Pwd; 
        }

        public UsersModel() { }

      
    }

   
}
