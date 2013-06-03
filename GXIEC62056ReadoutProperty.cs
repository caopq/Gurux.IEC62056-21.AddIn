using System;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;
using Gurux.Device.Editor;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// Reserved for future use
	/// </summary>
	[DataContract()]
	public class GXIEC62056ReadoutProperty : Gurux.Device.GXProperty
	{
		/// <summary>
		/// Initializes a new instance of the GXIECProperty class.
		/// </summary>
		public GXIEC62056ReadoutProperty()
		{
		}

		[Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("OBIS code.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.None, ValueAccessType.None)]
		public string Address
		{
			get;
			set;
		}

		[Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("Read Mode.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.None, ValueAccessType.None)]
		public string ReadMode
		{
			get;
			set;
		}

		[Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("Write Mode.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.None, ValueAccessType.None)]
		public string WriteMode
		{
			get;
			set;
		}
	}
}
