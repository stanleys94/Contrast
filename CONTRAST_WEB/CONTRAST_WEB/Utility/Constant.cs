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
        public static string Baseurl = "http://10.86.110.96:89/";
        //QA
        //public static string Baseurl = "http://10.85.26.25/";
        //Production
        //public static string Baseurl = "http://10.185.48.83:444/";

            
        //dev
        public static string PhotoFolder = "http://10.85.40.68:91/PhotoFolder/";
        //local
        //public static string PhotoFolder = "http://10.86.110.92:56552/PhotoFolder/";
        //QA
        //public static string PhotoFolder = "http://10.85.25.24/PhotoFolder/";
        //Production
        //public static string PhotoFolder = "https://passport.toyota.astra.co.id:5006/PhotoFolder/";

        public static string ImgPath = "~/img";
        public static string TravelExecutionReceiptFolder = "~/ExecutionFolder";
        public static string TravelSettlementReceiptFolder = "~/SettlementFolder";
        public static string TPhotoEmployeeFolder = "~/PhotoFolder";
        public static string LogFolder = "~/Log";


        public static CultureInfo culture = new CultureInfo("id-ID");

    }
}