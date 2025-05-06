using BraveCtl.Model;
namespace Bravectl.Service
{
    public interface IBraveAPIService
    {
        Task<BraveResponse?> Search(QueryParameters queryParameters);
    }
}