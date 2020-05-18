using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace OpenDataEngine.Connection
{
    public class Mysql: Base
    {
        private const UInt32 ConnectionTimeout = 30;
        private const UInt32 CommandTimeout = 120;
        private const UInt32 KeepAlive = 30;

        private static MySqlConnectionStringBuilder Init(String host, String user, String pass) => new MySqlConnectionStringBuilder
        {
            Server = host,
            UserID = user,
            Password = pass,
            SslMode = MySqlSslMode.VerifyFull,
            Port = 3306,
            UseCompression = true,
            PersistSecurityInfo = false,
            AllowZeroDateTime = false,
            ConvertZeroDateTime = true,
            CharacterSet = "utf8",
            Pooling = false,
            ConnectionTimeout = ConnectionTimeout,
            DefaultCommandTimeout = CommandTimeout,
            ConnectionReset = false,
            CacheServerProperties = true,
            Keepalive = KeepAlive,
            ProcedureCacheSize = 0,
            TreatTinyAsBoolean = true,
            IgnorePrepare = false,
            AllowUserVariables = true,
        };

        private readonly MySqlConnectionStringBuilder _connectionStringBuilder;
        private MySqlConnection _connection;

        public Mysql(MySqlConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder;
        }
        public Mysql(String host, String user, String pass): this(Init(host, user, pass)) {}
        public Mysql(String dsn) : this(new MySqlConnectionStringBuilder(dsn)) {}

        private Boolean IsConnected => _connection != null && _connection.State == ConnectionState.Open;
        private async Task Connect()
        {
            if (_connection != null)
            {
                return;
            }

            try
            {
                _connection = new MySqlConnection(_connectionStringBuilder.ToString());

                await _connection.OpenAsync();
            }
            catch (MySqlException exception)
            {
                throw new DatabaseException(exception);
            }
        }

        private async Task<DbDataReader> ExecuteQuery(String sql, (String, Object)[] arguments)
        {
            if (IsConnected == false)
            {
                throw new Exception("Connection not available for executing query");
            }

            try
            {
                await using MySqlCommand command = new MySqlCommand(sql, _connection);

                foreach ((String key, Object value) in arguments)
                {
                    command.Parameters.AddWithValue($"@{key}", value);
                }

                return await command.ExecuteReaderAsync();
            }
            catch (MySqlException exception)
            {
                throw new DatabaseException(exception, sql);
            }
        }

        public override async IAsyncEnumerable<IDictionary<String, dynamic>> Execute(String sql, (String, Object)[] arguments, CancellationToken token)
        {
            await Connect();
            await using DbDataReader reader = await ExecuteQuery(sql, arguments);

            while (await reader.ReadAsync())
            {
                yield return Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue);
            }
        }
    }

    public class DatabaseException : Exception
    {
        private static String? message(Int32 number) => number switch
        {
            0 => "Can't get hostname for your address",
            1042 => "Can't create IP socket. This could be caused by closing and opening connections to fast",
            1081 => "No connection could be astablished to the database",
            1044 => "Access denied",
            1045 => "Invalid user credentials",
            1053 => "Server errror, server is shutting down",
            1079 => "Server errror, server is shutting down",
            1077 => "Server errror, server is shutting down",
            1064 => "Error encountered in SQL syntax",
            1080 => "Forcing thread close",
            _ => null,
        };

        public DatabaseException(MySqlException exception, String query = null, [CallerMemberName] String caller = null, [CallerLineNumber] Int32 lineNumber = 0): 
            base($"({caller}:{lineNumber.ToString(CultureInfo.InvariantCulture)}) Database error: '{message(exception.Number) ?? exception.Message}', with query: '{query}'", exception) {}
    }
}