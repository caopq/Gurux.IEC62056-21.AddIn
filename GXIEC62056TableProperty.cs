using System;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;
using Gurux.Device.Editor;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// Extends Gurux.Device.GXProperty class with the IEC specific properties.
	/// </summary>
	[DataContract()]
    [Serializable]
	public class GXIEC62056TableProperty : Gurux.Device.GXProperty
	{
		/// <summary>
		/// Initializes a new instance of the GXIECProperty class.
		/// </summary>
		public GXIEC62056TableProperty()
		{
		}

		[Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("OBIS code.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.None, ValueAccessType.None)]
        public string Data
		{
			get;
			set;
		}

        [System.ComponentModel.Category("Design"), DefaultValue(null),
        System.ComponentModel.Description("The UI data type of the property, described in the template file.")]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        [Editor(typeof(GXValueTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        override public Type ValueType
        {
            get;
            set;
        }
	}
}
