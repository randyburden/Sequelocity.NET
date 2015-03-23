using System;
using System.Configuration;
using System.Reflection;

namespace SequelocityDotNet.Tests.MySql
{
    /// <summary>
    /// Connection String Names.
    /// </summary>
    public class ConnectionStringsNames
    {
        /// <summary>
        /// MySQL connection string name.
        /// </summary>
        public static string MySqlConnectionString = "MySqlConnectionString";

        static ConnectionStringsNames()
        {
            AddConnectionStringFromEnvironmentVariableAtRuntime( MySqlConnectionString, "MySql.Data.MySqlClient" );
        }

        /// <summary>
        /// Adds a connection string from an environment variable at runtime.
        /// </summary>
        /// <remarks>
        /// Note that this method is re-run safe and will return early if a connection string with the given name already exists.
        /// </remarks>
        /// <param name="connectionStringAndEnvironmentVariableName">Name of the connection string and environment variable.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <exception cref="System.Exception">Exception thrown when both the connection string and environment variable do not exist.</exception>
        public static void AddConnectionStringFromEnvironmentVariableAtRuntime( string connectionStringAndEnvironmentVariableName, string providerName )
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings[ connectionStringAndEnvironmentVariableName ];

            if ( connectionStringSettings == null )
            {
                var environmentVariable = Environment.GetEnvironmentVariable( connectionStringAndEnvironmentVariableName, EnvironmentVariableTarget.Process )
                                          ?? Environment.GetEnvironmentVariable( connectionStringAndEnvironmentVariableName, EnvironmentVariableTarget.User )
                                          ?? Environment.GetEnvironmentVariable( connectionStringAndEnvironmentVariableName, EnvironmentVariableTarget.Machine );

                if ( environmentVariable != null )
                {
                    // Enables adding a ConnectionString at runtime
                    typeof ( ConfigurationElementCollection )
                        .GetField( "bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic )
                        .SetValue( ConfigurationManager.ConnectionStrings, false );

                    connectionStringSettings = new ConnectionStringSettings( connectionStringAndEnvironmentVariableName, environmentVariable, providerName );

                    ConfigurationManager.ConnectionStrings.Add( connectionStringSettings );

                    return;
                }

                var message = string.Format( "A ConnectionString named '{0}' was not found, please add one or add an environment variable named '{0}' with the connection string as the value.", connectionStringAndEnvironmentVariableName );

                throw new Exception( message );
            }
        }
    }
}