using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class QualificationTypesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "QualificationTypes";
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
                        Unique = true
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "shortname",
                        DisplayName = "Скорочена назва",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
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
            {Statement.Select,"Перегляд типів кваліфікацій"},
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
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Без категорії" }, new VCell { StringValue = "-" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Друга категорія" }, new VCell { StringValue = "II" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Перша категорія" }, new VCell { StringValue = "I" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Вища категорія" }, new VCell { StringValue = "Вища" }, new VCell { BoolValue = true } } });

                return vGrid;
            }
        }
    }
}