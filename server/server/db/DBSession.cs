using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRPCServer.db
{
    public class DBSession : IDisposable
    {
        protected NpgsqlConnection NpgsqlConnection { get; set; }
        protected NpgsqlDataReader NpgsqlDataReader { get; set; }
        protected NpgsqlTransaction NpgsqlTransaction { get; set; }
        public DBSession()
        {
            NpgsqlConnection = null;
            NpgsqlTransaction = null;
            NpgsqlDataReader = null;
        }

        ~DBSession()
        {
            Dispose();
        }

        /// <summary>
        /// Вернуть соединени в список +
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (NpgsqlDataReader != null)
                {
                    NpgsqlDataReader.Close();
                }
                if (NpgsqlTransaction != null && !NpgsqlTransaction.IsCompleted)
                {
                    NpgsqlTransaction.Commit();
                    NpgsqlTransaction.Dispose();
                }
            }
            finally
            {
                NpgsqlDataReader = null;
                try
                {
                    if ((NpgsqlTransaction != null) && (!NpgsqlTransaction.IsCompleted))
                    {
                        NpgsqlTransaction.Rollback();
                        NpgsqlTransaction.Dispose();
                    }
                }
                finally { NpgsqlTransaction = null; }
            }

            DBConnectionPool.RerturnConn(NpgsqlConnection);
        }
        
        /// <summary>
        /// Получить указатель на активное свободное соединение из списка соединений +
        /// </summary>
        public void GetConn()
        {
            NpgsqlConnection = DBConnectionPool.GetConn();
        }

        internal NpgsqlDataAdapter GetDataAdapter(string sql)
        {
            return new NpgsqlDataAdapter(sql, NpgsqlConnection);
        }

        /// <summary>
        /// Выполнить асинхронно запрос типа NonQuery
        /// </summary>
        public async Task<bool> ExecuteNonQueryAsync(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                NpgsqlTransaction = await NpgsqlConnection.BeginTransactionAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, NpgsqlConnection, NpgsqlTransaction))
                {
                    if (parameters != null && parameters.Count > 0)
                    {
                        foreach (var par in parameters)
                        {
                            cmd.Parameters.Add(new NpgsqlParameter(par.Key, par.Value));
                        }
                    }
                    _ = await cmd.ExecuteNonQueryAsync();
                }
                await NpgsqlTransaction.CommitAsync();
            }
            catch (PostgresException e)
            {
                if (!NpgsqlTransaction.IsCompleted) { await NpgsqlTransaction.RollbackAsync(); }
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.MessageText));
            }
            catch (Exception e)
            {
                if (!NpgsqlTransaction.IsCompleted) { await NpgsqlTransaction.RollbackAsync(); }
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.Message));
            }
            return true;
        }

        /// <summary>
        /// Выполнить асинхронно запрос типа Reader
        /// </summary>
        public async Task<NpgsqlDataReader> ExecuteReaderAsync(string sql, Dictionary<string, object> parameters = null)
        {
            try
            {
                NpgsqlTransaction = await NpgsqlConnection.BeginTransactionAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, NpgsqlConnection, NpgsqlTransaction))
                {
                    if(parameters != null && parameters.Count > 0)
                    {
                        foreach (var par in parameters)
                        {
                            cmd.Parameters.Add(new NpgsqlParameter(par.Key, par.Value));
                        }
                    }
                    NpgsqlDataReader = await cmd.ExecuteReaderAsync();
                }
                return NpgsqlDataReader;
            }
            catch (PostgresException e)
            {
                await NpgsqlTransaction.RollbackAsync();
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.MessageText));
            }
            catch (Exception e)
            {
                if (!NpgsqlTransaction.IsCompleted) { await NpgsqlTransaction.RollbackAsync(); }
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.Message));
            }
        }
    }
}