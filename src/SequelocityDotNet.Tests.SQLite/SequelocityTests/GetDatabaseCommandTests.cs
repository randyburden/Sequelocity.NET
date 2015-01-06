using System.Configuration;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.SQLite.SequelocityTests
{
    [TestFixture]
    public class GetDatabaseCommandTests
    {
        [Test]
        public void Can_Get_A_DatabaseCommand_From_A_ConnectionString_Name()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            const string connectionStringName = "SqliteInMemoryDatabase";

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand( connectionStringName );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_From_A_ConnectionString_And_A_DbProviderFactoryInvariantName()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            const string connectionString = "SqliteInMemoryDatabase";

            const string dbProviderFactoryInvariantName = "System.Data.SQLite";

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand( connectionString, dbProviderFactoryInvariantName );

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_By_Setting_A_Default_ConnectionString_Name()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            Sequelocity.ConfigurationSettings.Default.ConnectionStringName = "SqliteInMemoryDatabase";

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand();

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Can_Get_A_DatabaseCommand_By_Setting_A_Default_ConnectionString_And_A_DbProviderFactoryInvariantName()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            Sequelocity.ConfigurationSettings.Default.ConnectionStringName = "SqliteInMemoryDatabase";
            Sequelocity.ConfigurationSettings.Default.DbProviderFactoryInvariantName = "System.Data.SQLite";

            // Act
            var databaseCommand = Sequelocity.GetDatabaseCommand();

            // Assert
            Assert.NotNull( databaseCommand );
            Assert.That( databaseCommand.TestConnection() );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Throws_An_Exception_When_No_ConnectionString_Could_Be_Found()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            // Act
            TestDelegate action = () => Sequelocity.GetDatabaseCommand();

            // Assert
            Assert.Throws<Sequelocity.ConnectionStringNotFoundException>( action );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }

        [Test]
        public void Throws_An_Exception_When_No_DbProviderFactory_Could_Be_Found()
        {
            // Arrange
            TestHelpers.ClearDefaultConfigurationSettings();

            string connectionString = ConfigurationManager.ConnectionStrings["SqliteInMemoryDatabase"].ConnectionString;

            // Act
            TestDelegate action = () => Sequelocity.GetDatabaseCommand( connectionString );

            // Assert
            Assert.Throws<Sequelocity.DbProviderFactoryNotFoundException>( action );

            // Reset
            TestHelpers.ClearDefaultConfigurationSettings();
        }
    }
}