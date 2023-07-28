using GRPCServer.Views;
using Grpc.Core;
using GRPCServer.Services;
using BroadCastView;
using System;
using Auth;
using View;
using GRPCServer.db;
using System.Threading.Tasks;
using System.Collections.Generic;
using GRPCServer.ViewDesigner;
using Npgsql;
using System.Data;
using System.Text;
using System.Linq;

namespace GRPCServer
{
    class Program
    {
        readonly static int Port = 5005;
        public static async Task Main(string[] args)
        {
            _ = new ViewManager();
            //await RequiredPermissionsRemakeAsync();
            UpdateDefaultDataInDb();

            Server server = new Server
            {
                Services = {
                    BroadCastService.BindService(new BroadCastServiceImpl()),
                    AuthorizationService.BindService(new Authauthorization()),
                    ViewService.BindService(new ViewEvents())
                },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("GRPC server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
        private static async Task RequiredPermissionsRemakeAsync()
        {

            foreach (KeyValuePair<ACommonView, string> view in ViewManager.RegisteredViews)
            {
                if (view.Key is IVGridData)
                {
                    VGrid vLocalGrid = (view.Key as IVGridData).VGrid;
                    VGrid vDBGrid = new VGrid();
                    VGrid vUpdateGrid = new VGrid();
                    VGrid vInsertGrid = new VGrid();

                    foreach (IColumnBuilder c in view.Key.ViewDescriptor.BuilderCollection)
                    {
                        vDBGrid.Columns.Add(c.ColumnInfo);
                        vUpdateGrid.Columns.Add(c.ColumnInfo);
                        vInsertGrid.Columns.Add(c.ColumnInfo);
                    }

                    using (DBSession dBSession = new DBSession())
                    {
                        string sql = "select * from " + ACommonView.QuotedStr(view.Key.Name);

                        dBSession.GetConn();
                        using (Task<NpgsqlDataReader> reader = await Task.FromResult(dBSession.ExecuteReaderAsync(sql, null)))
                        {
                            if ((reader.Result != null) && reader.Result.HasRows)
                            {
                                while (reader.Result.Read())
                                {
                                    VRow vRow = new VRow();

                                    foreach (IColumnBuilder columnBuilder in view.Key.ViewDescriptor.BuilderCollection)
                                    {
                                        vRow.Cells.Add(columnBuilder.BuildCellColumnNameOnly(reader.Result));
                                    }
                                    vDBGrid.Rows.Add(vRow);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < vLocalGrid.Rows.Count; i++)
                                {
                                    VRow vRow = vLocalGrid.Rows[i];
                                    VQuery vQuery = new VQuery()
                                    {
                                        Statement = Statement.Insert,
                                    };

                                    int column_index = 0;

                                    foreach (VCell cell in vRow.Cells)
                                    {
                                        vQuery.Conditions.Add(new VCondition() { ColumnName = vLocalGrid.Columns[column_index].Name, Value = cell });
                                        column_index++;
                                    }

                                    ACommonView aCommonView = ViewManager.GetRegisteredViewByName(view.Key.Name);
                                    VRequest request = new VRequest
                                    {
                                        Query = vQuery,
                                        VName = view.Key.Name
                                    };

                                    VResponse vResponse = aCommonView != null ? await Task.FromResult(aCommonView.AddViewRecordAsync(request)).Result : null;

                                    if (vResponse == null)
                                    {
                                        throw new RpcException(new Status(StatusCode.Aborted, "result is null"));
                                    }
                                }
                                continue;
                            }
                        };
                    }

                    List<string> UsedID = new List<string>();

                    List<int> Unique = new List<int>();

                    for (int i = 0; i < vLocalGrid.Columns.Count; i++)
                    {
                        if (vLocalGrid.Columns[i].Unique)
                        {
                            Unique.Add(i);
                        }
                    }

                    for (int i = 0; i < vLocalGrid.Rows.Count; i++)
                    {
                        VRow vRow = vLocalGrid.Rows[i];

                        bool insert = true;

                        for (int m = 0; m < vDBGrid.Rows.Count; m++)
                        {
                            bool found = false;

                            for (int q = 0; q < vRow.Cells.Count; q++)
                            {
                                if (vDBGrid.Columns[q].Name == "id")
                                {
                                    continue;
                                }

                                bool _break = false;

                                if (!found)
                                {
                                    foreach (var a in Unique)
                                    {
                                        if (!Equals(DBHelpers.GetCellData(vRow.Cells[a]), DBHelpers.GetCellData(vDBGrid.Rows[m].Cells[a])))
                                        {
                                            _break = true;
                                        }
                                        else insert = false;
                                        found = true;
                                    }
                                }

                                if (_break) break;

                                if (!Equals(DBHelpers.GetCellData(vRow.Cells[q]), DBHelpers.GetCellData(vDBGrid.Rows[m].Cells[q])))
                                {
                                    vRow.Cells[0].UUIDValue = vDBGrid.Rows[m].Cells[0].UUIDValue;
                                    vUpdateGrid.Rows.Add(vRow);
                                    if (UsedID.IndexOf(vDBGrid.Rows[m].Cells[0].UUIDValue) == -1)
                                    {
                                        UsedID.Add(vDBGrid.Rows[m].Cells[0].UUIDValue);
                                    }
                                    break;
                                }
                                else
                                {
                                    if (UsedID.IndexOf(vDBGrid.Rows[m].Cells[0].UUIDValue) == -1)
                                    {
                                        UsedID.Add(vDBGrid.Rows[m].Cells[0].UUIDValue);
                                    }
                                }

                                if (insert && (q == (vRow.Cells.Count - 1)))
                                {
                                    vInsertGrid.Rows.Add(vRow);
                                    break;
                                }
                            }
                        }

                        if (insert)
                        {
                            vInsertGrid.Rows.Add(vRow);
                        }
                    }

                    if (UsedID.Count != vDBGrid.Rows.Count)
                    {
                        foreach (var a in vDBGrid.Rows)
                        {
                            if (UsedID.IndexOf(a.Cells[0].UUIDValue) == -1)
                            {
                                using (DBSession dBSession = new DBSession())
                                {
                                    string sql = "update " + ACommonView.QuotedStr(view.Key.Name) + " set " + ACommonView.QuotedStr("active") + " = false ";
                                    sql += "where" + ACommonView.QuotedStr("id") + "= '" + a.Cells[0].UUIDValue + "'";
                                    sql += " returning " + ACommonView.QuotedStr("id");
                                    dBSession.GetConn();
                                    using (Task<Npgsql.NpgsqlDataReader> reader = await Task.FromResult(dBSession.ExecuteReaderAsync(sql, null)))
                                    {
                                        if ((reader.Result != null) && reader.Result.HasRows)
                                        {
                                            //ok 
                                        }
                                        else
                                        {
                                            throw new RpcException(new Status(StatusCode.Aborted, "result is null"));
                                        }
                                    };
                                }
                            }
                        }
                    }

                    if (vUpdateGrid.Rows.Count > 0)
                    {
                        for (int i = 0; i < vUpdateGrid.Rows.Count; i++)
                        {
                            VRow vRow = vUpdateGrid.Rows[i];
                            VQuery vQuery = new VQuery()
                            {
                                Statement = Statement.Update,
                            };

                            int column_index = 0;

                            foreach (VCell cell in vRow.Cells)
                            {
                                vQuery.Conditions.Add(new VCondition() { ColumnName = vUpdateGrid.Columns[column_index].Name, Value = cell });
                                column_index++;
                            }

                            ACommonView aCommonView = ViewManager.GetRegisteredViewByName(view.Key.Name);
                            VRequest request = new VRequest
                            {
                                Query = vQuery,
                                VName = view.Key.Name
                            };

                            vQuery.Conditions.Add(
                                new VCondition()
                                {
                                    ColumnName = "id",
                                    Operators = Operator.Equally,
                                    Value = new VCell() { UUIDValue = vUpdateGrid.Rows[i].Cells[0].UUIDValue }
                                }
                            );

                            VResponse vResponse = aCommonView != null ? await Task.FromResult(aCommonView.UpdateViewRecordAsync(request)).Result : null;

                            if (vResponse == null)
                            {
                                throw new RpcException(new Status(StatusCode.Aborted, "result is null"));
                            }
                        }
                    }

                    if (vInsertGrid.Rows.Count > 0)
                    {
                        for (int i = 0; i < vInsertGrid.Rows.Count; i++)
                        {
                            VRow vRow = vInsertGrid.Rows[i];
                            VQuery vQuery = new VQuery()
                            {
                                Statement = Statement.Insert,
                            };

                            int column_index = 0;

                            foreach (VCell cell in vRow.Cells)
                            {
                                vQuery.Conditions.Add(new VCondition() { ColumnName = vInsertGrid.Columns[column_index].Name, Value = cell });
                                column_index++;
                            }

                            ACommonView aCommonView = ViewManager.GetRegisteredViewByName(view.Key.Name);
                            VRequest request = new VRequest
                            {
                                Query = vQuery,
                                VName = view.Key.Name
                            };

                            VResponse vResponse = aCommonView != null ? await Task.FromResult(aCommonView.AddViewRecordAsync(request)).Result : null;

                            if (vResponse == null)
                            {
                                throw new RpcException(new Status(StatusCode.Aborted, "result is null"));
                            }
                        }
                    }
                }
            }
        }
        private static void UpdateDefaultDataInDb()
        {
            using DBSession dBSession = new DBSession();
            foreach (KeyValuePair<ACommonView, string> view in ViewManager.RegisteredViews)
            {
                if (!(view.Key is IVGridData)) continue;
                VGrid vLocalGrid = (view.Key as IVGridData).VGrid;
                string sql = "select * from " + ACommonView.QuotedStr(view.Key.Name);
                using NpgsqlDataAdapter dataAdapter = dBSession.GetDataAdapter(sql);

                DataSet dataSet = new DataSet();
                try
                {
                    dataAdapter.Fill(dataSet);
                }
                catch (PostgresException e)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.MessageText));
                }
                catch (Exception e)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.Message));
                }


                if (dataSet is null || dataSet.Tables.Count == 0) throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Aborted, "result is null"));
                List<VColumnInfo> uniqueColumns = vLocalGrid.Columns.Where(c => c.Unique).ToList();
                NpgsqlCommand updateCommand = new NpgsqlCommand();
                StringBuilder updateCommandStringBuilder = new StringBuilder();
                updateCommandStringBuilder.Append("update ");
                updateCommandStringBuilder.Append(ACommonView.QuotedStr(view.Key.Name));
                updateCommandStringBuilder.Append(" set ");
                foreach (DataColumn column in dataSet.Tables[0].Columns)
                {
                    updateCommandStringBuilder.Append(column.ColumnName);
                    updateCommandStringBuilder.Append($" = @{column.ColumnName}, ");
                    updateCommand.Parameters.Add($"@{column.ColumnName}", DBHelpers.ConvertValueTypeToNpgsqlDbType(column.DataType)).SourceColumn = column.ColumnName;
                }
                string cmdText = updateCommandStringBuilder.ToString().Trim(',', ' ') + " where id = @oldId";
                updateCommand.Parameters.Add("@oldId", NpgsqlTypes.NpgsqlDbType.Uuid).SourceColumn = "id";
                updateCommand.CommandText = cmdText;
                dataAdapter.UpdateCommand = updateCommand;
                if (uniqueColumns is null || uniqueColumns.Count == 0) throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "unique column not found"));
                for (int i = 0; i < vLocalGrid.Rows.Count; i++)
                {
                    VRow localRow = vLocalGrid.Rows[i];
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int col = 0; col < uniqueColumns.Count; col++)
                    {
                        VColumnInfo columnInfo = uniqueColumns[col];
                        stringBuilder.Append($"{columnInfo.Name}='{DBHelpers.GetCellTypeData(localRow.Cells[vLocalGrid.Columns.IndexOf(columnInfo)])}'");
                        if (col + 1 < uniqueColumns.Count) stringBuilder.Append(" and ");
                    }
                    DataRow[] dataRows = dataSet.Tables[0].Select(stringBuilder.ToString());
                    if (dataRows.Length == 0)
                    {
                        DataRow newRow = dataSet.Tables[0].NewRow();
                        for (int k = 0; k < vLocalGrid.Columns.Count; k++)
                        {
                            VColumnInfo vColumn = vLocalGrid.Columns[k];
                            if (vColumn.Name == "id")
                            {
                                newRow[vColumn.Name] = Guid.NewGuid();
                                continue;
                            }
                            newRow[vColumn.Name] = DBHelpers.GetCellTypeData(localRow.Cells[k]);
                        }
                        dataSet.Tables[0].Rows.Add(newRow);
                    }
                    else if (dataRows.Length == 1)
                    {
                        DataRow dbRow = dataRows[0];
                        for (int k = 0; k < vLocalGrid.Columns.Count; k++)
                        {
                            VColumnInfo vColumn = vLocalGrid.Columns[k];
                            if (vColumn.Name == "id") continue;
                            if (vColumn.Name == "active" && !(bool)dbRow["active"])
                            {
                                dbRow["active"] = true;
                                continue;
                            }
                            if (dbRow[vColumn.Name].ToString() == DBHelpers.GetCellTypeData(localRow.Cells[k]).ToString()) continue;
                            else
                            {
                                dbRow["active"] = false;
                                DataRow newRow = dataSet.Tables[0].NewRow();
                                for (int p = 0; p < vLocalGrid.Columns.Count; p++)
                                {
                                    VColumnInfo col = vLocalGrid.Columns[p];
                                    if (col.Name == "id")
                                    {
                                        newRow[col.Name] = Guid.NewGuid();
                                        continue;
                                    }
                                    newRow[col.Name] = DBHelpers.GetCellTypeData(localRow.Cells[p]);
                                }
                                dataSet.Tables[0].Rows.Add(newRow);
                                break;
                            }

                        }
                    }
                    else throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "unique column has no unique values"));
                }
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dataRow = dataSet.Tables[0].Rows[i];
                    bool rowIsExist = true;
                    for (int j = 0; j < vLocalGrid.Rows.Count; j++)
                    {
                        rowIsExist = true;
                        VRow row = vLocalGrid.Rows[j];
                        for (int k = 0; k < uniqueColumns.Count; k++)
                        {
                            VColumnInfo columnInfo = uniqueColumns[k];
                            int columnIndex = vLocalGrid.Columns.IndexOf(columnInfo);
                            bool isColumnValueEqual = (DBHelpers.GetCellData(row.Cells[columnIndex]).ToString() == dataRow[columnInfo.Name].ToString());
                            rowIsExist = rowIsExist && isColumnValueEqual;
                        }
                        if (rowIsExist) break;
                    }
                    if (rowIsExist) continue;
                    else dataRow["active"] = false;
                }
                NpgsqlCommandBuilder commandBuilder = new NpgsqlCommandBuilder(dataAdapter);
                try
                {
                    dataAdapter.Update(dataSet);
                }
                catch (PostgresException e)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.MessageText));
                }
                catch (Exception e)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.Message));
                }
            }
        }
    }
}