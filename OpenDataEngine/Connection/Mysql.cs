using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace OpenDataEngine.Connection
{
    public class Mysql: Base
    {
        private static MySqlConnectionStringBuilder Init(String host, String user, String pass) => new MySqlConnectionStringBuilder()
        {
            Server = host,
            UserID = user,
            Password = pass,
            Port = 3360,
            UseCompression = true,
            PersistSecurityInfo = false,
            AllowZeroDateTime = false,
            ConvertZeroDateTime = true,
            CharacterSet = "UTF8",
            Pooling = false,
            ConnectionTimeout = 30,
            DefaultCommandTimeout = 120,
            ConnectionReset = false,
            CacheServerProperties = true,
            Keepalive = 30,
            ProcedureCacheSize = 0,
            TreatTinyAsBoolean = true,
            IgnorePrepare = false,
            AllowUserVariables = true,
            SslMode = MySqlSslMode.VerifyFull,
        };

        private readonly MySqlConnectionStringBuilder _connectionStringBuilder;
        private MySqlConnection _connection;

        public Mysql(MySqlConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder;
        }
        public Mysql(String host, String user, String pass): this(Init(host, user, pass)) {}
        public Mysql(String dsn) : this(new MySqlConnectionStringBuilder(dsn)) {}

        private async Task Connect()
        {
            if (_connection != null)
            {
                return;
            }

            _connection = new MySqlConnection(_connectionStringBuilder.ToString());

            await _connection.OpenAsync();
        }

        public override async IAsyncEnumerable<dynamic> Execute(String sql, dynamic arguments)
        {
            DbDataReader reader;

            try
            {
                await Connect();

                await using MySqlCommand command = new MySqlCommand(sql, _connection);

                foreach ((String key, Object value) in ((String, Object)[])arguments)
                {
                    command.Parameters.AddWithValue($"@{key}", value);
                }

                reader = await command.ExecuteReaderAsync();
            }
            catch (MySqlException exception)
            {
                throw new DatabaseException(exception, sql);
            }

            while (await reader.ReadAsync())
            {
                yield return Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, reader.GetValue).ToObject();
            }

            await reader.CloseAsync();
            await reader.DisposeAsync();
        }
    }

    public class DatabaseException : Exception
    {
        private static String message(Int32 number) => number switch
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
            _ => "General error, no further specification"
        };

        public DatabaseException(MySqlException exception, String query = null, [CallerMemberName] String caller = null, [CallerLineNumber] Int32 lineNumber = 0): 
            base($"({caller}:{lineNumber.ToString(CultureInfo.InvariantCulture)}) Database error: '{message(exception.Number)}', with query: '{query}'", exception) {}
    }
}