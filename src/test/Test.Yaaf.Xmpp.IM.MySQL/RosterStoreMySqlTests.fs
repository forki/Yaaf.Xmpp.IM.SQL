// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
namespace Test.Yaaf.Xmpp.IM.Sql.MySql

open Yaaf.Xmpp
open Test.Yaaf.Xmpp.IM
open Yaaf.Xmpp.IM.Sql
open Yaaf.Xmpp.IM.Sql.MySql
open FsUnit
open NUnit.Framework
open Yaaf.Helper
open Yaaf.TestHelper
open Yaaf.Database
open Yaaf.Xmpp.IM.Server

open MySql.Data.Entity
open System.Data.Entity

type ApplicationDbTestContext() as x = 
    inherit MySqlRosterStoreDbContext(
      (let env = System.Environment.GetEnvironmentVariable ("connection_mysql")
       if System.String.IsNullOrWhiteSpace env then "RosterStore_MySQL" else env), false)
  
[<TestFixture>]
[<Category("MYSQL")>]
type ``Test-Yaaf-Xmpp-IM-Sql-DbContext: Check that MySQL backend is ok``() = 
    inherit RosterStoreTests()

    override x.CreateRosterStore () = 
        // Fix for some bug, see http://stackoverflow.com/questions/15693262/serialization-exception-in-net-4-5
        System.Configuration.ConfigurationManager.GetSection("dummy") |> ignore
        //System.Data.Entity.Database.SetInitializer(new NUnitInitializer<ApplicationDbTestContext>())
        System.Data.Entity.Database.SetInitializer<ApplicationDbTestContext>(null)

        use context = new ApplicationDbTestContext() :> AbstractRosterStoreDbContext
        context.Database.Delete() |> ignore
        context.SaveChanges() |> ignore
        context.Upgrade()
        SqlRosterStore(fun () -> new ApplicationDbTestContext() :> AbstractRosterStoreDbContext) :> IRosterStore
    
    // inheriting the interface tests from the base class!