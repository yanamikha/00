using GRPCServer.db;
using GRPCServer.ViewDesigner;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using View;

namespace GRPCServer.Views
{
    public class ViewEvents : ViewService.ViewServiceBase
    {
        public override async Task<VResponse> ViewEventAsync(VRequest vRequest, ServerCallContext context)
        {
            if ((vRequest.Token == null) || (vRequest.Token.Value == ""))
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token is null"));
            }

            if(vRequest == null || vRequest.Query == null || vRequest.Query.Statement == Statement.NoneStatement)
            {
                throw new RpcException(new Status(StatusCode.Cancelled, "VQury.Statement is null"));
            }
            else
            { 
                ACommonView aCommonView = ViewManager.GetRegisteredViewByName(vRequest.VName);
                _ = await Task.FromResult(AuthorizeRequestAsync(vRequest, aCommonView)).Result;

                switch (vRequest.Query.Statement)
                {
                    case Statement.Select:
                        {
                            return aCommonView != null ? await Task.FromResult(aCommonView.ShowViewRecordAsync(vRequest)).Result : null;
                        }
                    case Statement.Insert:
                        {
                            return aCommonView != null ? await Task.FromResult(aCommonView.AddViewRecordAsync(vRequest)).Result : null;
                        }
                    case Statement.Delete:
                        {
                            return aCommonView != null ? await Task.FromResult(aCommonView.DeleteViewRecordAsync(vRequest)).Result : null;
                        }
                    case Statement.Update:
                        {
                            return aCommonView != null ? await Task.FromResult(aCommonView.UpdateViewRecordAsync(vRequest)).Result : null;
                        }
                    default: throw new RpcException(new Status(StatusCode.Cancelled, "VQury.Statement value " + Convert.ToString(vRequest.Query.Statement) + " not exists"));
                }
            }
        }

        public override async Task<HResponse> ShowHintAsync(HRequest hRequest, ServerCallContext context)
        {
            ACommonView aCommonView = ViewManager.GetRegisteredViewByName(hRequest.HColumns.VName);
            return aCommonView != null ? await Task.FromResult(aCommonView.HintSqlAsync(hRequest)).Result : null;
        }

        public override async Task<RVResponse> GetRegisteredViewAsync(Authenticity.Token token, ServerCallContext context)
        {
            return await Task.FromResult(new RVResponse() { RegisteredViews = { ViewManager.GetRegisteredViewNames } });
        }

        private async Task<bool> AuthorizeRequestAsync(VRequest vRequest, ACommonView aCommonView)
        {
            return true;
            string[] permisions = await GetPermisionsAsync(vRequest.Token);
            return permisions.Contains(aCommonView.ViewDescriptor.RequiredPermissions[vRequest.Query.Statement]);
        }

        private async Task<string[]> GetPermisionsAsync(Authenticity.Token token)
        {
            string SQLQuery = $"select users.id, user_groups.permissions from user_groups left join users on users.group_id=user_groups.id where users.id = :param0;";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add(":param0", Guid.Parse(token.Value));
            string[] permisions = null;
            
            using (DBSession dBSession = new DBSession())
            {
                dBSession.GetConn();
                using (Task<Npgsql.NpgsqlDataReader> reader = await Task.FromResult(dBSession.ExecuteReaderAsync(SQLQuery, parameters)))
                {
                    if (reader.Result.HasRows)
                    {
                        reader.Result.Read();
                        permisions = reader.Result["permissions"] as string[];
                    }
                    else
                    {
                        throw new RpcException(new Status(StatusCode.Cancelled, "No access"));
                    }
                };
            }
            return permisions;
        }
    }
}