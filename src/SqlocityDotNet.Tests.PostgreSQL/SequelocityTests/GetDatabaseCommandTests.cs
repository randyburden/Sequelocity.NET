using System.Configuration;
using NUnit.Framework;

namespace SqlocityNetCore.Tests.PostgreSQL.SequelocityTests
{
    [TestFixture]
    public class GetDatabaseCommandTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_From_A_ConnectionString_Name()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionStringName = ConnectionStringsNames.PostgreSQLConnectionString;

            // Act
            var databaseCommand = Sqlocity.GetDatabaseCommand(connectionStringName);

            // Assert
            Assert.NotNull(databaseCommand);
            Assert.That(databaseCommand.TestConnection());

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_From_A_ConnectionString_And_A_DbProviderFactoryInvariantName()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.PostgreSQLConnectionString].ConnectionString;

            const string dbProviderFactoryInvariantName = "Npgsql";

            // Act
            var databaseCommand = Sqlocity.GetDatabaseCommand(connectionString, dbProviderFactoryInvariantName);

            // Assert
            Assert.NotNull(databaseCommand);
            Assert.That(databaseCommand.TestConnection());

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_By_Setting_A_Default_ConnectionString_Name()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            Sqlocity.ConfigurationSettings.Default.ConnectionStringName = ConnectionStringsNames.PostgreSQLConnectionString;

            // Act
            var databaseCommand = Sqlocity.GetDatabaseCommand();

            // Assert
            Assert.NotNull(databaseCommand);
            Assert.That(databaseCommand.TestConnection());

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_By_Setting_A_Default_ConnectionString_And_A_DbProviderFactoryInvariantName()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            Sqlocity.ConfigurationSettings.Default.ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.PostgreSQLConnectionString].ConnectionString;
            Sqlocity.ConfigurationSettings.Default.DbProviderFactoryInvariantName = "Npgsql";

            // Act
            var databaseCommand = Sqlocity.GetDatabaseCommand();

            // Assert
            Assert.NotNull(databaseCommand);
            Assert.That(databaseCommand.TestConnection());

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Throws_An_Exception_When_No_ConnectionString_Could_Be_Found()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            // Act
            TestDelegate action = () => Sqlocity.GetDatabaseCommand();

            // Assert
            Assert.Throws<Sqlocity.ConnectionStringNotFoundException>(action);

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Throws_An_Exception_When_No_DbProviderFactory_Could_Be_Found()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringsNames.PostgreSQLConnectionString].ConnectionString;

            // Act
            TestDelegate action = () => Sqlocity.GetDatabaseCommand(connectionString);

            // Assert
            Assert.Throws<Sqlocity.DbProviderFactoryNotFoundException>(action);

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}