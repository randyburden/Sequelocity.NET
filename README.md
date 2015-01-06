Sequelocity.NET
===============

*Note that this project is currently in a pre-release state.*

###What is it?###

Sequelocity.NET is a simple data access library for the Microsoft .NET Framework providing lightweight ADO.NET wrapper, object mapper, and helper functions.

It enables you to write fluent style syntax creating a more elegant and concise way of writing data access code. 

It can be used as a C# single file drop in or referenced as a DLL and found on NuGet ( NuGet release coming soon ).

###Documentation?###

Yes, we have documentation. =)

**XML Comments**

All of the methods in the codebase our heavily XML commented which should give you plenty of intellisense description feel-goodness.

**Wiki**

We have a growing number of wiki articles that can be found here: https://github.com/AmbitEnergyLabs/Sequelocity.NET/wiki

**MSDN-Style Documentation**

We will also soon have MSDN-style documentation to explore.

###Why Use It?###

Here is the super awesome list of reasons to use Sequelocity:
- Simplifies your code by eliminating repetitive boilerplate code
- Offers a very readable and concise fluent ( method chaining ) syntax
- Automatically handles all database connection opening, closing, and disposing of unneeded resources
- Can map results back to strongly typed objects, dynamic objects, DataSets or DataTables
- Can generate SQL insert statements given a strongly typed object, anonymous object, or dynamic object
- Is backed by over 300 tests

###Usage###

One of the design goals of Sequelocity is provide a very simple fluent ( method chaining ) programming interface enabling much more concise and readable code than standard ADO.NET code is typically written.

Common Sequelocity code would be implemented like this:

```csharp
using SequelocityDotNet;

public static SuperHero GetSuperHero( string superHeroName )
{
	const string sqlQuery = @"
SELECT	TOP 1 *
FROM	SuperHero
WHERE	SuperHeroName = @SuperHeroName
";

	var superHero = Sequelocity.GetDatabaseCommand()
		.SetCommandText( sqlQuery )
		.AddParameter( "@SuperHeroName", superHeroName, DbType.AnsiString )
		.ExecuteToList<SuperHero>()
		.FirstOrDefault();
	
	return superHero;
}
```

Please see the project unit tests for many more examples.

###Basic Concepts###

Sequelocity provides two main paths for easy data access: The DatabaseCommand object and DbCommand extension methods.

####DatabaseCommand####

Sequelocity introduces one layer of abstraction upon the native .NET framework System.Data.Common.DbCommand in order to introduce several beneficial features such as automatic connection handling, event handlers, and extension points.

**Automatic Connection Handling**

When utilizing the DatabaseCommand object, all of its execute methods automatically open and close the underlying database connection and dispose of the underlying ADO.NET DbCommand which removes the need for writing the common and repetitive boilerplate code of opening connections, closing connections, and C# using statements.

Through experience, we've found that the majority of database code simply needs to issue a single query and return the results so we've optimized the library to handle this most common scenario with ease by handling all connection-related concerns and disposing of no longer needed resources.

Of course if you are in need to keep a connection open after issuing a command, there are optional parameters that allow you to specify that giving you complete control of the connection handling.

**Event Handlers aka Interception Points aka Hooks**

When utilizing the DatabaseCommand object, all of its execute methods will automatically invoke / call all registered event handlers before calling the database ( Pre-Invoke ), after the database has been called and the results returned ( Post-Invoke ), and upon any unhandled exceptions.

It is very easy to register to one of these events in order to facilitate injecting your own behavior, to assist in debugging, or to enable logging queries, results, or exceptions.

**Extension Points**

If you take a look at the code base for Sequelocity, you will find that almost all of the behavior for the DatabaseCommand object is actually implemented as extension methods. What this means is that new behavior can be easily added by you or your team by simply implementing your own extension methods.

This architectural decision was very much intentional and demonstrates that Sequelocity is not really a framework but a utility and helper library for helping you connect and interact with your ADO.NET supported database of choice. The DatabaseCommand object is just a very simple light-weight wrapper around the .NET framework provided DbCommand in order to enable additional functionality and features.

####DbCommand Extension Methods####

Most of the code in Sequelocity is actually just extension methods and a large portion of the behavior of the Sequelocity DatabaseCommand object simply forwards calls to the many extension methods we've created for the .NET framework provided DbCommand. This enables an alternative way of using Sequelocity as just a set of fluent extension methods for the native DbCommand object in order to reduce the repetitive boilerplate code typically required to be written when working with raw ADO.NET.

A majority of the Sequelocity DbCommand extension methods are implemented with the method both taking in the DbCommand as an argument and returning the DbCommand as the result enabling a fluent style syntax creating a more elegant and concise way of writing data access code.

###Unit Tests and Integration Tests###

Sequelocity is backed by a large test suite comprising over 300 unit and integration tests. These tests serve as a contract specifying the behavior we expect out of each and every method which leads to a lot of tests which appear to be near duplicates of other tests and indeed many of the DatabaseCommand tests are 95% the same code as their DbCommand test counterparts but this is intentional and again is meant to serve as validation of expected method behavior. We hope that such a large test suite will give you confidence and 'the warm and fuzzies' knowing that many, many hours were put into writing tests that cover every inch of code in the Sequelocity code base and it is our hope that this will ensure a low number of bugs, increased quality, and the ability to add new features without breaking existing functionality.

Unit Test vs Integration Test
- We define unit tests as code that attempts to isolate a single unit, which we typically consider a method, and confirm its expected behavior attempting to not exercise any external resources. 
- We define an integration test as basically the same thing as a unit test except the goal is to also exercise the external resources such as running the method against an actual SQLite or Sql Server database. *Note that 'integration test' usually means more than this meaning but for this project this simplified meaning helps to differentiate the two types of testing being performed.

Project Breakdown:
- SequelocityDotNet - This is the main project and contains only a single file named SequelocityDotNet.cs.
- SequelocityDotNet.Tests - These are unit tests as defined above, are the most abundant, and are lightning fast to run.
- SequelocityDotNet.Tests.SQLite - These are integration tests against an in-memory SQLite database and are lightning fast to run.
- SequelocityDotNet.Tests.SqlServer - These are integration tests against a Sql Server database kindly provided by AppHarbor and take about 8 seconds to run depending on your connection speed. *Note that we do not share the ConnectionString to our test Sql Server for obvious reasons so the tests won't be immediately runnable but you can simply replace the ConnectionString with your own and the tests should run just fine as all tests are self contained and will conveniently create any database objects that they need to satisfy the test.

###Database Support###

Sequelocity works with all ADO.NET providers including SQL Server, SQLite, SQL CE, Oracle, MySQL, PostgreSQL, Firebird, etc.

**Database Provider Specific Implementations**

Sequelocity does include a few database provider specific implementation methods listed below. If you would like to contribute additional database provider specific implementations, please feel free to do so by submitting a pull request.

SQLite
- GenerateInsertForSQLite - Generates a parameterized SQLite INSERT statement from the given object
- GenerateInsertsForSQLite - Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects

SQL Server
- GenerateInsertForSqlServer - Generates a parameterized SQLite INSERT statement from the given object
- GenerateInsertsForSqlServer - Generates a list of concatenated parameterized SQLite INSERT statements from the given list of objects

###Open Source###

Sequelocity is open source software licensed under the MIT License. We love open source and use it extensively in all of our projects so it is with great pleasure that we can contribute back to the developer community with Sequelocity. We encourage you to explore the project, use it to your hearts content, provide feedback, open issue or feature requests, contribute to the project and / or fork it for your own use.

If you do find Sequelocity useful, we would love to hear about your experience and add your name to the list of organizations and/or projects that use Sequelocity.

###License###

MIT License:

Copyright (c) 2015 Ambit Energy. All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, including 
without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN 
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 