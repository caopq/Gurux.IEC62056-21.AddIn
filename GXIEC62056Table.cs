using System;
using System.Reflection;
using System.ComponentModel;
using Gurux.Device.Editor;
using Gurux.Communication;
using System.Runtime.Serialization;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// Extends Gurux.Device.GXProperty class with the IEC specific properties.
	/// </summary>
	//[Gurux.Device.Editor.GXReadMessage("ReadTableData", "ReadTableDataReply")]
	[Gurux.Device.Editor.GXReadMessage("ReadTableData", "ReadTableDataReply", "IsTableRead", "ReadTableDataNext")]
	[DataContract()]
    public class GXIEC62056Table : Gurux.Device.GXTable, IGXPartialRead
	{
		/// <summary>
		/// Initializes a new instance of the GXIECProperty class.
		/// </summary>
		public GXIEC62056Table()
		{
            ((IGXPartialRead)this).Type = PartialReadType.New | PartialReadType.All | PartialReadType.Last | PartialReadType.Range;
			ReadMode = "6";
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
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
		public string ReadMode
		{
			get;
			set;
		}

		private GXIEC62056TableProperty CreateTableProperty(string name, string obisCode, string unit)
		{
			name = name.Replace('-', '.');
			name = name.Replace(':', '.');
			GXIEC62056TableProperty prop = new GXIEC62056TableProperty();
			prop.Name = name;
			prop.Address = obisCode;
			this.Columns.Add(prop);
			return prop;
		}

        #region IGXPartialRead Members

        public PartialReadType Type
        {
            get;
            set;
        }

		private object m_Start = null;
        public object Start
        {
			get
			{
				return m_Start;
			}
			set
			{
				m_Start = value;
			}
        }

        public object End
        {
            get;
            set;
        }

        #endregion
    }
}
