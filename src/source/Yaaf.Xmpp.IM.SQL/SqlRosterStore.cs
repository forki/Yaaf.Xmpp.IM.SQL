// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaaf.Xmpp.IM.Sql.Model;

namespace Yaaf.Xmpp.IM.Sql
{


	public class SqlRosterStore : Yaaf.Xmpp.IM.Server.IRosterStore {
		private Func<AbstractRosterStoreDbContext> contextCreator;
		public SqlRosterStore (Func<AbstractRosterStoreDbContext> contextCreator)
		{
			this.contextCreator = contextCreator;
		}

		private async Task<string> GetUser (Yaaf.Xmpp.JabberId value)
		{
			using (var context = contextCreator ()) {
				var username = value.Localpart.Value;

				var um = new UserManager<ApplicationUser> (
						new UserStore<ApplicationUser> (context));
				var user = await um.FindByNameAsync (username);

				if (user == null) {
					// create user
					user = new ApplicationUser () { UserName = username };
					var res = await um.CreateAsync (user);
					if (!res.Succeeded) {
						throw new Exception (string.Format ("Error while creating new user: {0}", string.Join (", ", res.Errors)));
					}
                    await context.MySaveChanges();
					if (string.IsNullOrEmpty (user.Id)) {
						Console.Error.WriteLine ("User Entity was not refreshed, Please remove me!");
						user = await um.FindByNameAsync (username);
					}
				}

				if (string.IsNullOrEmpty (user.Id)) {
					throw new Exception (string.Format ("Could not find or create user: {0}", username));
				}

				context.Entry (user).State = System.Data.Entity.EntityState.Detached;
				return user.Id;
			}
		}

		public bool ExistsUser (Yaaf.Xmpp.JabberId value)
		{
			return GetUser(value) != null;
		}

		public async Task<Server.IUserRoster> GetRoster (Yaaf.Xmpp.JabberId value)
		{
			return new SqlUserStore (contextCreator, await GetUser (value));
		}

		public static FSharpOption<T> ToFSharp<T>(T data) {
			if (data == null) {
				return FSharpOption<T>.None;
			}
			return FSharpOption<T>.Some (data);
		}
		public static T FromFSharp<T> (FSharpOption<T> data) where T : class
		{
			return FromFSharp (data, null);
		}
		public static T FromFSharp<T> (FSharpOption<T> data, T def)
		{
			if (data == null) {
				return def;
			}
			return data.Value;
		}
	}
}
