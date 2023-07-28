using GRPCServer.ViewDesigner;
using System.Collections.Generic;
using View;

namespace GRPCServer.Views
{
    internal class PermissionGroupsView : ACommonView, ICommonViewDescriptor, IVGridData
    {
        public override string Name => "PermissionGroups";
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
            {Statement.Select,"Перегляд груп дозволів"},
            {Statement.Insert,"Додавання груп дозволів"},
            {Statement.Update,"Редагування груп дозволів"},
            {Statement.Delete,"Видалення груп дозволів"}
        };
        public override ICommonViewDescriptor ViewDescriptor => this;

        public VGrid VGrid
        {
            get
            {
                VGrid vGRid = new VGrid();

                foreach (IColumnBuilder c in BuilderCollection)
                {
                    vGRid.Columns.Add(c.ColumnInfo);
                };

                foreach (RegisteredView name in ViewManager.GetRegisteredViewNames)
                {
                    vGRid.Rows.Add(new VRow() { Cells = { new VCell() { UUIDValue = "" }, new VCell() { StringValue = name.VName }, new VCell() { BoolValue = true } } });
                }

                return vGRid;
            }
        }
    }
}