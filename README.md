# ef-localdb-mock
Entity Framework Mocking using SQL Server LocalDB

# NuGet

	 Install-Package NeuroSpeech.EFLocalDBMock

# Example For xUnit


	using NeuroSpeech.EFLocalDBMock;


	// Assuming you have `AppDbContext` as your EF DbContext in side your actual application project
	// For test purposes, you will have to use AppDbTestContext 
	// or you can use AppDbTestContext as dependency in your DI container
	public class AppDbTestContext : AppDbContext {

		// this is important
		// since databases are dynamically created and destroyed
		// MockDatabaseContext.Current.ConnectionString contains 
		// correct database for current test context
		
		// MockDatabaseContext.Current will work correctly with async await
		// without worrying about passing context

		public AppDbTestContext(): base(MockDatabaseContext.Current.ConnectionString)
		{

		}
	}

	public class AppDbCleanMigrationConfiguration : DbMigrationsConfiguration<AppDbTestContext>
	{
		public AppDbCleanMigrationConfiguration()
		{
			this.AutomaticMigrationDataLossAllowed = true;
			this.AutomaticMigrationsEnabled = true;
		}
	}

	public abstract class BaseTest: MockSqlDatabaseContext<AppDbTestContext, AppDbCleanMigrationConfiguration>
	{

		public BaseTest(ITestOutputHelper writer)
		{
			this.Writer = writer;
		}

		protected override void DumpLogs()
		{
			this.Writer.WriteLine(base.GeneratedLog);
		}

		public ITestOutputHelper Writer { get; private set; }
	}

Now you can derive all your tests from BaseTest and you can use AppDbTestContext 
