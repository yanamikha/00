using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class CityTypesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "CityTypes";
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
        }

        public Dictionary<Statement, string> RequiredPermissions => new Dictionary<Statement, string>
        {
            {Statement.Select,"Перегляд типів населеного пункту"}
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
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Місто" }, new VCell { StringValue = "м." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Селище" }, new VCell { StringValue = "сщ." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "СМТ" }, new VCell { StringValue = "смт" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Хутір" }, new VCell { StringValue = "х." }, new VCell { BoolValue = true } } });

                return vGrid;
            }
        }
    }
}