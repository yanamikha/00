using GRPCServer.db;
using System.Collections.Generic;
using View;

namespace GRPCServer.ViewDesigner
{
    internal class CommonColumnBuilder : IColumnBuilder
    {
        public VColumnInfo ColumnInfo
        {
            get;
            private set;
        }

        public CommonColumnBuilder(VColumnInfo columnInfo)
        {
            ColumnInfo = columnInfo;
        }

        public VCell BuildVCell(Npgsql.NpgsqlDataReader reader)
        {
            string column_name = (ColumnInfo.HColumns != null) && (ColumnInfo.HColumns.ColumnName != null) && (ColumnInfo.HColumns.VName != null)
                ? ColumnInfo.HColumns.VName + "_" + ColumnInfo.HColumns.ColumnName
                : ColumnInfo.Name;
            object targetColumn = reader[column_name];

            if (targetColumn == null)
            {
                throw new System.Exception("Unable to build VCell: row contains no target column " + column_name);
            }

            return DBHelpers.BuildVCell(targetColumn);
        }

        public VCell BuildCellColumnNameOnly(Npgsql.NpgsqlDataReader reader)
        {
            object targetColumn = reader[ColumnInfo.Name];

            if (ColumnInfo.Name == null)
            {
                throw new System.Exception("Unable to build VCell: row contains no target column " + ColumnInfo.Name);
            }

            return DBHelpers.BuildVCell(targetColumn);
        }
    }
}