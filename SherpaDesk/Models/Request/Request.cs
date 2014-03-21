using SherpaDesk.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SherpaDesk.Models.Request
{
    public abstract class Request : ObjectBase
    {
        public abstract IList<ValidationResult> Validate();
    }

    public sealed class Request<T> : Request where T : IRequestType
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
    public abstract class PostRequest : ObjectBase, IRequestType
    {
        public virtual eRequestType Type
        {
            get { return eRequestType.POST; }
        }

        public bool IsEmpty
        {
            get { return false; }
        }
    }

    [DataContract]
    public abstract class GetRequest : ObjectBase, IRequestType
    {
        public virtual eRequestType Type
        {
            get { return eRequestType.GET; }
        }

        public bool IsEmpty
        {
            get { return false; }
        }
    }

    [DataContract]
    internal sealed class EmptyRequest : GetRequest
    {
        public bool IsEmpty
        {
            get { return true; }
        }
    }
}
