using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CONTRAST_WEB.Models
{
    public class ValidTdate:ValidationAttribute
    {
        public ValidTdate() { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
               
            DateTime?[] arr = new DateTime? []{ }; // declare an empty array
            arr = value as DateTime?[];
            var model = (Models.TravelRequestHelper)validationContext.ObjectInstance;
            List<tb_r_travel_request> request = new List<tb_r_travel_request>();
            request=GetData.TravelRequestDateTimeList(model.employee_info.code);
            

            if (value != null)
            { 
                for (int k = 0; k < arr.Count(); k++)
                {

                    if (k == 0)
                    {
                        if (arr[k] == null)
                        {
                            return new ValidationResult("Invalid date : Null value are not allowed");

                        }
                    }
                    else
                    if (arr[k] == null) break;

                    var dt = (DateTime)arr[k];

                    if (dt <= DateTime.Now)
                    {
                        return new ValidationResult("Invalid date : Back date or same day request are not allowed");
                    }

                    if (arr[k] > model.tend_date[k])
                    {
                        return new ValidationResult("Invalid date : Start date must be earlier than end date");
                    }

                    for (int i = 0; i < request.Count(); i++)
                    {
                        if (arr[k] >= request[i].start_date && arr[k] <= request[i].end_date)
                        {
                            string error_string ="Invalid date : "+request[i].start_date.Value.ToShortDateString()+" to "+request[i].end_date.Value.ToShortDateString() + " are already used by " + request[i].group_code;
                             
                            return new ValidationResult(error_string);
                        }
                    }
                }
                return ValidationResult.Success;
            }
            return new ValidationResult("Invalid date : Null value are not allowed");

        }
    }
}