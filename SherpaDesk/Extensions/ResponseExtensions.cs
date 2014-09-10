using System.Collections.Generic;
using System.Linq;
using SherpaDesk.Models;
using SherpaDesk.Models.Response;

namespace SherpaDesk.Extensions
{
    public static class ResponseExtensions
    {
        public static TResponse Invalid<TResponse>(this TResponse response, params string[] messages)
            where TResponse : Response
        {
            response.Status = eResponseStatus.Invalid;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }

        public static TResponse Fail<TResponse>(this TResponse response, params string[] messages)
            where TResponse : Response
        {
            response.Status = eResponseStatus.Fail;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }


        public static TResponse Error<TResponse>(this TResponse response, params string[] messages)
            where TResponse : Response
        {
            response.Status = eResponseStatus.Error;

            foreach (var msg in messages)
                response.Messages.Add(msg);

            return response;
        }

        public static bool IsSingle(this IEnumerable<OrganizationResponse> list)
        {
            var organizationResponses = list as IList<OrganizationResponse> ?? list.ToList();
            return organizationResponses.Count() == 1 && organizationResponses.First().Instances.Count() == 1;
        }
    }
}
