using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Host.IIS
{
    /// <summary>
    /// Summary description for adesign
    /// </summary>
    [WebService(Namespace = "http://www.slin.com.pe/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class adesign : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld(string us, string pwd, string xml_base64)
        {


            return "";
        }
    }
}
