﻿using System.Linq;
using System.Collections.Generic;

namespace SherpaDesk.Models.Response
{
    public enum eResponseStatus
    {
        Success,
        Invalid,
        Fail,
        Error
    }

    public abstract class Response : ObjectBase
    {
        protected Response()
        {
            this.Status = eResponseStatus.Success;
            this.Messages = new List<string>();
        }

        public eResponseStatus Status { get; set; }

        public IList<string> Messages { get; set; }

        public string Message
        {
            get
            {
                return this.Messages.FirstOrDefault() ?? string.Empty;
            }
        }
    }

    public sealed class Response<T> : Response
        where T : class
    {
        [Details]
        public T Data { get; set; }
    }



}