using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class PositionsView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "Positions";
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
            {Statement.Select,"Перегляд посад"},
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

                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Адміністратор" }, new VCell() { BoolValue = true } } });
                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Водій" }, new VCell() { BoolValue = true } } });
                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Стажер" }, new VCell() { BoolValue = true } } });
                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Парамедик" }, new VCell() { BoolValue = true } } });
                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Фахівець" }, new VCell() { BoolValue = true } } });
                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Робітник" }, new VCell() { BoolValue = true } } });
                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Диспетчер" }, new VCell() { BoolValue = true } } });
                vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = "Оператор" }, new VCell() { BoolValue = true } } });

                return vGRid;
            }
        }
    }
}