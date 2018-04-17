using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.CustomValidator
{
    public class ParticipantValidator : ValidationAttribute
    {
        public ParticipantValidator() { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (Models.TravelRequestHelper)validationContext.ObjectInstance;

            List<tb_m_vendor_employee> bankNamePart = new List<tb_m_vendor_employee>();

            bankNamePart = GetData.VendorEmployeeValidate(Convert.ToInt32(model.tparticipant));
            if (value != null)
            {
                if (bankNamePart.Count() == 0)
                {

                    return new ValidationResult("This Employee Has No Bank Account");
                }
                else return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }

}