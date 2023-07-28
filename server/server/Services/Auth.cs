using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Auth;
using Grpc.Core;
using View;
using GRPCServer.ViewDesigner;
using GRPCServer.Views;
using Authenticity;

namespace GRPCServer.Services
{
    internal class Authauthorization : AuthorizationService.AuthorizationServiceBase
    {
        public static List<Token> TokenList = new List<Token>();
        public override async Task<Token> LogIn(User user, ServerCallContext context)
        {
            VQuery vQuery = new VQuery
            {
                Statement = Statement.Select,
                Conditions = { 
                    new VCondition { ColumnName = "login", Operators = Operator.Equally, Value = new VCell { StringValue = user.Log } },
                    new VCondition { ColumnName = "password", Operators = Operator.Equally, Value = new VCell { StringValue = user.Pass } },
                },
            };

            vQuery.UnitOperators.Add(UnitOperator.And);
            vQuery.UnitOperators.Add(UnitOperator.NoneUnitOperator);

            VRequest vRequest = new VRequest() { VName = "users", Query = vQuery };
            ACommonView aCommonView = ViewManager.GetRegisteredViewByName(vRequest.VName);
            VResponse vResponse = aCommonView != null ? await Task.FromResult(aCommonView.ShowViewRecordAsync(vRequest)).Result : null;

            if ((vResponse != null) && (vResponse.Grid.Rows != null) && (vResponse.Grid.Rows.Count != 0))
            {
                Token token = new Token() { Value = vResponse.Grid.Rows[0].Cells[0].UUIDValue };
                lock (TokenList)
                {
                    TokenList.Add(token);
                }
                return token;
            }
            else
            {
                throw new RpcException(new Status(StatusCode.Cancelled, "user not found"));
            }
        }
        /*
        public override async Task<Token> LogOut(User request, ServerCallContext context)
        {



            return new Token() { Value = DateTime.Now.Ticks.ToString() };
        }
        */
    }
}
