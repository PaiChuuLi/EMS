using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Services
{
    public class scopesservice
    {
        string adress { get; set; }
        public scopesservice(string adress) { this.adress = adress; }
        public List<string> scopes=new List<string>();
        public void getScopes()
        {            
            string scope = "";            
            StreamReader objReader = new StreamReader(this.adress);
            do
            {
                scope = objReader.ReadLine();
                if (scope != null)
                    scopes.Add(scope);
            } while (scope != null);
            objReader.Close();            
        }
    }
}