using System.Threading.Tasks;
using NUnit.Framework;
using System.Data;

namespace SqlocityNetCore.Tests.PostgreSQL.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteNonQueryAsyncTests
    {
        [Test]
        public void Should_Return_A_Task_Resulting_In_The_Number_Of_Affected_Rows()
        {
            // Arrange
            const string sql = @"
DROP TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     serial not null primary key,
    SuperHeroName	VARCHAR(120)    NOT NULL
);

INSERT INTO SuperHero ( SuperHeroName ) VALUES ( 'Superman' );
";

            // Act
            var rowsAffectedTask = Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText(sql)
                .ExecuteNonQueryAsync();

            // Assert
            Assert.IsInstanceOf<Task<int>>(rowsAffectedTask);
            Assert.That(rowsAffectedTask.Result == 1);
        }

        [Test]
        public void Should_Null_The_DbCommand_By_Default()
        {
            // Arrange
            const string sql = @"
DROP TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     serial not null primary key,
    SuperHeroName	VARCHAR(120)    NOT NULL
);

INSERT INTO SuperHero ( SuperHeroName ) VALUES ( 'Superman' );
";
            var databaseCommand = Sqlocity.GetDatabaseCommandForMySql(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText(sql);

            // Act
            databaseCommand.ExecuteNonQueryAsync()
                .Wait(); // Block until the task completes.

            // Assert
            Assert.IsNull(databaseCommand.DbCommand);
        }

        [Test]
        public void Should_Keep_The_Database_Connection_Open_If_keepConnectionOpen_Parameter_Was_True()
        {
            // Arrange
            const string sql = @"
DROP TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     serial not null primary key,
    SuperHeroName	VARCHAR(120)    NOT NULL
);

INSERT INTO SuperHero ( SuperHeroName ) VALUES ( 'Superman' );
";
            var databaseCommand = Sqlocity.GetDatabaseCommandForMySql(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText(sql);

            // Act
            databaseCommand.ExecuteNonQueryAsync(true)
                .Wait(); // Block until the task completes.

            // Assert
            Assert.That(databaseCommand.DbCommand.Connection.State == ConnectionState.Open);

            // Cleanup
            databaseCommand.Dispose();
        }

        [Test]
        public void Should_Call_The_DatabaseCommandPreExecuteEventHandler()
        {
            // Arrange
            bool wasPreExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add(command => wasPreExecuteEventHandlerCalled = true);

            // Act
            Sqlocity.GetDatabaseCommandForMySql(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("SELECT 1")
                .ExecuteNonQueryAsync()
                .Wait(); // Block until the task completes.

            // Assert
            Assert.IsTrue(wasPreExecuteEventHandlerCalled);
        }

        [Test]
        public void Should_Call_The_DatabaseCommandPostExecuteEventHandler()
        {
            // Arrange
            bool wasPostExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add(command => wasPostExecuteEventHandlerCalled = true);

            // Act
            Sqlocity.GetDatabaseCommandForMySql(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("SELECT 1")
                .ExecuteNonQueryAsync()
                .Wait(); // Block until the task completes.

            // Assert
            Assert.IsTrue(wasPostExecuteEventHandlerCalled);
        }

        [Test]
        public void Should_Call_The_DatabaseCommandUnhandledExceptionEventHandler()
        {
            // Arrange
            bool wasUnhandledExceptionEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandUnhandledExceptionEventHandlers.Add((exception, command) =>
            {
                wasUnhandledExceptionEventHandlerCalled = true;
            });

            // Act
            TestDelegate action = async () => await Sqlocity.GetDatabaseCommandForMySql(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("asdf;lkj")
                .ExecuteNonQueryAsync();

            // Assert
            Assert.Throws<global::Npgsql.NpgsqlException>(action);
            Assert.IsTrue(wasUnhandledExceptionEventHandlerCalled);
        }
    }
}
