using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.CustomValidator
{
    public class AttchValidator3 : ValidationAttribute
    {
        public AttchValidator3() { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var model = (Models.SettlementHelper)validationContext.ObjectInstance;

            List<tb_r_travel_request> request = new List<tb_r_travel_request>();
            request = GetData.TravelRequestDateTimeList(model.TravelRequest.no_reg.ToString());

            if (model.ReceiptFileOther == null && model.MiscSettlement != 0)
            {
                return new ValidationResult("Please upload attachment Other if you reimbursment");
            }



            return ValidationResult.Success;
        }
    }
}