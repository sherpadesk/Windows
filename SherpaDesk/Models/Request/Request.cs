using SherpaDesk.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Linq;

namespace SherpaDesk.Models.Request
{
    public abstract class Request : ObjectBase
    {
        public abstract IEnumerable<ValidationResult> Validate();
    }

    public sealed class Request<T> : Request where T : IRequestType
    {
        public Request(T data)
        {
            this.Data = data;
        }

        [Details]
        public T Data { get; set; }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (this.Data is IValidatableObject)
            {
                return ((IValidatableObject)this.Data).Validate(new ValidationContext(this.Data)).ToList();
            }
            else
            {
                var results = new List<ValidationResult>();
                if (!Validator.TryValidateObject(
                        this.Data,
                        new ValidationContext(this.Data),
                        results,
                        true))
                    return results;
                else
                    return (new ValidationResult[0]).AsEnumerable();
            }
        }


    }

    [DataContract]
    internal sealed class EmptyRequest : GetRequest
    {
        public override bool IsEmpty
        {
            get { return true; }
        }
    }
}
