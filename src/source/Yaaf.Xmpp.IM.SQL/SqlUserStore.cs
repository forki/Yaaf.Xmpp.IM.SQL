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
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yaaf.Xmpp.IM.Sql.Model;
using Yaaf.Xmpp.XmlStanzas;

namespace Yaaf.Xmpp.IM.Sql {
	public class SqlUserStore : Yaaf.Xmpp.IM.Server.IUserRoster {
		private readonly string user;
		private Func<AbstractRosterStoreDbContext> contextCreator;

		public SqlUserStore (Func<AbstractRosterStoreDbContext> contextCreator, string user)
		{
			if (string.IsNullOrWhiteSpace(user)) {
				throw new ArgumentNullException ("user");
			}
			if (contextCreator == null) {
				throw new ArgumentNullException ("contextCreator");
			}

			this.contextCreator = contextCreator;
			this.user = user;
		}

		private AbstractRosterStoreDbContext CreateContext ()
		{
			var context = contextCreator ();
			return context;
		}

		public async Task<Microsoft.FSharp.Collections.FSharpList<Tuple<RosterChange, string>>> ChangesSince (string value)
		{
			using (var context = CreateContext ()) {
				var lastVersion = int.Parse (value);
				var latestChanges =
					/*
					// Not working in MySql
					from change in context.RosterChanges
					where change.ApplicationUserId == user.Id
					where change.Version > lastVersion
					group change by change.Jid into g
					let latest =
								(from item in g
								orderby item.Version descending
								select item)
								.FirstOrDefault()
					orderby latest.Version
					select latest;
				
					// Not working in MySql
					context.RosterChanges
						.Where (c => c.ApplicationUserId == user.Id && c.Version > lastVersion)
						.GroupBy (c => c.Jid)
						.Select (g => g.OrderByDescending(i => i.Version).FirstOrDefault())
						.OrderBy(i => i.Version); */

					from change in context.RosterChanges
					join gro in
						(from c in context.RosterChanges
						 group c by c.Jid into g
						 select new { Name = g.Key, MaxVer = g.Select (i => i.Version).Max () })
							on change.Jid equals gro.Name
					where change.ApplicationUserId == user &&
							change.Version > lastVersion &&
							change.Version == gro.MaxVer
					orderby change.Version
					select change;

				var list = new List<Tuple<RosterChange, string>> ();
				foreach (var item in await latestChanges.ToListAsync ()) {
					// build list
					RosterChange change = null;
					switch (item.ChangeType) {
					case DbChangeType.Set:
						var items =
							await (from i in context.RosterItems
							 where i.ApplicationUserId == user
							 where i.Jid == item.Jid
							 select i)
							.FirstOrDefaultAsync ();

						change = RosterChange.NewSetItem (items.ToFSharp ());
						break;
					case DbChangeType.Delete:
						change = RosterChange.NewDeleteItem (JabberId.Parse (item.Jid));
						break;
					default:
						throw new InvalidOperationException ("Invalid ChangeType");
					}
					var t = Tuple.Create (change, VersionString (item.Version));
					list.Add (t);
				}
				return ListModule.OfSeq (list);
				//return null;
			}
		}
		private async Task<ApplicationUser> GetUser (AbstractRosterStoreDbContext context)
		{
			var um = new UserManager<ApplicationUser> (
					new UserStore<ApplicationUser> (context));
			return await um.FindByIdAsync(user);
		}


		private async Task<int> CurrentVersion (AbstractRosterStoreDbContext context)
		{
			return (await GetUser (context)).RosterVersion;
		}
		public async Task<string> GetCurrentRosterVersion()
		{

			using (var context = CreateContext ()) {
				return VersionString (await CurrentVersion (context));
			}
		}
		public Task<string> CurrentRosterVersion
		{
			get {
				return GetCurrentRosterVersion ();
			}
		}

		private string VersionString (int version)
		{
			return version.ToString ();
		}

		private int NextVersion(int currentVersion)
		{
			return currentVersion + 1;
		}
		private async Task<int> NextVersion (AbstractRosterStoreDbContext context)
		{
			return NextVersion(await CurrentVersion (context));
		}

		private async Task<string> SaveNextVersion (AbstractRosterStoreDbContext context)
		{
			var user = await GetUser (context);
			user.RosterVersion = NextVersion(user.RosterVersion);
			await context.MySaveChanges ();
			return VersionString(user.RosterVersion);
		}

		public async Task<string> DeleteItem (JabberId value)
		{
			using (var context = CreateContext ()) {
				var jid = value.BareId;
				context.RosterItems.RemoveRange (await
				   (from item in context.RosterItems
					where item.ApplicationUserId == user
					where item.Jid == jid
					select item).ToListAsync ());
				var change =
					new DbRosterChange () {
						Version = await NextVersion(context),
						ApplicationUserId = user,
						Jid = jid,
						ChangeType = DbChangeType.Delete
					};
				//user.RosterChanges.Add (change);
				context.RosterChanges.Add (change);
				return await SaveNextVersion (context);
			}
		}

		public async Task<Microsoft.FSharp.Core.FSharpOption<RosterItem>> GetItem (JabberId value)
		{
			using (var context = CreateContext ()) {
				var jid = value.BareId;
				var rosterItem = await
					(from item in context.RosterItems
					 where item.ApplicationUserId == user
					 where item.Jid == jid
					 select item).FirstOrDefaultAsync ();
				if (rosterItem == null) {
					return FSharpOption<RosterItem>.None;
				}
				return FSharpOption<RosterItem>.Some (rosterItem.ToFSharp ());
			}
		}

		public async Task<Microsoft.FSharp.Collections.FSharpList<RosterItem>> GetItems ()
		{
			using (var context = CreateContext ()) {
				IEnumerable<DbRosterItem> data = await
					(from item in context.RosterItems.Include ("Groups")
					 where item.ApplicationUserId == user
					 select item).ToListAsync ();

				return ListModule.OfSeq (data.Select (s => s.ToFSharp ()));
			}
		}

		public async Task<int> GetRosterSize ()
		{
			using (var context = CreateContext ()) {
				var count = await (from item in context.RosterItems
								   where item.ApplicationUserId == user
								   select item).CountAsync ();
				return count;
			}
		}

		public async Task<string> UpdateOrAddRosterItem (bool onlyUpdate, RosterItem value)
		{
			using (var context = CreateContext ()) {
				var item = DbRosterItem.FromFSharp (value);
				item.ApplicationUserId = user;
				if (onlyUpdate) {
					var contextTrackedItem = await context.RosterItems.FindAsync (item.ApplicationUserId, item.Jid);
					if (contextTrackedItem == null) {
						//foreach (var group in item.Groups) {
						//	group.ApplicationUserId = user;
						//	context.RosterItemGroups.Add (group);
						//	//context.Entry (group).State = EntityState.Added;
						//	//context.Entry (group.RosterGroup).State = EntityState.Added;
						//}
						context.RosterItems.Add (item);
						//context.Entry (item).State = EntityState.Modified;
					} else {
						System.Diagnostics.Debug.Assert(contextTrackedItem.ApplicationUserId == user);
						// delete all groups
						context.RosterItemGroups.RemoveRange (contextTrackedItem.Groups);
							//from ri in context.RosterItemGroups
							//where ri.RosterItemId == contextTrackedItem.Jid && ri.ApplicationUserId == contextTrackedItem.ApplicationUserId
							//select ri);

						context.Entry (contextTrackedItem).CurrentValues.SetValues (item);
						contextTrackedItem.Groups.Clear ();
						foreach (var group in item.Groups) {
							group.ApplicationUserId = contextTrackedItem.ApplicationUserId;
							group.RosterItemId = contextTrackedItem.Jid;
							group.RosterItem = contextTrackedItem;
							contextTrackedItem.Groups.Add (group);
						}
					}
				} else {
					context.RosterItems.Add (item);
				}

				var change =
					new DbRosterChange () {
						Version = await NextVersion(context),
						ApplicationUserId = user,
						Jid = value.Jid.BareId,
						ChangeType = DbChangeType.Set
					};
				context.RosterChanges.Add (change);
				//context.RosterChanges.Add (change);
				return await SaveNextVersion (context);
			}
		}


		public async Task StoreSubscriptionRequest (JabberId jid, XmlStanzas.Stanza<PresenceProcessingType> value)
		{
			using (var context = CreateContext ()) {
				var request = DbSubscriptionRequest.FromFSharp (jid, value);
				request.ApplicationUserId = user;
				var contextItem = await context.SubscriptionRequests.FindAsync (request.ApplicationUserId, request.FromJid);
				if (contextItem == null) {
					context.SubscriptionRequests.Add (request);
				} else {
					context.Entry (contextItem).CurrentValues.SetValues (request);
				}
				await context.MySaveChanges ();
			}
		}


		public async Task RemoveSubscriptionRequest (JabberId value)
		{
			using (var context = CreateContext ()) {
				var jid = value.BareId;
				context.SubscriptionRequests.RemoveRange
					(await (from req in context.SubscriptionRequests
					 where req.ApplicationUserId == user && req.FromJid == jid
					 select req).ToListAsync());
				await context.MySaveChanges ();
			}
		}

		public async Task<FSharpList<Tuple<JabberId, XmlStanzas.Stanza<PresenceProcessingType>>>> RetrieveStoredSubscriptionRequests ()
		{
			using (var context = CreateContext ()) {
				var subs =
					(from req in context.SubscriptionRequests
					 where req.ApplicationUserId == user
					 select req);
				var stanzas = (await subs.ToListAsync() as IEnumerable<DbSubscriptionRequest>).Select (req => req.ToFSharp ());
				return ListModule.OfSeq (stanzas);
			}
		}
		private PresenceStatus currentPresenceData = 
			PresenceStatus.NewSetStatusUnavailable(
				ListModule.OfSeq( 
					new [] {
						Tuple.Create<string, FSharpOption<Langcode>>("offline", null)}));

		public async Task<PresenceStatus> GetCurrentPresence() 
		{
			using (var context = CreateContext()) {
				return (await GetUser (context)).CurrentPresenceData;
			}
		}
		public async Task SetCurrentPresence(PresenceStatus presence) 
		{
			if (presence == null) {
				throw new ArgumentNullException ("value");
			}
			using (var context = CreateContext ()) {
				(await GetUser (context)).CurrentPresenceData = presence;
				await context.MySaveChanges ();
			}
		}
		

	}
}
