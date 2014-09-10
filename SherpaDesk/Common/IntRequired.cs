using System;
using System.ComponentModel.DataAnnotations;

namespace SherpaDesk.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IntRequired : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is Int32)) 
                return base.IsValid(value, validationContext);
            
            return (int)value == 0 ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}
