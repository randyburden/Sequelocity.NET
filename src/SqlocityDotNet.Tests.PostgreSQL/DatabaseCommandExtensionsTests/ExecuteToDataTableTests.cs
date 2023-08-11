using System.Data;
using NUnit.Framework;

namespace SqlocityNetCore.Tests.PostgreSQL.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteToDataTableTests
    {
        [Test]
        public void Should_Return_A_DataSet()
        {
            // Arrange
            const string sql = @"
DROP TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     serial not null,
    SuperHeroName	VARCHAR(120)    NOT NULL,
    PRIMARY KEY ( SuperHeroId )
);

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    SuperHero;
";

            // Act
            var dataTable = Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText(sql)
                .ExecuteToDataTable();

            // Assert
            Assert.That(dataTable.Rows.Count == 2);
            Assert.That(dataTable.Rows[0][0].ToString() == "1");
            Assert.That(dataTable.Rows[0][1].ToString() == "Superman");
            Assert.That(dataTable.Rows[1][0].ToString() == "2");
            Assert.That(dataTable.Rows[1][1].ToString() == "Batman");
        }

        [Test]
        public void Should_Null_The_DbCommand_By_Default()
        {
            // Arrange
            const string sql = @"
DROP TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     serial not null,
    SuperHeroName	VARCHAR(120)    NOT NULL,
    PRIMARY KEY ( SuperHeroId )
);

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText(sql);

            // Act
            databaseCommand.ExecuteToDataTable();

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
    SuperHeroId     serial not null,
    SuperHeroName	VARCHAR(120)    NOT NULL,
    PRIMARY KEY ( SuperHeroId )
);

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Superman' );

INSERT INTO SuperHero ( SuperHeroName )
VALUES ( 'Batman' );

SELECT  SuperHeroId,
        SuperHeroName
FROM    SuperHero;
";
            var databaseCommand = Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText(sql);

            // Act
            databaseCommand.ExecuteToDataTable(true);

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
            Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("SELECT 1")
                .ExecuteToDataTable();

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
            Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("SELECT 1")
                .ExecuteToDataTable();

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
            TestDelegate action = () => Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("asdf;lkj")
                .ExecuteToDataTable();

            // Assert
            Assert.Throws<global::Npgsql.NpgsqlException>(action);
            Assert.IsTrue(wasUnhandledExceptionEventHandlerCalled);
        }
    }
}