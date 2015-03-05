// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Yaaf.Xmpp.XmlStanzas;

namespace Yaaf.Xmpp.IM.Sql.Model {
	[Table ("AU")] // Short names because MySql FK keys have a limit
	public class ApplicationUser : IdentityUser {

		public int RosterVersion { get; set; }

		// TODO query RosterChanges for oldest version
		public int OldestRosterVersion { get; set; }

		public string DbCurrentPresenceData { get; set; }

		[NotMapped]
		private static PresenceStatus Offline =
			PresenceStatus.NewSetStatusUnavailable(
				ListModule.OfSeq( 
					new [] {
						Tuple.Create<string, FSharpOption<Langcode>>("offline", null)}));
		[NotMapped]
		public PresenceStatus CurrentPresenceData
		{
			get
			{
				var raw = DbCurrentPresenceData;
				if (string.IsNullOrEmpty (raw)) {
					return Offline;
				}
				var elem = XElement.Parse(raw);
				var presenceStanza = Parsing.parsePresenceStanza ("jabber:server", elem);
				if (presenceStanza.Data.IsStatusInfo) {
					var info = (PresenceProcessingType.StatusInfo)presenceStanza.Data;
					return info.Item;
				} else {
					// Log error
					return Offline;
				}
			}
			set
			{
				var presenceData = PresenceProcessingType.NewStatusInfo (value);
				var presence = Parsing.createPresenceElement (null, null, null, presenceData);
                var elem = XmlStanzas.Parsing.createStanzaElement("jabber:server", presence);
				DbCurrentPresenceData = elem.ToString ();
			}
		}


		public virtual ICollection<DbRosterItem> Roster { get; set; }
		public virtual ICollection<DbRosterItemGroup> RosterGroups { get; set; }
		public virtual ICollection<DbRosterChange> RosterChanges { get; set; }
		public virtual ICollection<DbSubscriptionRequest> SubscriptionsRequests { get; set; }
	}
}
