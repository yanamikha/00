using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using System.Text;
using View;

namespace GRPCServer.Views
{
    class UsersView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "Users";
        
        public ColumnBuilderCollection BuilderCollection
        {
            get
            {
                ColumnBuilderCollection collection = new ColumnBuilderCollection
                {
                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "id",
                        DisplayName = "Ідентифікатор",
                        ValueType = ValueType.Uuidvalue,
                        IsScan = false,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "login",
                        DisplayName = "Логін",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "password",
                        DisplayName = "Пароль",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
                    }), 

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "usergroup_id",
                        DisplayName = "Група",
                        ValueType = ValueType.HintValue,
                        HColumns = new HColumn{ VName = "UserGroups", ColumnName = "name" },
                        IsScan = false
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "first_name",
                        DisplayName = "Ім'я",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "last_name",
                        DisplayName = "Прізвище",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "middle_name",
                        DisplayName = "По-батькові",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
                    }),
                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "short_fio",
                        DisplayName = "Прізвище та ініціали",
                        ValueType = ValueType.StringValue,
                        IsScan = false,
                    }),
                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "position_id",
                        DisplayName = "Посада",
                        ValueType = ValueType.HintValue,
                        IsScan = false,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "active",
                        DisplayName = "Активність",
                        ValueType = ValueType.BoolValue,
                        IsScan = false,
                    })
                };

                return collection;
            }
        }

        public Dictionary<Statement, string> RequiredPermissions => new Dictionary<Statement, string>
        {
            {Statement.Select,"Перегляд користувачів"},
            {Statement.Insert,"Додавання користувачів"},
            {Statement.Update,"Редагування користувачів"},
            {Statement.Delete,"Видалення користувачів"}
        };
        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}