using CONTRAST_WEB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.CustomValidator
{
    public class LocationValidator : ValidationAttribute
    {
        public LocationValidator() { }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {             
            
            return ValidationResult.Success;
        }
    }
        
}