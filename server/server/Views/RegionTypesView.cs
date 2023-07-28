using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class RegionTypesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "RegionTypes";
        public ColumnBuilderCollection BuilderCollection => new ColumnBuilderCollection
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
                Name = "name",
                DisplayName = "Назва",
                ValueType = View.ValueType.StringValue,
                IsScan = false,
                Unique = true
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "shortname",
                DisplayName = "Скорочена назва",
                ValueType = View.ValueType.StringValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "active",
                DisplayName = "Активність",
                ValueType = View.ValueType.Uuidvalue,
                IsScan = false
            }),
        };

        public Dictionary<Statement, string> RequiredPermissions => new Dictionary<Statement, string>
        {
            {Statement.Select,"Перегляд типів регіону"}
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

                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Не визначено" }, new VCell { StringValue = "-" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Область" }, new VCell { StringValue = "обл." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Край" }, new VCell { StringValue = "край" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Автономна республіка" }, new VCell { StringValue = "АР" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Місто (спеціальний статус)" }, new VCell { StringValue = "м." }, new VCell { BoolValue = true } } });

                return vGrid;
            }
        }
    }
}