using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class CarTypesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "CarTypes";
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
                Name = "description",
                DisplayName = "Опис",
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
            {Statement.Select,"Перегляд типів транспортних засобів"}
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

                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АД" }, new VCell { StringValue = "автодрабина" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АЦ" }, new VCell { StringValue = "автоцистерна" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АПД" }, new VCell { StringValue = "автомобіль першої допомоги" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АПГ" }, new VCell { StringValue = "автомобіль пінного гасіння" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АП" }, new VCell { StringValue = "автомобіль порошкового гасіння" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АГВГ" }, new VCell { StringValue = "автомобіль газоводяного гасіння" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АКГ" }, new VCell { StringValue = "автомобіль комбінованого гасіння" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АНР" }, new VCell { StringValue = "автомобіль насосно-рукавний" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АА" }, new VCell { StringValue = "аеродромний автомобіль" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АГДЗ" }, new VCell { StringValue = "автомобіль газодимозахисної служби" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АТС" }, new VCell { StringValue = "автомобіль технічної служби" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АШ" }, new VCell { StringValue = "автомобіль штабний" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АР" }, new VCell { StringValue = "автомобіль рукавний" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "ПНС" }, new VCell { StringValue = "пожежна насосна станція" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "АЗО" }, new VCell { StringValue = "автомобіль технічної служби, зв`язку і освітлення" }, new VCell { BoolValue = true } } });

                return vGrid;
            }
        }
    }
}