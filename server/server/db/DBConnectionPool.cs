using System;
using Npgsql;
using System.Collections.Generic;

namespace GRPCServer.db
{
    public class DBConnectionPool
    {
        ~DBConnectionPool()
        {
            if(Connections.Count > 0)
            {
                foreach (NpgsqlConnection tmp in Connections)
                {
                    tmp.Close();
                }
            }
        }

        public static string ConnectionString = "Host=127.0.0.1; Port=5432; Database=TEST; Username=test; Password=test;";
       
        /// <summary>
        ///  Список соединений +
        /// </summary>
        protected static List<NpgsqlConnection> Connections { get; set; } = new List<NpgsqlConnection>();
       
        protected static int MaxConnections = 20;
        protected static int MinConnections = 3;
        protected static int CurrConnectionsCount = 0;

        /// <summary>
        /// Подключение к СУБД +
        /// </summary>
        protected static NpgsqlConnection Connect(string ConnectionString)
        {
            try
            {
                NpgsqlConnection Connection = new NpgsqlConnection(ConnectionString);
                Connection.Open();
                return Connection;
            }
            catch (PostgresException e)
            {
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.MessageText));
            }
            catch (Exception e)
            {
                //Log.Write(e); 
                throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, e.Message));
            }
        }

        /// <summary>
        /// Вернуть соединени в список +
        /// </summary>
        public static void RerturnConn(NpgsqlConnection npgsqlConnection)
        {
            if (npgsqlConnection == null)
            {
                return;
            }
            else
            {
                lock (Connections)
                {
                    if (CurrConnectionsCount < MinConnections)
                    {
                        Connections.Add(npgsqlConnection);
                    }
                    else
                    {
                        npgsqlConnection.Close();
                        CurrConnectionsCount--;
                    }
                }
            }
        }

        /// <summary>
        /// Получить указатель на активное свободное соединение из списка соединений +
        /// </summary>
        public static NpgsqlConnection GetConn()
        {
            lock (Connections)
            {
                if(CurrConnectionsCount == MaxConnections)
                {
                    throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Cancelled, "connection limit reached. try again."));
                }

                for (int i = 0; i < Connections.Count; i++)
                {
                    NpgsqlConnection tmp = Connections[i];

                    if (tmp.FullState == System.Data.ConnectionState.Open)
                    {
                        NpgsqlConnection connection = tmp;
                        _ = Connections.Remove(tmp);
                        return connection;
                    }
                    else if (tmp.FullState == System.Data.ConnectionState.Closed)
                    {
                        CurrConnectionsCount--;
                        _ = Connections.Remove(tmp);
                    }
                }
                
                NpgsqlConnection npgsqlConnection = Connect(ConnectionString);
                CurrConnectionsCount++;
                return npgsqlConnection;
            }
        }
    }
}