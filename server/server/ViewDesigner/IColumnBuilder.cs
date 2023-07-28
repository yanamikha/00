using View;

namespace GRPCServer.ViewDesigner
{
    internal interface IColumnBuilder
    {
        VColumnInfo ColumnInfo
        {
            get;
        }

        VCell BuildVCell(Npgsql.NpgsqlDataReader reader);

        VCell BuildCellColumnNameOnly(Npgsql.NpgsqlDataReader reader);
    }
}