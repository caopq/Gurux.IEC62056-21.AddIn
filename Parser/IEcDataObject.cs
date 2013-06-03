using System;
using System.Collections.Generic;
using System.Text;

namespace Gurux.IEC62056_21.AddIn.Parser
{
	class IECDataObject
	{
		public IECDataObject()
		{
			RowValues = new List<string>();
		}

		public string Address
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public List<string> RowValues
		{
			get;
			set;
		}

		public string Unit
		{
			get;
			set;
		}
	}
}
