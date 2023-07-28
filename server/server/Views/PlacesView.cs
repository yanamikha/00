using GRPCServer.ViewDesigner;
using System;
using System.Collections.Generic;
using System.Text;
using View;

namespace GRPCServer.Views
{
    class PlacesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "Places";
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
            {Statement.Select,"Перегляд місць"}
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

                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "квартира" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "номер" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "кабінет" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "сходова клітина" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "ліфт" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "підвал" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "технічний поверх" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "сміттєзбірник" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "вагон" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "станція" }, new VCell { BoolValue = true } } });

                return vGrid;
            }
        }
    }
}