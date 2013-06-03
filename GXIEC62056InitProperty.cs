using System;
using System.Reflection;
using System.ComponentModel;
using Gurux.Device.Editor;
using Gurux.Device;
using System.Runtime.Serialization;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// Extends Gurux.Device.GXProperty class with the IEC specific properties.
	/// </summary>
	[Gurux.Device.Editor.GXReadMessage("InitRead", Index = 1)]
	[DataContract()]
	public class GXIEC62056InitProperty : GXProperty
	{
		/// <summary>
		/// Initializes a new instance of the GXIECProperty class.
		/// </summary>
		public GXIEC62056InitProperty()
		{
		}
	}
}
