using NUnit.Framework;

namespace SequelocityDotNet.Tests.SQLite.SequelocityTests
{
    [TestFixture]
    public class GetDatabaseCommandForSqLiteTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_For_A_SqlServer()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            const string connectionString = "SqliteInMemoryDatabase";

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommandForSQLite( connectionString );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.DbCommand.Connection.ToString() == "System.Data.SQLite.SQLiteConnection" );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}