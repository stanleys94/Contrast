using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.CustomValidator
{
    public class SettlementDateValidator : ValidationAttribute
    {
        public SettlementDateValidator() { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var model = (Models.SettlementHelper)validationContext.ObjectInstance;

            List<tb_r_travel_request> request = new List<tb_r_travel_request>();
            request = GetData.TravelRequestDateTimeList(model.TravelRequest.no_reg.ToString());

            
            if (model.extend_flag==true)
            {
                
                if (value == null)
                {
                    return new ValidationResult("Invalid date : Null value are not allowed");
                }
                var dt = (DateTime)value;
                if (dt.Ticks == 0)
                {
                    return new ValidationResult("Invalid date : Null value are not allowed");
                }
              

                //if (dt <= DateTime.Now)
                //{
                //    return new ValidationResult("Invalid date : Back date or same day request are not allowed");
                //}


                //if (model.tend_date0!=null&&dt > model.tend_date0)
                if (DateTime.Compare((DateTime)model.End_Extend, dt) < 0)
                {
                    return new ValidationResult("Invalid date : Start date must be earlier than end date");
                }

                if (model.ReceiptFileTransportation != null && model.TransportationSettlement != 0 )
                {
                    return new ValidationResult("Please upload attachment if you reimbursment");
                }


                //for (int i = 0; i < request.Count(); i++)
                //{
                //    if (dt >= request[i].start_date && dt <= request[i].end_date)
                //    {
                //        string error_string = "Invalid date : " + request[i].start_date.Value.ToShortDateString() + " to " + request[i].end_date.Value.ToShortDateString() + " are used by " + request[i].group_code;

                //        return new ValidationResult(error_string);
                //    }
                //}

            }



            return ValidationResult.Success;
        }
    }

}