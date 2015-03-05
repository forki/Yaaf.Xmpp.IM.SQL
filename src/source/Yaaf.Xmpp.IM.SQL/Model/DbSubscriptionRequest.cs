// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Yaaf.Xmpp.IM.Sql.Model {
	[Table ("SR")]  // Short names because MySql FK keys have a limit
	public class DbSubscriptionRequest {
		[Key, Column (Order = 0)]
		public string ApplicationUserId { get; set; }
		[ForeignKey ("ApplicationUserId")]
		public virtual ApplicationUser ApplicationUser { get; set; }

		[Key, Column (Order = 1)]
		public string FromJid { get; set; }

		public string Content { get; set; }


		public static DbSubscriptionRequest FromFSharp (JabberId from, XmlStanzas.Stanza<PresenceProcessingType> presenceStanza)
		{
            var elem = XmlStanzas.Parsing.createStanzaElement("jabber:server", presenceStanza);
			return
				new DbSubscriptionRequest () {
					FromJid = from.BareId,
					Content = elem.ToString ()
				};
		}

		public Tuple<JabberId, XmlStanzas.Stanza<PresenceProcessingType>> ToFSharp ()
		{
			var elem = XElement.Parse (Content);
			var presenceStanza = Parsing.parsePresenceStanza ("jabber:server", elem);
			var from = JabberId.Parse(FromJid);
			return Tuple.Create(from, presenceStanza);
		}
	}
}
