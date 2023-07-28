using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class AreasView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "Areas";
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
            {Statement.Select,"Перегляд районів населених пунктів"},
            {Statement.Insert,"Додавання районів населених пунктів"},
            {Statement.Update,"Редагування районів населених пунктів"},
            {Statement.Delete,"Видалення районів населених пунктів"}
        };

        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}