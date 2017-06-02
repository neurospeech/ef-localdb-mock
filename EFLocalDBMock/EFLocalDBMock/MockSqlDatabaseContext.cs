using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;

namespace NeuroSpeech.EFLocalDBMock
{


    /// <summary>
    /// This class will create a temporary database inside SQL Server LocalDB instance.
    /// 
    /// In order to avoid multiple creation and initial seeding, this class will create
    /// database and perform migration including seeding only once per change in version of 
    /// assembly containing the context.
    /// 
    /// For every subsequent calls, MDF/LDF files of initial version will be copied as a 
    /// new database for every test.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <typeparam name="TContextConfiguration">The type of the context configuration.</typeparam>
    /// <seealso cref="NeuroSpeech.EFLocalDBMock.MockDatabaseContext" />
    public class MockSqlDatabaseContext<TDbContext,TContextConfiguration> : MockDatabaseContext
        where TDbContext : DbContext
        where TContextConfiguration: DbMigrationsConfiguration<TDbContext>, new ()
        
    {

        protected override void OnBeforeInitialize()
        {
            try
            {
                Database.SetInitializer<TDbContext>(new MigrateDatabaseToLatestVersion<TDbContext, TContextConfiguration>());
            }
            catch (Exception ex) {
                WriteLine(ex.ToString());
                    
            }

        }

        internal override void Initialize()
        {

            try
            {

#pragma warning disable CS0436 // Type conflicts with imported type
                //SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);
#pragma warning restore CS0436 // Type conflicts with imported type


                OnBeforeInitialize();



                var od = OriginalDatabase<TDbContext>.Instance;

                TempFiles.Add(od.DbFile);
                TempFiles.Add(od.LogFile);


                DBName = od.DBName;

                ConnectionString = (new SqlConnectionStringBuilder()
                {
                    DataSource = "(localdb)\\MSSQLLocalDB",
                    //sqlCnstr.AttachDBFilename = t;
                    InitialCatalog = od.DBName,
                    IntegratedSecurity = true,
                    ApplicationName = "EntityFramework"
                }).ToString();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
                throw;
            }


        }
    }
}
