using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Services
{
    public class scopesservice
    {
        public scopesservice() { }
        public List<string> scopes { get; set; }
        public void getScopes()
        {
            
            string scope = "";            
            StreamReader objReader = new StreamReader(@"emsscopes.ini");
            while (scope != null)
            {
                scope = objReader.ReadLine();
                if (scope != null)
                    scopes.Add(scope);
            }
            objReader.Close();            
        }
    }
}