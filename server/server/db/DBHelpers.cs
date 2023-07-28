using System;
using Google.Protobuf;
using View;

namespace GRPCServer.db
{
    internal class DBHelpers
    {
        /// <summary>
        ///  Приведение данных из reader к VCell
        /// </summary>
        public static VCell BuildVCell(object row, VCell.ValueOneofCase typeRestriction = VCell.ValueOneofCase.None)
        {
            if (typeRestriction != VCell.ValueOneofCase.None)
            {
                throw new NotImplementedException("Type restrictions are not implemented yet");
            }

            VCell result = new VCell();
            System.Type valueType = row.GetType();
            if (valueType == typeof(DBNull))
            {
                result.DBNULL = "";
            }
            else if (valueType == typeof(string[]))
            {
                result.ArrayOfString = row.ToString();
            }
            else if (valueType == typeof(Guid))
            {
                result.UUIDValue = row.ToString();
            }
            else if (valueType == typeof(double))
            {
                result.DoubleValue = (double)row;
            }
            else if (valueType == typeof(float))
            {
                result.FloatValue = (float)row;
            }
            else if (valueType == typeof(int))
            {
                result.Int32Value = (int)row;
            }
            else if (valueType == typeof(long))
            {
                result.Int64Value = (long)row;
            }
            else if (valueType == typeof(uint))
            {
                result.Uint32Value = (uint)row;
            }
            else if (valueType == typeof(ulong))
            {
                result.Uint64Value = (ulong)row;
            }
            else if (valueType == typeof(bool))
            {
                result.BoolValue = (bool)row;
            }
            else if (valueType == typeof(string))
            {
                result.StringValue = (string)row;
            }
            else if (valueType == typeof(byte[])) // ??
            {
                result.BytesValue = ByteString.CopyFromUtf8(row.ToString());
            }

            else if (valueType == typeof(short))
            {
                result.Int32Value = (short)row;
            }
            if (result.ValueCase == VCell.ValueOneofCase.None)
            {
                throw new FormatException("Unable to convert database cell to VCell: unknow input value type " + row.GetType().ToString());
            }

            return result;
        }

        public static object GetCellData(VCell vCell)
        {
            try
            {
                switch (vCell.ValueCase.ToString())
                {
                    case "DoubleValue":
                        {
                            return vCell.DoubleValue;
                        };

                    case "FloatValue":
                        {
                            return Convert.ToString(vCell.FloatValue);
                        };

                    case "Int32Value":
                        {
                            return Convert.ToString(vCell.Int32Value);
                        };

                    case "Int64Value":
                        {
                            return Convert.ToString(vCell.Int64Value);
                        };

                    case "Uint32Value":
                        {
                            return Convert.ToString(vCell.Uint32Value);
                        };

                    case "Uint64Value":
                        {
                            return Convert.ToString(vCell.Uint64Value);
                        };

                    case "BoolValue":
                        {
                            return Convert.ToString(vCell.BoolValue);
                        };

                    case "StringValue":
                        {
                            return Convert.ToString(vCell.StringValue);
                        };

                    case "BytesValue":
                        {
                            return Convert.ToString(vCell.BytesValue);
                        };

                    case "UUIDValue":
                        {
                            return Convert.ToString(vCell.UUIDValue);
                        };                    
                }
                return "";
            }
            catch (Exception e) { /*Log.Write(e); */ return ""; }

        }

        public static object GetCellTypeData(VCell vCell)
        {
            try
            {
                switch (vCell.ValueCase.ToString())
                {
                    case "DoubleValue":
                        {
                            return vCell.DoubleValue;
                        };

                    case "FloatValue":
                        {
                            return vCell.FloatValue;
                        };

                    case "Int32Value":
                        {
                            return vCell.Int32Value;
                        };

                    case "Int64Value":
                        {
                            return vCell.Int64Value;
                        };

                    case "Uint32Value":
                        {
                            return vCell.Uint32Value;
                        };

                    case "Uint64Value":
                        {
                            return vCell.Uint64Value;
                        };

                    case "BoolValue":
                        {
                            return vCell.BoolValue;
                        };

                    case "StringValue":
                        {
                            return vCell.StringValue;
                        };

                    case "BytesValue":
                        {
                            return vCell.BytesValue;
                        };

                    case "UUIDValue":
                        {
                            return Guid.Parse(vCell.UUIDValue);
                        };
                }
                return "";
            }
            catch (Exception e) { /*Log.Write(e); */ return ""; }

        }
        public static NpgsqlTypes.NpgsqlDbType ConvertValueTypeToNpgsqlDbType(System.Type type)
        {
            if (type == typeof(bool)) return NpgsqlTypes.NpgsqlDbType.Boolean;
            else if (type == typeof(float)) return NpgsqlTypes.NpgsqlDbType.Real;
            else if (type == typeof(int)) return NpgsqlTypes.NpgsqlDbType.Integer;
            else if (type == typeof(long)) return NpgsqlTypes.NpgsqlDbType.Bigint;
            else if (type == typeof(string)) return NpgsqlTypes.NpgsqlDbType.Text;
            else if (type == typeof(Guid)) return NpgsqlTypes.NpgsqlDbType.Uuid;
            else return NpgsqlTypes.NpgsqlDbType.Unknown;
        }
    }
}