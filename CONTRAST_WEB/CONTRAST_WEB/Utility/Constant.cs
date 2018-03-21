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
        public static string PhotoFolder = "http://10.85.40.68:91/PhotoFolder/";
        //local
        //public static string PhotoFolder = "http://10.86.110.92:56552/PhotoFolder/";
        //QA
        //public static string PhotoFolder = "http://10.85.25.24/PhotoFolder/";
        //Production
        //public static string PhotoFolder = "https://passport.toyota.astra.co.id:5006/PhotoFolder/";

        //local
        public static string DocumentFolder = "http://10.86.110.96:88/TravelDocuments/";


        public static string ImgPath = "~/img";
        public static string TravelExecutionReceiptFolder = "~/ExecutionFolder";
        public static string TravelSettlementReceiptFolder = "~/SettlementFolder";
        public static string TPhotoEmployeeFolder = "~/PhotoFolder";
        public static string LogFolder = "~/Log";
        public static string TravelDocumentsFolder = "~/TravelDocuments";


        public static CultureInfo culture = new CultureInfo("id-ID");

    }
}