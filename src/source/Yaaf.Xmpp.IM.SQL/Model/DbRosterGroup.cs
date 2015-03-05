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

namespace Yaaf.Xmpp.IM.Sql.Model {
	[Table ("RG")]  // Short names because MySql FK keys have a limit
	public class DbRosterGroup {
		[Key]
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
