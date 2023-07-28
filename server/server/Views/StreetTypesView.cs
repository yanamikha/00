using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    class StreetTypesView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "StreetTypes";
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
            {Statement.Select,"Перегляд типів вулиць"}
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
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Вулиця" }, new VCell { StringValue = "вул." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Житловий масив" }, new VCell { StringValue = "ж/м" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Узвіз" }, new VCell { StringValue = "узв." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Тупик" }, new VCell { StringValue = "туп." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Провулок" }, new VCell { StringValue = "пров." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Шосе" }, new VCell { StringValue = "шосе" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Проспект" }, new VCell { StringValue = "просп." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Алея" }, new VCell { StringValue = "алея" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Балка" }, new VCell { StringValue = "балка" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Набережна" }, new VCell { StringValue = "наб." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Площа" }, new VCell { StringValue = "площа" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Бульвар" }, new VCell { StringValue = "бульв." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Селище" }, new VCell { StringValue = "селище" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Готель" }, new VCell { StringValue = "готель" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Станція" }, new VCell { StringValue = "ст" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Військове містечко" }, new VCell { StringValue = "вм" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Проїзд" }, new VCell { StringValue = "проїзд." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Прохід" }, new VCell { StringValue = "прохід" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Роз’їзд" }, new VCell { StringValue = "рзд." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Заїзд" }, new VCell { StringValue = "заїзд" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Турбази" }, new VCell { StringValue = "турбази" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Міст" }, new VCell { StringValue = "міст" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Озеро" }, new VCell { StringValue = "оз." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Острів" }, new VCell { StringValue = "о." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Мікрорайон" }, new VCell { StringValue = "мкрн." }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Сквер" }, new VCell { StringValue = "сквер" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Шлях" }, new VCell { StringValue = "шлях" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Парк" }, new VCell { StringValue = "парк" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Дачі" }, new VCell { StringValue = "дачі" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "В’їзд" }, new VCell { StringValue = "в’їзд" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Дорога" }, new VCell { StringValue = "дорога" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Завулок" }, new VCell { StringValue = "завулок" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Квартал" }, new VCell { StringValue = "кв-л" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Лінія" }, new VCell { StringValue = "лінія" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Майдан" }, new VCell { StringValue = "майдан" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Містечко" }, new VCell { StringValue = "містечко" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Урочище" }, new VCell { StringValue = "урочище" }, new VCell { BoolValue = true } } });
                vGrid.Rows.Add(new VRow { Cells = { new VCell { UUIDValue = "" }, new VCell { StringValue = "Хутір" }, new VCell { StringValue = "хутір" }, new VCell { BoolValue = true } } });

                return vGrid;
            }
        }
    }
}