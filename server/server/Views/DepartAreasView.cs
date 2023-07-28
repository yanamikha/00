using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class DepartAreasView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "DepartAreas";
        public ColumnBuilderCollection BuilderCollection => new ColumnBuilderCollection
        {
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "id",
                DisplayName = "Ідентифікатор",
                ValueType = View.ValueType.Uuidvalue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "city_id",
                DisplayName = "Населений пункт",
                HColumns = new HColumn{VName = "Cities", ColumnName="name"},
                ValueType = View.ValueType.HintValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "area_id",
                DisplayName = "Район населеного пункту",
                HColumns = new HColumn{VName = "Areas", ColumnName="name"},
                ValueType = View.ValueType.HintValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "name",
                DisplayName = "Назва",
                ValueType = View.ValueType.StringValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "active",
                DisplayName = "Активність",
                ValueType = View.ValueType.BoolValue,
                IsScan = false
            })
        };

        public Dictionary<Statement, string> RequiredPermissions => new Dictionary<Statement, string>
        {
            {Statement.Select,"Перегляд районів виїзду"},
            {Statement.Insert,"Додавання районів виїзду"},
            {Statement.Update,"Редагування районів виїзду"},
            {Statement.Delete,"Видалення районів виїзду"}
        };

        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}