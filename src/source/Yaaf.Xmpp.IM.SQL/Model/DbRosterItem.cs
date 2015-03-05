// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using Microsoft.FSharp.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaaf.Xmpp.IM.Sql.Model {
	public enum DbAskType
	{
	    None = 0,
		Subscribe = 1,
	}
	public enum DbSubscriptionType {
		NotEntered = -1,
	    None = 0,
		To = 1,
		From = 2,
		Both = 3
	}

	[Table ("RI")]  // Short names because MySql FK keys have a limit
	public class DbRosterItem {
		[Key, Column (Order = 0)]
		public string ApplicationUserId { get; set; }
		[ForeignKey ("ApplicationUserId")]
		public virtual ApplicationUser ApplicationUser { get; set; }

		[Key, Column (Order = 1)]
		public string Jid { get; set; } 
		public string Name { get; set; }
		public bool Approved { get; set; }
		public int DbAskType { get; set; }
		
		[NotMapped]
		public DbAskType Ask
		{ get { return (DbAskType) DbAskType; } set { DbAskType = (int) value; } }
		public int DbSubscriptionType { get; set; }

		[NotMapped]
		public DbSubscriptionType Subscription
		{ get { return (DbSubscriptionType) DbSubscriptionType; } set { DbSubscriptionType = (int) value; } }

		public virtual ICollection<DbRosterItemGroup> Groups { get; set; }
		//public string GroupList { get; set; }
		//private 

		public static DbRosterItem FromFSharp (RosterItem item)
		{
			var subs = Yaaf.Xmpp.IM.Sql.Model.DbSubscriptionType.NotEntered;
			if (item.Subscription != null) {
				if (item.Subscription.Value == SubscriptionType.Both) {
					subs = Model.DbSubscriptionType.Both;
				} else if (item.Subscription.Value == SubscriptionType.From) {
					subs = Model.DbSubscriptionType.From;
				} else if (item.Subscription.Value == SubscriptionType.To) {
					subs = Model.DbSubscriptionType.To;
				} else if (item.Subscription.Value == SubscriptionType.SubsNone) {
					subs = Model.DbSubscriptionType.None;
				}
			}

			var ask = Yaaf.Xmpp.IM.Sql.Model.DbAskType.None;
			if (item.Ask != null) {
				if (item.Ask.Value == AskType.Subscribe) {
					ask = Model.DbAskType.Subscribe;
				} 
			}
			var rosterItem = new DbRosterItem () {
				Jid = item.Jid.BareId,
				Name = SqlRosterStore.FromFSharp(item.Name),
				Approved = SqlRosterStore.FromFSharp(item.Approved, false),
				Ask = ask,
				Subscription = subs
			};
			var groups = new List<DbRosterItemGroup> ();
			foreach (var group in item.Groups) {
				var groupEntity = new DbRosterGroup () { Name = group };
				var groupCon = new DbRosterItemGroup () { RosterGroup = groupEntity, RosterItem = rosterItem };
				groups.Add (groupCon);
			}
			rosterItem.Groups = groups;
			return rosterItem;
		}

		public RosterItem ToFSharp ()
		{
			Microsoft.FSharp.Core.FSharpOption<AskType> approved = null;
			Microsoft.FSharp.Core.FSharpOption<SubscriptionType> subs = null;
			switch (Ask)
			{
			case Yaaf.Xmpp.IM.Sql.Model.DbAskType.Subscribe:
				 approved = new Microsoft.FSharp.Core.FSharpOption<AskType>(AskType.Subscribe); 
				break;
			default:
				break;
			}
			switch (Subscription)
			{
			case Yaaf.Xmpp.IM.Sql.Model.DbSubscriptionType.Both:
				 subs = new Microsoft.FSharp.Core.FSharpOption<SubscriptionType>(SubscriptionType.Both); 
				break;
			case Yaaf.Xmpp.IM.Sql.Model.DbSubscriptionType.None:
				 subs = new Microsoft.FSharp.Core.FSharpOption<SubscriptionType>(SubscriptionType.SubsNone); 
				break;
			case Yaaf.Xmpp.IM.Sql.Model.DbSubscriptionType.To:
				 subs = new Microsoft.FSharp.Core.FSharpOption<SubscriptionType>(SubscriptionType.To); 
				break;
			case Yaaf.Xmpp.IM.Sql.Model.DbSubscriptionType.From:
				 subs = new Microsoft.FSharp.Core.FSharpOption<SubscriptionType>(SubscriptionType.From); 
				break;
			default:
				break;
			}
			var list = ListModule.OfSeq (from g in Groups select g.RosterGroup.Name);
			return new RosterItem (
				JabberId.Parse(Jid), 
				SqlRosterStore.ToFSharp (Name), 
				!Approved ? null : (SqlRosterStore.ToFSharp (true)),
				approved, subs, list);
		}
	}
}
