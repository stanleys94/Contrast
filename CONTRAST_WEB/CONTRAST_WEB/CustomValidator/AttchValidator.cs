using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.CustomValidator
{
    public class AttchValidator : ValidationAttribute
    {
        public AttchValidator() { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var model = (Models.SettlementHelper)validationContext.ObjectInstance;

            List<tb_r_travel_request> request = new List<tb_r_travel_request>();
            request = GetData.TravelRequestDateTimeList(model.TravelRequest.no_reg.ToString());


            if (model.ReceiptFileTransportation == null && model.TransportationSettlement != 0)
            {
                return new ValidationResult("Please upload attachment Transportation if you reimbursment");
            }


            return ValidationResult.Success;
        }
    }
}