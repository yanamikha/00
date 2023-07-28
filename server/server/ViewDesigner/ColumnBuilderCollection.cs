using System.Collections.ObjectModel;

namespace GRPCServer.ViewDesigner
{
    internal class ColumnBuilderCollection : KeyedCollection<string, IColumnBuilder>
    {
        protected override string GetKeyForItem(IColumnBuilder item)
        {
            return item.ColumnInfo.Name;
        }
    }
}