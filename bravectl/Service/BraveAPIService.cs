using Bravectl.Service;
using BraveCtl.Model;

namespace Bravectl.Service
{
    public class BraveAPIService : IBraveAPIService
    {
        public Task<BraveResponse> GetRequest(QueryParameters queryParameters)
        {
            return Task.Run(() =>
            {
                BraveResponse braveResponse = new();
                return braveResponse;
            });
        }
    }
}