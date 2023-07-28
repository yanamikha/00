using GRPCServer.db;
using GRPCServer.ViewDesigner;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using View;

namespace GRPCServer.Views
{
    class PlaceSubTypesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "PlaceSubTypes";
        public ColumnBuilderCollection BuilderCollection
        {
            get => new ColumnBuilderCollection
            {
                new CommonColumnBuilder(new VColumnInfo
                {
                    Name = "id",
                    DisplayName = "Ідентифікатор",
                    ValueType = View.ValueType.Uuidvalue,
                    IsScan=false
                }),
                new CommonColumnBuilder(new VColumnInfo
                {
                    Name = "placetype_id",
                    DisplayName = "Тип місця",
                    HColumns = new HColumn{VName = "PlaceTypes", ColumnName="name"},
                    ValueType = View.ValueType.HintValue,
                    IsScan = false
                }),
                new CommonColumnBuilder(new VColumnInfo
                {
                    Name = "name",
                    DisplayName = "Назва",
                    ValueType = View.ValueType.StringValue,
                    IsScan = false,
                    Unique = true
                }),
                new CommonColumnBuilder(new VColumnInfo
                {
                    Name = "active",
                    DisplayName = "Активність",
                    ValueType = View.ValueType.Uuidvalue,
                    IsScan = false
                }),
            };
        }

        public Dictionary<Statement, string> RequiredPermissions => new Dictionary<Statement, string>
        {
            {Statement.Select,"Перегляд підтипів місць"}
        };

        public override ICommonViewDescriptor ViewDescriptor => this;

        public VGrid VGrid
        {
            get
            {
                VGrid vGrid = new VGrid();

                foreach (var column in BuilderCollection)
                {
                    vGrid.Columns.Add(column.ColumnInfo);
                }
                DataSet dataSet;
                using (DBSession dBSession = new DBSession())
                {
                    string sql = "select * from " + QuotedStr("PlaceTypes");
                    using var dataAdapter = dBSession.GetDataAdapter(sql);
                    dataSet = new DataSet();
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
                }
                if (dataSet is null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0) throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Aborted, "result is null"));
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    string id = row["id"]?.ToString();
                    string name = row["name"]?.ToString().ToLower();
                    switch (name)
                    {
                        case "будівля":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "приватний житловий будинок" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "дачний будинок" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "багатоквартирний житловий будинок" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "інша будівля" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        case "лісовий масив":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "площа" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "низова" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "верхова" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        case "транспорт повітряний":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "літак" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "вертоліт" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "гелікоптер" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "безпілотний літальний апарат" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "космічний апарат" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        case "транспорт залізничний":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "пасажирський поїзд" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "товарний поїзд" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        case "транспорт водний річковий":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "круїзне судно" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "пасажирське судно" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "яхта" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "човен" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "парусний човен" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        case "транспорт міський громадський":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "маршрутне таксі" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "автобус" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "тролейбус" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "трамвай" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        case "транспорт автомобільний":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "електромобіль" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "на дизельному паливі" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "на газовому паливі" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "на бензиновому паливі" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        case "метрополітен":
                            {
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "платформа" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "верхній вестибюль" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "машинне відділення" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "кабельний колектор" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "підплатформені приміщення" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "перехід" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "перегін між станціями" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "ескалатор" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "глухий кут відстою поїздів" }, new VCell { BoolValue = true } } });
                                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { UUIDValue = id }, new VCell { StringValue = "інше приміщення" }, new VCell { BoolValue = true } } });
                                break;
                            }
                        default: break;
                    }
                }
                dataSet.Dispose();
                return vGrid;
            }
        }
    }
}