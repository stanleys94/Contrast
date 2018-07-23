using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public static class Constant
    {
        //dev	       
        //public static string Baseurl = "http://10.85.40.68:90/";
        //local	
        //public static string Baseurl = "http://10.86.110.14:89/";	
        //QA	
        //public static string Baseurl = "http://10.85.26.25/";	
        //Production	
        //public static string Baseurl = "http://10.185.48.83:444/";
        public static string Baseurl = "http://localhost:51687/";


        //dev
        public static string PhotoFolder = "http://10.85.40.68:91/PhotoFolder/";
        //local
        //public static string PhotoFolder = "http://10.86.110.92:56552/PhotoFolder/";
        //QA
        //public static string PhotoFolder = "http://10.85.25.24/PhotoFolder/";
        //Production
        //public static string PhotoFolder = "https://passport.toyota.astra.co.id:5006/photofolder/";

        //Dev
        public static string DocumentFolder = "http://10.85.40.68:91/TravelDocuments/";
        //local
        //public static string DocumentFolder = "http://10.86.110.96:88/TravelDocuments/";
        //Production
        //public static string DocumentFolder = "https://passport.toyota.astra.co.id:5006/TravelDocuments/";

        //Dev
        public static string Attch = "http://10.85.40.68:91/";
        //Prod
        //public static string Attch = "http://passport.toyota.astra.co.id:5006/";

        public static string ImgPath = "~/img";
        public static string TravelExecutionReceiptFolder = "~/ExecutionFolder";
        public static string TravelSettlementReceiptFolder = "~/SettlementFolder";
        public static string TPhotoEmployeeFolder = "~/PhotoFolder";
        public static string LogFolder = "~/Log";
        public static string TravelDocumentsFolder = "~/TravelDocuments";


        public static CultureInfo culture = new CultureInfo("id-ID");

        //dev
        //public static string homeURL = "http://10.85.40.68:91/";

        //production
        //public static string homeURL = "https://passport.toyota.astra.co.id:5006/photofolder/";

        //local
        public static string homeURL = "http://localhost:56552/";
    }
}
