﻿using SherpaDesk.Models;
using System.Linq;
using SherpaDesk.Models.Response;
using System.Collections.Generic;

namespace SherpaDesk.Common
{
    public static class ResponseExtensions
    {
        public static TResponse Invalid<TResponse>(this TResponse response, params string[] messages)
            where TResponse : SherpaDesk.Models.Response.Response
        {
            response.Status = eResponseStatus.Invalid;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }

        public static TResponse Fail<TResponse>(this TResponse response, params string[] messages)
            where TResponse : SherpaDesk.Models.Response.Response
        {
            response.Status = eResponseStatus.Fail;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }


        public static TResponse Error<TResponse>(this TResponse response, params string[] messages)
            where TResponse : SherpaDesk.Models.Response.Response
        {
            response.Status = eResponseStatus.Error;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }

        public static bool IsSingle(this IEnumerable<OrganizationResponse> list)
        {
            return list.Count() == 1 && list.First().Instances.Count() == 1;
        }

    }
}
