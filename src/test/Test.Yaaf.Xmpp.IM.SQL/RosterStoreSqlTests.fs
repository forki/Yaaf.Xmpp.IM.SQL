// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Test.Yaaf.Xmpp.IM.Sql

open Yaaf.Xmpp
open Test.Yaaf.Xmpp.IM
open Yaaf.Xmpp.IM.Sql
open FsUnit
open NUnit.Framework
open Yaaf.Helper
open Yaaf.TestHelper
open Yaaf.Database
open Yaaf.Xmpp.IM.Server
open System.Data.Entity
open System.Data.SqlClient

[<DbConfigurationType (typeof<EmptyConfiguration>)>]
type ApplicationDbTestContext() =
    inherit AbstractRosterStoreDbContext(ApplicationDbTestContext.ConnectionName)

    override x.Init() = System.Data.Entity.Database.SetInitializer(new NUnitInitializer<ApplicationDbTestContext>())
    static member ConnectionName
      with get () =  
        let env = System.Environment.GetEnvironmentVariable ("connection_mssql")
        if System.String.IsNullOrWhiteSpace env then "RosterStore_MSSQL" else env

[<TestFixture>]
[<Category("MSSQL")>]
type ``Test-Yaaf-Xmpp-IM-Sql-DbContext: Check that Sql backend is ok``() = 
    inherit RosterStoreTests()

    [<TestFixtureSetUp>]
    member x.FixtureSetup () =
        ()

    member x.DetachDatabase() =
        // This is currently completly broken.
        let nameOrCon = ApplicationDbTestContext.ConnectionName
        let connString =
            if not (nameOrCon.Contains (";")) then 
                System.Configuration.ConfigurationManager.ConnectionStrings.[nameOrCon].ConnectionString
            else nameOrCon
        
        use sqlDatabaseConnection = new SqlConnection(connString)
        try
            sqlDatabaseConnection.Open()
            let dbName = "rosterdb-nunit"
            let commandString = 
                sprintf "ALTER DATABASE '%s' SET OFFLINE WITH ROLLBACK IMMEDIATE ALTER DATABASE '%s' SET SINGLE_USER EXEC sp_detach_db '%s'"
                   dbName dbName dbName
            let sqlDatabaseCommand = new SqlCommand(commandString, sqlDatabaseConnection)
            sqlDatabaseCommand.ExecuteNonQuery() |> ignore
        with
        | ex ->
            printfn "DetachDatabase failed: %O" ex
    
    [<TestFixtureTearDown>]
    member x.FixtureTearDown() =
        //x.DetachDatabase()
        ()

    override x.CreateRosterStore () = 
        // Fix for some bug, see http://stackoverflow.com/questions/15693262/serialization-exception-in-net-4-5
        System.Configuration.ConfigurationManager.GetSection("dummy") |> ignore
        use context = new ApplicationDbTestContext() :> AbstractRosterStoreDbContext
        context.Database.Delete() |> ignore
        context.SaveChanges() |> ignore
        SqlRosterStore(fun () -> new ApplicationDbTestContext() :> AbstractRosterStoreDbContext) :> IRosterStore
    
    // inheriting the interface tests from the base class!