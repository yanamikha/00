using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class CarStatusHistoriesView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "CarStatusHistories";
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
                Name = "changedate",
                DisplayName = "Час",
                ValueType = View.ValueType.DateTimeValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "car_id",
                DisplayName = "Транспортний засіб",
                HColumns = new HColumn{VName = "Cars", ColumnName="name"},
                ValueType = View.ValueType.HintValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "carstatus_id",
                DisplayName = "Стан транспортного засобу",
                HColumns = new HColumn{VName = "CarStatuses", ColumnName="name"},
                ValueType = View.ValueType.HintValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "dep_id",
                DisplayName = "Підрозділ приписки ТЗ",
                HColumns = new HColumn{VName = "Deps", ColumnName="name"},
                ValueType = View.ValueType.HintValue,
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
            {Statement.Select,"Перегляд історії зміни стану ТЗ"},
            {Statement.Insert,"Додавання історії зміни стану ТЗ"},
            {Statement.Update,"Редагування історії зміни стану ТЗ"},
            {Statement.Delete,"Видалення історії зміни стану ТЗ"}
        };

        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}