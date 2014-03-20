using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SherpaDesk.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class IntRequired : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Int32)
                if ((int)value == 0)
                    return new ValidationResult(this.ErrorMessage);
                else
                    return ValidationResult.Success;
            else
                return base.IsValid(value, validationContext);
        }
    }
}
