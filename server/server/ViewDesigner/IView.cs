using System.Threading.Tasks;
using View;

namespace GRPCServer.ViewDesigner
{
    internal interface IView
    {
        Task<VResponse> AddViewRecordAsync(VRequest request);
        Task<VResponse> DeleteViewRecordAsync(VRequest request);
        Task<VResponse> ShowViewRecordAsync(VRequest request);
        Task<VResponse> UpdateViewRecordAsync(VRequest request);
        Task<HResponse> HintSqlAsync(HRequest hRequest);
        string BuildSql(VRequest request);
    }
}