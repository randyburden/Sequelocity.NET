using System.Data;
using NUnit.Framework;

namespace SqlocityNetCore.Tests.MySql.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteToDynamicObjectTests
    {
        public class SuperHero
        {
            public long SuperHeroId;
            public string SuperHeroName;
        }

        [Test]
        public async void Should_Map_The_Results_Back_To_A_List_Of_Dynamic()
        {
            // Arrange
            const string sql = @"
DROP TEMPORARY TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     INT             NOT NULL    AUTO_INCREMENT,
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
            var superHero = await Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( sql )
                .ExecuteToDynamicObjectAsync();

            // Assert
            Assert.NotNull( superHero );
            Assert.That( superHero.SuperHeroId == 1 );
            Assert.That( superHero.SuperHeroName == "Superman" );
        }

        [Test]
        public async void Should_Null_The_DbCommand_By_Default()
        {
            // Arrange
            const string sql = @"
DROP TEMPORARY TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     INT             NOT NULL    AUTO_INCREMENT,
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
            var databaseCommand = Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( sql );

            // Act
            await databaseCommand.ExecuteToDynamicObjectAsync();

            // Assert
            Assert.IsNull( databaseCommand.DbCommand );
        }

        [Test]
        public async void Should_Keep_The_Database_Connection_Open_If_keepConnectionOpen_Parameter_Was_True()
        {
            // Arrange
            const string sql = @"
DROP TEMPORARY TABLE IF EXISTS SuperHero;

CREATE TEMPORARY TABLE SuperHero
(
    SuperHeroId     INT             NOT NULL    AUTO_INCREMENT,
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
            var databaseCommand = Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( sql );

            // Act
            await databaseCommand.ExecuteToDynamicObjectAsync( true );

            // Assert
            Assert.That( databaseCommand.DbCommand.Connection.State == ConnectionState.Open );

            // Cleanup
            databaseCommand.Dispose();
        }

        [Test]
        public async void Should_Call_The_DatabaseCommandPreExecuteEventHandler()
        {
            // Arrange
            bool wasPreExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add( command => wasPreExecuteEventHandlerCalled = true );

            // Act
            await Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
                .ExecuteToDynamicObjectAsync();

            // Assert
            Assert.IsTrue( wasPreExecuteEventHandlerCalled );
        }

        [Test]
        public async void Should_Call_The_DatabaseCommandPostExecuteEventHandler()
        {
            // Arrange
            bool wasPostExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add( command => wasPostExecuteEventHandlerCalled = true );

            // Act
            await Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( "SELECT 1 as SuperHeroId, 'Superman' as SuperHeroName" )
                .ExecuteToDynamicObjectAsync();

            // Assert
            Assert.IsTrue( wasPostExecuteEventHandlerCalled );
        }

        [Test]
        public void Should_Call_The_DatabaseCommandUnhandledExceptionEventHandler()
        {
            // Arrange
            bool wasUnhandledExceptionEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandUnhandledExceptionEventHandlers.Add( ( exception, command ) =>
            {
                wasUnhandledExceptionEventHandlerCalled = true;
            } );

            // Act
            TestDelegate action = async () => await Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( "asdf;lkj" )
                .ExecuteToDynamicObjectAsync();

            // Assert
            Assert.Throws<global::MySql.Data.MySqlClient.MySqlException>( action );
            Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
        }
    }
}