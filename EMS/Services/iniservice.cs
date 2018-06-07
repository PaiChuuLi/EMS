using System;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Xml.Linq;

namespace EMS.Services
{
    [Serializable]
    public class iniservice
    {
        public string EmsPath { get; set; }
        public string CallbackUrl { get; set; }
        public string SecretKey { get; set; }
        public string ClientId { get; set; }
        public string ExpectedState { get; set; }

        public iniservice(string path)
        {
            this.EmsPath = path;
            XDocument doc = XDocument.Load((EmsPath));
            foreach (XElement el in doc.Root.Elements())
            {
                foreach (XElement element in el.Elements())
                {
                    switch (element.Name.ToString())
                    {
                        case "CallbackURL":
                            CallbackUrl = element.Value;
                            break;

                        case "SecretKey":
                            SecretKey = element.Value;
                            break;
                        case "ClientID":
                            ClientId = element.Value;
                            break;
                        case "ExpectedState":
                            ExpectedState = element.Value;
                            break;
                        default: break;
                    }
                }
            }
        }
    }
}
