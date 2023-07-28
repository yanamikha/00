using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class DistrictsView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "Districts";
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
                Name = "region_id",
                DisplayName = "Область",
                HColumns = new HColumn{VName = "Regions", ColumnName="name"},
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
                Name = "gcode",
                DisplayName = "Код",
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
            {Statement.Select,"Перегляд обласних районів"},
            {Statement.Insert,"Додавання обласних районів"},
            {Statement.Update,"Редагування обласних районів"},
            {Statement.Delete,"Видалення обласних районів"}
        };


        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}