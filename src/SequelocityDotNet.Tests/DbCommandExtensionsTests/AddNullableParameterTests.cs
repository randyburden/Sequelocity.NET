using System;
using System.Data;
using NUnit.Framework;

namespace SequelocityDotNet.Tests.DbCommandExtensionsTests
{
    [TestFixture]
    public class AddNullableParameterTests
    {
        [Test]
        [TestCase( null, DbType.Int16 )]
        [TestCase( null, DbType.AnsiString )]
        [TestCase( "", DbType.AnsiString )]
        public void Should_Set_Parameter_Value_To_DBNull_Given_That_Parameter_Is_Null_Or_Empty(object parameterValue, DbType dbType)
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            // Act
            dbCommand = dbCommand.AddNullableParameter( parameterName, parameterValue, dbType );

            // Assert
            Assert.That( dbCommand.Parameters[parameterName].Value == DBNull.Value );
        }

        [Test]
        [TestCase( 1234, DbType.Int16 )]
        [TestCase( "Green Lantern", DbType.AnsiString )]
        [TestCase( " ", DbType.AnsiString )]
        public void Should_Set_Parameter_To_The_Given_Value_When_Value_Is_Not_Null( object parameterValue, DbType dbType )
        {
            // Arrange
            var dbCommand = TestHelpers.GetDbCommand();

            const string parameterName = "@SuperHeroName";

            // Act
            dbCommand = dbCommand.AddNullableParameter( parameterName, parameterValue, dbType );

            // Assert
            Assert.That( dbCommand.Parameters[parameterName].Value == parameterValue );
        }

        [Test]
        [TestCase( "VARCHAR(100)", DbType.AnsiString, "" )]
        [TestCase( "SMALLINT", DbType.Int16, null )]
        public void Should_Set_Sql_Parameter_To_Null_When_Null_Or_Empty_Is_Assigned(string sqlDataType, DbType dbType, object parameterValue)
        {
            // Arrange
            const string sql = @"
DECLARE 
@SuperHeroName {0} = @pSuperHeroName

SELECT @SuperHeroName as SuperHeroName
";
            string formattedSql = string.Format(sql, sqlDataType);

            // Act
            string result = TestHelpers.GetDatabaseCommand()
                .SetCommandText( formattedSql )
                .AddNullableParameter( "@pSuperHeroName", parameterValue, dbType )
                .ExecuteScalar<string>();

            // Assert
            Assert.That(result == null);
        }
    }
}
