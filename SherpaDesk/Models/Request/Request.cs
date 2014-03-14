using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SherpaDesk.Models.Request
{
    public abstract class Request : ObjectBase
    {
        public static Request Empty = new Request<EmptyRequest>(new EmptyRequest());

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
                    results))
                return results;
            else
                return new List<ValidationResult>();
        }
    }


    internal sealed class EmptyRequest
    {
    }
}
