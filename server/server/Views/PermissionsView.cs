using GRPCServer.db;
using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using System.Threading.Tasks;
using View;

namespace GRPCServer.Views
{
    internal class PermissionsView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "Permissions";
        public ColumnBuilderCollection BuilderCollection
        {
            get
            {
                ColumnBuilderCollection collection = new ColumnBuilderCollection
                {
                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "id",
                        DisplayName = "Ідентифікатор",
                        ValueType = ValueType.Uuidvalue,
                        IsScan = false,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "permissiongroup_id",
                        DisplayName = "Назва групи дозволів",
                        ValueType = ValueType.HintValue,
                        HColumns = new HColumn() { VName = "PermissionGroups" , ColumnName = "name"},
                        IsScan = false,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "name",
                        DisplayName = "Назва",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
                        Unique = true,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "active",
                        DisplayName = "Активність",
                        ValueType = ValueType.BoolValue,
                        IsScan = false,
                    })
                };

                return collection;
            }
        }
        public Dictionary<Statement, string> RequiredPermissions => new Dictionary<Statement, string>
        {
            {Statement.Select,"Перегляд дозволів"},
        };
        public override ICommonViewDescriptor ViewDescriptor => this;

        public VGrid VGrid
        {
            get
            {
                VGrid vGRid = new VGrid();

                foreach (IColumnBuilder c in ViewDescriptor.BuilderCollection)
                {
                    vGRid.Columns.Add(c.ColumnInfo);
                };


                foreach (RegisteredView name in ViewManager.GetRegisteredViewNames)
                {
                    string sql = "select " + QuotedStr("id") + ", " + QuotedStr("active") + " from " + QuotedStr("PermissionGroups") + " where " + QuotedStr("name") + " = " + "'" + name.VName + "'";

                    using (DBSession dBSession = new DBSession())
                    {
                        dBSession.GetConn();

                        using (Npgsql.NpgsqlDataReader reader = Task.Run(async () => await dBSession.ExecuteReaderAsync(sql, null)).Result)
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    foreach (KeyValuePair<Statement, string> requiredPermissions in ViewManager.GetRegisteredViewByName(name.VName).ViewDescriptor.RequiredPermissions)
                                    {
                                        vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { UUIDValue = reader["id"].ToString() }, new VCell() { StringValue = requiredPermissions.Value }, new VCell() { BoolValue = true } } });
                                    }
                                }
                            }
                            else
                            {
                                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Aborted, "result is null"));
                            }
                        };
                    }
                }

                return vGRid;
            }
        }
    }
}