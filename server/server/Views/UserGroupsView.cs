using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class UserGroupsView : ACommonView, ICommonViewDescriptor
    {
        public override string Name => "UserGroups";
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
                        Name = "name",
                        DisplayName = "Назва",
                        ValueType = ValueType.StringValue,
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
            {Statement.Select,"Перегляд груп користувачів"},
            {Statement.Insert,"Додавання групи користувачів"},
            {Statement.Update,"Редагування групи користувачів"},
            {Statement.Delete,"Видалення групи користувачів"}
        };
        public override ICommonViewDescriptor ViewDescriptor => this;
    }
}