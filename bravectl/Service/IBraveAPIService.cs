using BraveCtl.Model;
namespace Bravectl.Service
{
    public interface IBraveAPIService
    {
        Task<BraveResponse?> GetRequest(QueryParameters queryParameters);
    }
}