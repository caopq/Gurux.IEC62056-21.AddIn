using System;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;
using Gurux.Device.Editor;
using Gurux.Device;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// Extends Gurux.Device.GXProperty class with the IEC specific properties.
	/// </summary>
	[Gurux.Device.Editor.GXReadMessage("ReadData", "ReadDataReply", EnableParentRead = false)]
	//Mikko [Gurux.Device.Editor.GXWriteMessage("WriteData", "WriteDataReply")]
	[DataContract()]
	public class GXIEC62056Property : Gurux.Device.GXProperty
	{
		/// <summary>
		/// Initializes a new instance of the GXIECProperty class.
		/// </summary>
		public GXIEC62056Property()
		{
            this.ValueType = typeof(string);
            ReadMode = WriteMode;
		}

        [Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("OBIS code.")]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        public string Data
        {
            get;
            set;
        }

        [Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("Data parameter")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
		public string Parameters
		{
			get;
			set;
		}

		[Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("Read command type.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
		public int ReadMode
		{
			get;
			set;
		}

        [Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("Write command type.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
		public int WriteMode
		{
			get;
			set;
		}

        [System.ComponentModel.Category("Design"), DefaultValue(null),
        System.ComponentModel.Description("The data type of the property, described in the template file.")]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]        
        [Editor(typeof(GXValueTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public DataType DataType
        {
            get;
            set;
        }
	}
}
