namespace SqlocityNetCore.Tests.PostgreSQL
{
    public class TestHelpers
    {
        public static void ClearDefaultConfigurationSettings()
        {
            Sqlocity.ConfigurationSettings.Default.ConnectionString = null;
            Sqlocity.ConfigurationSettings.Default.ConnectionStringName = null;
            Sqlocity.ConfigurationSettings.Default.DbProviderFactoryInvariantName = null;
        }
    }
}
