using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SherpaDesk.Interfaces
{
    public interface IValidatableObject
    {
         IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}
