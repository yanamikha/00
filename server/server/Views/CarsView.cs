using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class CarsView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "Cars";
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
                Name = "cartype_id",
                DisplayName = "Тип транпортного засобу",
                HColumns = new HColumn{VName = "CarTypes", ColumnName="name"},
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
                Name = "regnumber",
                DisplayName = "Державний номер",
                ValueType = View.ValueType.StringValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "brand",
                DisplayName = "Марка",
                ValueType = View.ValueType.StringValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "leader",
                DisplayName = "Старший ТЗ",
                HColumns = new HColumn{VName = "Users", ColumnName="short_fio"},
                ValueType = View.ValueType.HintValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "carstatus_id",
                DisplayName = "Поточний стан ТЗ",
                HColumns = new HColumn{VName = "CarStatuses", ColumnName="name"},
                ValueType = View.ValueType.HintValue,
                IsScan = false
            }),
            new CommonColumnBuilder(new VColumnInfo
            {
                Name = "dep_id",
                DisplayName = "Підрозділ приписки",
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
            {Statement.Select,"Перегляд транспортних засобів"},
            {Statement.Insert,"Додавання транспортних засобів"},
            {Statement.Update,"Редагування транспортних засобів"},
            {Statement.Delete,"Видалення транспортних засобів"}
        };


        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}