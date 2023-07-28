using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GRPCServer.db;
using View;

namespace GRPCServer.ViewDesigner
{
    internal abstract class ACommonView : IView
    {
        public abstract string Name
        {
            get;
        }

        public abstract ICommonViewDescriptor ViewDescriptor
        {
            get;
        }

        public async Task<VResponse> AddViewRecordAsync(VRequest request)
        {
            if (request.Query.Statement != Statement.Insert)
            {
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "VQury.Statement value " + Convert.ToString(request.Query.Statement) + " does not match requested operation"));
            }

            string SQLQuery = BuildSql(request);
            SQLQuery += " RETURNING " + QuotedStr("id");
            Dictionary<string, object> parameters = CreateSqlParameters(request);                         
            string ID = "";
            
            using (DBSession dBSession = new DBSession())
            {
                dBSession.GetConn();
                using (Task<Npgsql.NpgsqlDataReader> reader = await Task.FromResult(dBSession.ExecuteReaderAsync(SQLQuery, parameters)))
                {
                    if ((reader.Result != null) && (reader.Result.HasRows))
                    {
                        while (reader.Result.Read())
                        {
                            ID = reader.Result["id"].ToString();
                        }
                    }
                    else
                    {
                        throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "result is null"));
                    }
                };
            }

            VRequest vRequest = new VRequest()
            {
                Token = request.Token,
                VName = request.VName,
                Query = new VQuery() { 
                    Statement = Statement.Select,
                    Conditions = { new VCondition() { ColumnName = "id", Operators = View.Operator.Equally, Value = new VCell() { UUIDValue = ID } } }
                }
            };
            VResponse vResponse = await ShowViewRecordAsync(vRequest);
            return vResponse;
        }

        public async Task<VResponse> DeleteViewRecordAsync(VRequest request)
        {
            if (request.Query.Statement != Statement.Delete)
            {
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "VQury.Statement value " + Convert.ToString(request.Query.Statement) + " does not match requested operation"));
            }

            string SQLQuery = BuildSql(request);
            SQLQuery += " RETURNING " + QuotedStr("id");
            Dictionary<string, object> parameters = CreateSqlParameters(request);

            VResponse vResponse = new VResponse()
            {
                VName = request.VName,
                Grid = new VGrid(),
            };

            using (DBSession dBSession = new DBSession())
            {
                dBSession.GetConn();
                using (Task<Npgsql.NpgsqlDataReader> reader = await Task.FromResult(dBSession.ExecuteReaderAsync(SQLQuery, parameters)))
                {
                    if ((reader.Result != null) && (reader.Result.HasRows))
                    {
                        while (reader.Result.Read())
                        {
                            vResponse.Grid.Columns.Add(GetBuilderCollectionItemByName("id"));
                            vResponse.Grid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = reader.Result["id"].ToString() } } });
                        }
                    }
                    else
                    {
                        throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "result is null"));
                    }
                };
            }
            return vResponse;
        }

        public async Task<VResponse> ShowViewRecordAsync(VRequest request)
        {
            if (request.Query.Statement != Statement.Select)
            {
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "VQury.Statement value " + Convert.ToString(request.Query.Statement) + " does not match requested operation"));
            }

            string SQLQuery = BuildSql(request);

            Dictionary<string, object> parameters = CreateSqlParameters(request);

            VResponse response = new VResponse
            {
                Grid = new VGrid()
            };

            ColumnBuilderCollection columnBuilders = new ColumnBuilderCollection();

            foreach (IColumnBuilder c in ViewDescriptor.BuilderCollection)
            {
                if ((request.Query != null) && (request.Query.Type == View.Type.All))
                {
                    columnBuilders.Add(c);
                    response.Grid.Columns.Add(c.ColumnInfo);
                }
                else
                {
                    foreach(var a in request.Query.ColumnNames)
                    {
                        if (a == c.ColumnInfo.Name)
                        {
                            columnBuilders.Add(c);
                            response.Grid.Columns.Add(c.ColumnInfo);
                        }
                    }
                }
            }

            using (DBSession dBSession = new DBSession())
            {
                dBSession.GetConn();
                using (Task<Npgsql.NpgsqlDataReader> reader = await Task.FromResult(dBSession.ExecuteReaderAsync(SQLQuery, parameters)))
                {
                    if ((reader.Result != null) && (reader.Result.HasRows))
                    {
                        while (reader.Result.Read())
                        {
                            VRow vRow = new VRow();

                            foreach (IColumnBuilder columnBuilder in columnBuilders)
                            {
                                vRow.Cells.Add(columnBuilder.BuildVCell(reader.Result));
                            }
                            response.Grid.Rows.Add(vRow);
                        }
                    }
                    else
                    {
                        // "result is null";
                    }
                };
            }
            return response;
        }

        public async Task<VResponse> UpdateViewRecordAsync(VRequest request)
        {
            if (request.Query.Statement != Statement.Update)
            {
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "VQury.Statement value " + Convert.ToString(request.Query.Statement) + " does not match requested operation"));
            }

            string SQLQuery = BuildSql(request);

            Dictionary<string, object> parameters = CreateSqlParameters(request);

            VResponse response = new VResponse
            {
                Grid = new VGrid()
            };

            VRow vOneRow = new VRow();
            foreach (var c in request.Query.Conditions)
            {
                VColumnInfo vColumnInfo = GetBuilderCollectionItemByName(c.ColumnName);
                response.Grid.Columns.Add(vColumnInfo);
                vOneRow.Cells.Add(c.Value);
            }
            response.Grid.Rows.Add(vOneRow);

            using (DBSession dBSession = new DBSession())
            {
                dBSession.GetConn();
                using (Task<bool> reader = await Task.FromResult(dBSession.ExecuteNonQueryAsync(SQLQuery, parameters)))
                {
                    if (!reader.Result)
                    {
                        throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "result is null"));
                    }
                };
            }
            return response;
        }

        public string BuildSql(VRequest vRequest)
        {
            string res = "";

            if (vRequest.Query.Statement == Statement.Select)
            {
                res = "select ";

                List<string> left_join = new List<string>();
                if ((vRequest.Query != null) && (vRequest.Query.Type == View.Type.All))
                {
                    foreach (IColumnBuilder c in ViewDescriptor.BuilderCollection)
                    {
                        if ((c.ColumnInfo.HColumns != null) && (c.ColumnInfo.HColumns.VName != null) && (c.ColumnInfo.HColumns.VName != null))
                        {
                            string leftjoin = " left join " + QuotedStr(c.ColumnInfo.HColumns.VName) + " on " + QuotedStr(vRequest.VName) + "." + QuotedStr(c.ColumnInfo.Name) + " = " + QuotedStr(c.ColumnInfo.HColumns.VName) + "." + QuotedStr("id");

                            bool isLeftJoin = false;

                            foreach (var lg in left_join)
                            {
                                if (lg.Contains(" left join " + QuotedStr(c.ColumnInfo.HColumns.VName) + " on " + QuotedStr(vRequest.VName)))
                                {
                                    isLeftJoin = true;
                                    break;
                                }
                            }

                            if (!isLeftJoin) left_join.Add(leftjoin);

                            res += (res == "select ") ?
                                QuotedStr(c.ColumnInfo.HColumns.VName) + "." + QuotedStr(c.ColumnInfo.HColumns.ColumnName) + " as " + QuotedStr(c.ColumnInfo.HColumns.VName + "_" + c.ColumnInfo.HColumns.ColumnName)
                                : ", " + QuotedStr(c.ColumnInfo.HColumns.VName) + "." + QuotedStr(c.ColumnInfo.HColumns.ColumnName) + " as " + QuotedStr(c.ColumnInfo.HColumns.VName + "_" + c.ColumnInfo.HColumns.ColumnName);
                        }
                        else
                        {
                            res += (res == "select ") ? QuotedStr(vRequest.VName) + "." + QuotedStr(c.ColumnInfo.Name) : ", " + QuotedStr(vRequest.VName) + "." + QuotedStr(c.ColumnInfo.Name);
                        }
                    }
                }
                else
                {
                    foreach (var c in vRequest.Query.ColumnNames)
                    {
                        if(GetBuilderCollectionItemByName(c) == null)
                        {
                            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "undefined column in BuilderCollection"));
                        }
                        res += (res == "select ") ? QuotedStr(c) : ", " + QuotedStr(c);
                    }
                }
                res += " from " + QuotedStr(vRequest.VName);

                if ((left_join != null) && (left_join.Count > 0))
                {
                    for (int i = 0; i < left_join.Count; i++)
                    {
                        res += left_join[i];
                    }
                }

                if ((vRequest.Query != null) && (vRequest.Query.Conditions != null) && (vRequest.Query.Conditions.Count > 0))
                {
                    res += " where ";
                    for (int i = 0; i < vRequest.Query.Conditions.Count; i++)
                    {
                        VColumnInfo vColumnInfo = GetBuilderCollectionItemByName(vRequest.Query.Conditions[i].ColumnName);

                        if (vColumnInfo == null)
                        {
                            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "undefined column in BuilderCollection"));
                        }
                        else
                        {
                            if (vColumnInfo.HColumns != null)
                            {
                               res += QuotedStr(vColumnInfo.HColumns.VName) + "." + QuotedStr(vColumnInfo.HColumns.ColumnName) + " " + Operator(vRequest.Query.Conditions[i].Operators) + $":param{i}";
                            }
                            else
                            {
                                res += QuotedStr(vRequest.VName) + "." + QuotedStr(vRequest.Query.Conditions[i].ColumnName) + " " + Operator(vRequest.Query.Conditions[i].Operators) + $":param{i}";
                            }

                            if ((vRequest.Query.UnitOperators != null) && (vRequest.Query.UnitOperators.Count > 0) && (vRequest.Query.UnitOperators[i] != UnitOperator.NoneUnitOperator))
                            {
                                res += " " + Unit_Operator(vRequest.Query.UnitOperators[i]) + " ";
                            }
                        }
                    }
                }
            }

            else if (vRequest.Query.Statement == Statement.Update)
            {
                res = "update " + QuotedStr(vRequest.VName) + " set ";

                if (vRequest.Query.Conditions != null && vRequest.Query.Conditions.Count > 0)
                {
                    bool where = false;
                    int unit_index = 0;
                    for (int i = 0; i < vRequest.Query.Conditions.Count; i++)
                    {
                        VColumnInfo vColumnInfo = GetBuilderCollectionItemByName(vRequest.Query.Conditions[i].ColumnName);

                        if (vColumnInfo == null)
                        {
                            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "unregister column name"));
                        }

                        if ((vColumnInfo.HColumns == null) || ((vColumnInfo.HColumns != null) && (vColumnInfo.HColumns.MainColumn == "")))
                        {
                            if (vRequest.Query.Conditions[i].Operators == View.Operator.NoneOperator)
                            {
                                res += QuotedStr(vRequest.Query.Conditions[i].ColumnName) + " " + Operator(View.Operator.Equally) + $" :param{i}";

                                if ((i < (vRequest.Query.Conditions.Count - 1) && (vRequest.Query.Conditions[i + 1].Operators == View.Operator.NoneOperator)))
                                {
                                    VColumnInfo v = GetBuilderCollectionItemByName(vRequest.Query.Conditions[i + 1].ColumnName);
                                    if ((v.HColumns == null) || ((v.HColumns != null) && (v.HColumns.MainColumn == "")))
                                        res += ",";
                                }
                            }
                            else
                            {
                                if (!where) { res += " where "; where = true; }

                                res += QuotedStr(vRequest.Query.Conditions[i].ColumnName) + " " + Operator(View.Operator.Equally) + $" :param{i}";
                                if (i < (vRequest.Query.Conditions.Count - 1)) { res += ","; }

                                if ((vRequest.Query.UnitOperators != null) && (vRequest.Query.UnitOperators.Count > 0))
                                {
                                    res += " " + Unit_Operator(vRequest.Query.UnitOperators[unit_index]) + " ";
                                    unit_index++;
                                }
                            }
                        }
                        
                    }
                }
                else
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "VQury.Statement column is null"));
                }

            }

            else if (vRequest.Query.Statement == Statement.Insert)
            {
                res = "insert into " + QuotedStr(vRequest.VName) + " ( ";
                string column_name = "", value = ") values(";

                if ((vRequest.Query != null) && (vRequest.Query.Conditions != null) && (vRequest.Query.Conditions.Count > 0))
                {
                    for (int i = 0; i < vRequest.Query.Conditions.Count; i++)
                    {
                        VColumnInfo vColumnInfo = GetBuilderCollectionItemByName(vRequest.Query.Conditions[i].ColumnName);

                        if (vColumnInfo == null)
                        {
                            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "unregister column name"));
                        }
                        
                        if (vColumnInfo.Name == "id")
                        {
                            continue;
                        }

                        if ((vColumnInfo.HColumns == null) || ((vColumnInfo.HColumns != null) && (vColumnInfo.HColumns.MainColumn == "")))
                        {
                            column_name += (value == ") values(") ? QuotedStr(vRequest.Query.Conditions[i].ColumnName) : "," + QuotedStr(vRequest.Query.Conditions[i].ColumnName);
                            value += (value == ") values(") ? $":param{i}" : $", :param{i}";
                        }
                    }
                    res += column_name + value + ")";
                }
                else
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "VQury.Statement column is null"));
                }
            }

            else if (vRequest.Query.Statement == Statement.Delete)
            {
                res = "delete from " + QuotedStr(vRequest.VName);

                if (vRequest.Query.Conditions != null && vRequest.Query.Conditions.Count > 0)
                {
                    res += " where ";
                    for (int i = 0; i < vRequest.Query.Conditions.Count; i++)
                    {
                        res += QuotedStr(vRequest.Query.Conditions[i].ColumnName) + " " + Operator(vRequest.Query.Conditions[i].Operators) + $" :param{i}";
                       
                        if (i < (vRequest.Query.Conditions.Count - 1))
                        {
                            if ((vRequest.Query.UnitOperators != null) && (vRequest.Query.UnitOperators.Count > 0) && (vRequest.Query.UnitOperators[i] != UnitOperator.NoneUnitOperator))
                            {
                                res += " " + Unit_Operator(vRequest.Query.UnitOperators[i]) + " ";
                            }
                        }
                    }
                }
                else
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "VConditions is null"));
                }

            }

            return res;
        }

        public static string QuotedStr(string data)
        {
            return "\"" + data.Replace("'", "''") + "\"";
        }

        public static string Operator(Operator curr_operator)
        {
            if (curr_operator == View.Operator.More)
            {
                return ">";
            }
            else if (curr_operator == View.Operator.MoreEqual)
            {
                return ">=";
            }
            else if (curr_operator == View.Operator.MoreEqual)
            {
                return ">=";
            }
            else if (curr_operator == View.Operator.Less)
            {
                return "<";
            }
            else if (curr_operator == View.Operator.LessEqual)
            {
                return "<=";
            }
            else if (curr_operator == View.Operator.Equally)
            {
                return "=";
            }
            else if (curr_operator == View.Operator.NotEqual)
            {
                return "<>";
            }
            else if (curr_operator == View.Operator.Like)
            {
                return "like";
            }
            else
            {
                return curr_operator == View.Operator.Between ? "between" :
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "Operators value " + Convert.ToString(curr_operator) + " does not match requested operation"));
            }
        }

        public static string Unit_Operator(UnitOperator curr_operator)
        {
            if (curr_operator == UnitOperator.And)
            {
                return "and";
            }
            else
            {
                return curr_operator == UnitOperator.Or ? "or" :
                    throw new Exception("Unit_Operator value " + Convert.ToString(curr_operator) + " does not match requested operation");
            }
        }

        public string BuildHintString(VColumnInfo vColumnInfo, VCell vCell)
        {
            string res;
            if (vCell != null)
            {
                res = "select " + QuotedStr("id") + ", " + QuotedStr(vColumnInfo.HColumns.ColumnName) + " from " + QuotedStr(vColumnInfo.HColumns.VName)
                    + " where " + QuotedStr(vColumnInfo.HColumns.ColumnName) + " like% " + vCell.StringValue;
            }
            else res = "select " + QuotedStr("id") + ", " + QuotedStr(vColumnInfo.HColumns.ColumnName) + " from " + QuotedStr(vColumnInfo.HColumns.VName);
            return res;
        }

        protected VColumnInfo GetBuilderCollectionItemByName(string name)
        {
            VColumnInfo vColumnInfo = null;

            foreach (IColumnBuilder c in ViewDescriptor.BuilderCollection)
            {
                if (c.ColumnInfo.Name == name)
                {
                    vColumnInfo = c.ColumnInfo;
                    break;
                }
            }
            return vColumnInfo;
        }

        public async Task<HResponse> HintSqlAsync(HRequest hRequest)
        {
            HResponse response = new HResponse();
            VColumnInfo vColumnInfo = GetBuilderCollectionItemByName(hRequest.HColumns.ColumnName);

            if (vColumnInfo.HColumns == null)
            {
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "undefined column in BuilderCollection"));
            }

            using (DBSession dBSession = new DBSession())
            {
                dBSession.GetConn();
                using (Task<Npgsql.NpgsqlDataReader> reader = await Task.FromResult(dBSession.ExecuteReaderAsync(BuildHintString(vColumnInfo, hRequest.HColumns.Value))))
                {
                    while (reader.Result.Read())
                    {
                        Hint hint = new Hint
                        {
                            Key = DBHelpers.BuildVCell(reader.Result["id"]),
                            Value = DBHelpers.BuildVCell(reader.Result[vColumnInfo.HColumns.ColumnName])
                        };
                        response.Hints.Add(hint);
                    }
                };
            }
            return response;
        }

        private Dictionary<string, object> CreateSqlParameters(VRequest request)
        {
            if ((request.Query != null) && (request.Query.Conditions != null) && (request.Query.Conditions.Count > 0))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                for (int i = 0; i < request.Query.Conditions.Count; i++)
                {
                    if(request.Query.Statement == Statement.Insert)
                    if(request.Query.Conditions[i].ColumnName == "id")
                    {
                        continue;
                    }

                    VColumnInfo vColumnInfo = GetBuilderCollectionItemByName(request.Query.Conditions[i].ColumnName);
                    if ((vColumnInfo.HColumns == null) || ((vColumnInfo.HColumns != null) && (vColumnInfo.HColumns.MainColumn == "")))
                    {
                        VCondition cond = request.Query.Conditions[i];
                        parameters.Add($":param{i}", DBHelpers.GetCellData(cond.Value));
                    }
                }
                return parameters;
            }

            return null;
        }
    }
}