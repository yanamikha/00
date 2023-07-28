using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class UserGroupPermissionsView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "UserGroupPermissions";
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
                        Name = "usergroup_id",
                        DisplayName = "Назва",
                        ValueType = ValueType.HintValue,
                        HColumns = new HColumn() { VName = "UserGroups" , ColumnName = "name" },
                        IsScan = false,
                        Unique = true,
                    }),

                    new CommonColumnBuilder(new VColumnInfo
                    {
                        Name = "permission_id",
                        DisplayName = "Назва дозволу",
                        ValueType = ValueType.HintValue,
                        HColumns = new HColumn() { VName = "Permissions" , ColumnName = "name"},
                        IsScan = false,
                        Unique = true,
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
            {Statement.Select,"Перегляд дозволів груп користувачів"},
            {Statement.Insert,"Додавання дозволів груп користувачів"},
            {Statement.Update,"Редагування дозволів груп користувачів"},
            {Statement.Delete,"Видалення дозволів груп користувачів"}
        };
        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}