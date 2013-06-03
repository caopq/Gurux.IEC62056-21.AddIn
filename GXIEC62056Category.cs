using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Gurux.IEC62056_21.AddIn
{
	[DataContract()]
	public class GXIEC62056Category : Gurux.Device.GXCategory
	{
		/// <summary>
		/// Initializes a new instance of the GXIEC62056Category class.
		/// </summary>
		public GXIEC62056Category()
		{
		}
	}
}
