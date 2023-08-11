using System.Data;
using NUnit.Framework;

namespace SqlocityNetCore.Tests.MySql.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class ExecuteToDataSetTests
    {
        [Test]
        public void Should_Return_A_DataSet()
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
            var dataSet = Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( sql )
                .ExecuteToDataSet();

            // Assert
            Assert.That( dataSet.Tables[0].Rows.Count == 2 );
            Assert.That( dataSet.Tables[0].Rows[0][0].ToString() == "1" );
            Assert.That( dataSet.Tables[0].Rows[0][1].ToString() == "Superman" );
            Assert.That( dataSet.Tables[0].Rows[1][0].ToString() == "2" );
            Assert.That( dataSet.Tables[0].Rows[1][1].ToString() == "Batman" );
        }
        
        [Test]
        public void Should_Null_The_DbCommand_By_Default()
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
            databaseCommand.ExecuteToDataSet();

            // Assert
            Assert.IsNull( databaseCommand.DbCommand );
        }

        [Test]
        public void Should_Keep_The_Database_Connection_Open_If_keepConnectionOpen_Parameter_Was_True()
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
            databaseCommand.ExecuteToDataSet( true );

            // Assert
            Assert.That( databaseCommand.DbCommand.Connection.State == ConnectionState.Open );

            // Cleanup
            databaseCommand.Dispose();
        }

        [Test]
        public void Should_Call_The_DatabaseCommandPreExecuteEventHandler()
        {
            // Arrange
            bool wasPreExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPreExecuteEventHandlers.Add( command => wasPreExecuteEventHandlerCalled = true );

            // Act
            Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( "SELECT 1" )
                .ExecuteToDataSet();

            // Assert
            Assert.IsTrue( wasPreExecuteEventHandlerCalled );
        }

        [Test]
        public void Should_Call_The_DatabaseCommandPostExecuteEventHandler()
        {
            // Arrange
            bool wasPostExecuteEventHandlerCalled = false;

            Sqlocity.ConfigurationSettings.EventHandlers.DatabaseCommandPostExecuteEventHandlers.Add( command => wasPostExecuteEventHandlerCalled = true );

            // Act
            Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( "SELECT 1" )
                .ExecuteToDataSet();

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
            TestDelegate action = () => Sqlocity.GetDatabaseCommand( ConnectionStringsNames.MySqlConnectionString )
                .SetCommandText( "asdf;lkj" )
                .ExecuteToDataSet();

            // Assert
            Assert.Throws<global::MySql.Data.MySqlClient.MySqlException>( action );
            Assert.IsTrue( wasUnhandledExceptionEventHandlerCalled );
        }
    }
}