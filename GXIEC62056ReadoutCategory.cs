using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Gurux.IEC62056_21.AddIn
{
	[Gurux.Device.Editor.GXReadMessage("Readout", Index = 1)]
	[DataContract()]
	public class GXIEC62056ReadoutCategory : Gurux.Device.GXCategory
	{
		/// <summary>
		/// Initializes a new instance of the GXIEC62056ReadoutCategory class.
		/// </summary>
		public GXIEC62056ReadoutCategory()
		{
		}
	}
}
