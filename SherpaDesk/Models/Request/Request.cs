using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    public abstract class Request : ObjectBase
    {
        public abstract IList<ValidationResult> Validate();
    }

    public sealed class Request<T> : Request where T : class
    {
        public Request(T data)
        {
            this.Data = data;
        }

        [Details]
        public T Data { get; set; }

        public override IList<ValidationResult> Validate()
        {
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(
                    this.Data,
                    new ValidationContext(this.Data),
                    results,
                    true))
                return results;
            else
                return new List<ValidationResult>();
        }
    }

    [DataContract]
    internal sealed class EmptyRequest { }
}
