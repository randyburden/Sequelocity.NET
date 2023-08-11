using NUnit.Framework;

namespace SqlocityNetCore.Tests.MySql.DatabaseCommandExtensionsTests
{
    [TestFixture]
    public class SetTransactionTests
    {
        [Test]
        public void Should_Associate_A_Transaction_With_The_DatabaseCommand()
        {
            // Arrange
            var connection = Sqlocity.CreateDbConnection( ConnectionStringsNames.MySqlConnectionString );
            connection.Open();
            var transaction = connection.BeginTransaction();
            var databaseCommand = Sqlocity.GetDatabaseCommand( connection );

            // Act
            databaseCommand.SetTransaction( transaction );

            // Assert
            Assert.That( databaseCommand.DbCommand.Transaction == transaction );

            // Cleanup
            connection.Close();
        }
    }
}