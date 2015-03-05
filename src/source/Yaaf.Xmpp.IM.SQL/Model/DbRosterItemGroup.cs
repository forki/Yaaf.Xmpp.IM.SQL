// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yaaf.Xmpp.IM.Sql.Model {
	[Table ("RIG")]  // Short names because MySql FK keys have a limit
	public class DbRosterItemGroup {
		//[Required]
		[Key, Column (Order = 0)]
		[ForeignKey ("RosterItem")]
		public string ApplicationUserId { get; set; }
		//[Required]
		[Key, Column (Order = 1)]
		[ForeignKey ("RosterItem")]
		public string RosterItemId { get; set; }

		public virtual DbRosterItem RosterItem { get; set; }


		[Key, Column (Order = 2)]
		public int RosterGroupId { get; set; }
		[ForeignKey ("RosterGroupId")]
		public virtual DbRosterGroup RosterGroup { get; set; }
	}
}
