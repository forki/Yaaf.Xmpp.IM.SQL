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
	public enum DbChangeType {
		Set = 1,
		Delete = 2,
	}

	[Table ("RC")]  // Short names because MySql FK keys have a limit
	public class DbRosterChange {
		[Key, Column (Order = 0)]
		public string ApplicationUserId { get; set; }
		[ForeignKey ("ApplicationUserId")]
		public virtual ApplicationUser ApplicationUser { get; set; }

		[Key, Column (Order = 1)]
		public int Version { get; set; }

		public string Jid { get; set; }

		public int DbChangeType { get; set; }

		[NotMapped]
		public DbChangeType ChangeType
		{ get { return (DbChangeType) DbChangeType; } set { DbChangeType = (int) value; } }

	}
}
