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

[<DbConfigurationType (typeof<EmptyConfiguration>)>]
type ApplicationDbTestContext() = 
    inherit AbstractRosterStoreDbContext(
        let env = System.Environment.GetEnvironmentVariable ("connection_mssql")
        if System.String.IsNullOrWhiteSpace env then "RosterStore_MSSQL" else env)

    override x.Init() = System.Data.Entity.Database.SetInitializer(new NUnitInitializer<ApplicationDbTestContext>())
    

[<TestFixture>]
[<Category("MSSQL")>]
type ``Test-Yaaf-Xmpp-IM-Sql-DbContext: Check that Sql backend is ok``() = 
    inherit RosterStoreTests()

    override x.CreateRosterStore () = 
        // Fix for some bug, see http://stackoverflow.com/questions/15693262/serialization-exception-in-net-4-5
        System.Configuration.ConfigurationManager.GetSection("dummy") |> ignore
        use context = new ApplicationDbTestContext() :> AbstractRosterStoreDbContext
        context.Database.Delete() |> ignore
        context.SaveChanges() |> ignore
        SqlRosterStore(fun () -> new ApplicationDbTestContext() :> AbstractRosterStoreDbContext) :> IRosterStore
    
    // inheriting the interface tests from the base class!