using System.Data;
using NUnit.Framework;

namespace SqlocityNetCore.Tests.PostgreSQL.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteToObjectTests
    {
        public class SuperHero
        {
            public long SuperHeroId;
            public string SuperHeroName;
        }

        [Test]
        public async void Should_Return_A_Type_Of_T()
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
            var superHero = await Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText(sql)
                .ExecuteToObjectAsync<SuperHero>();

            // Assert
            Assert.NotNull(superHero);
            Assert.That(superHero.SuperHeroId == 1);
            Assert.That(superHero.SuperHeroName == "Superman");
        }

        [Test]
        public async void Should_Null_The_DbCommand_By_Default()
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
            await databaseCommand.ExecuteToObjectAsync<SuperHero>();

            // Assert
            Assert.IsNull(databaseCommand.DbCommand);
        }

        [Test]
        public async void Should_Keep_The_Database_Connection_Open_If_keepConnectionOpen_Parameter_Was_True()
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
            await databaseCommand.ExecuteToObjectAsync<SuperHero>(true);

            // Assert
            Assert.That(databaseCommand.DbCommand.Connection.State == ConnectionState.Open);

            // Cleanup
            databaseCommand.Dispose();
        }

        [Test]
        public async void Should_Call_The_DatabaseCommandPreExecuteEventHandler()
        {
            // Arrange
            bool wasPreExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add(command => wasPreExecuteEventHandlerCalled = true);

            // Act
            await Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName")
                .ExecuteToObjectAsync<SuperHero>();

            // Assert
            Assert.IsTrue(wasPreExecuteEventHandlerCalled);
        }

        [Test]
        public async void Should_Call_The_DatabaseCommandPostExecuteEventHandler()
        {
            // Arrange
            bool wasPostExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add(command => wasPostExecuteEventHandlerCalled = true);

            // Act
            await Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName")
                .ExecuteToObjectAsync<SuperHero>();

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
            TestDelegate action = async () => await Sqlocity.GetDatabaseCommand(ConnectionStringsNames.PostgreSQLConnectionString)
                .SetCommandText("asdf;lkj")
                .ExecuteToObjectAsync<SuperHero>();

            // Assert
            Assert.Throws<global::Npgsql.NpgsqlException>(action);
            Assert.IsTrue(wasUnhandledExceptionEventHandlerCalled);
        }
    }
}