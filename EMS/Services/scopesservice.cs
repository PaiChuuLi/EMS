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
        public List<string> scopes=new List<string>();
        public void getScopes()
        {
            
            string scope = "";            
            StreamReader objReader = new StreamReader(@"G:\EMS\EMS\EMS\emsscopes.ini");
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