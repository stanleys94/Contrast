using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace CONTRAST_WEB.CustomValidator
{
    public class ParticipantValidator : ValidationAttribute
    {
        public ParticipantValidator() { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (Models.TravelRequestHelper)validationContext.ObjectInstance;
            Regex regex = new Regex(@"^\d$");
      
            List<tb_m_vendor_employee> bankNamePart = new List<tb_m_vendor_employee>();
            if (value != null)
            {
                string val = value.ToString();
                int temp;
                if (int.TryParse(val, out temp))
                {
                    
                    bankNamePart = GetData.VendorEmployeeValidate(Convert.ToInt32(model.tparticipant));
                    if (bankNamePart.Count() == 0)
                    {
                        return new ValidationResult("This Employee Has No Bank Account");
                    }

                    if (model.participants != null)
                    {
                        if (model.participants.Where(m => m.no_reg == temp).Count() > 0)
                        {
                            return new ValidationResult("This Employee Already in Participant List");
                        }
                    }
                }
                else return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }

}