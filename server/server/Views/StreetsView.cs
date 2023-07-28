using GRPCServer.ViewDesigner;
using System;
using System.Collections.Generic;
using System.Text;
using View;

namespace GRPCServer.Views
{
    class StreetsView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "Streets";
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
                Name = "district_id",
                DisplayName = "Обласний район",
                HColumns = new HColumn{VName = "Districs", ColumnName="name"},
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
                Name = "streettype_id",
                DisplayName = "Тип",
                HColumns = new HColumn{VName = "StreetTypes", ColumnName="name"},
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
                Name = "oldname",
                DisplayName = "Стара назва",
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
            {Statement.Select,"Перегляд вулиць"},
            {Statement.Insert,"Додавання вулиць"},
            {Statement.Update,"Редагування вулиць"},
            {Statement.Delete,"Видалення вулиць"}
        };


        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}