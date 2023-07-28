using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class PlaceTypesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "PlaceTypes";
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
                    Name = "active",
                    DisplayName = "Активність",
                    ValueType = View.ValueType.Uuidvalue,
                    IsScan = false
                }),
            };
        }

        public Dictionary<Statement, string> RequiredPermissions => new Dictionary<Statement, string>
        {
            {Statement.Select,"Перегляд типів місць"}
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

                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "будівля" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "відкрита територія" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "об`єкт з масовим перебуванням людей" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "підприємство" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "сміттєвий бак" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "лісовий масив" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "транспорт повітряний" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "транспорт залізничний" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "транспорт водний річковий" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "транспорт міський громадський" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "транспорт автомобільний" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "трубопровід" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "метрополітен" }, new VCell { BoolValue = true } } });

                return vGrid;
            }
        }
    }
}