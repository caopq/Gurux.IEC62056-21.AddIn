using System;
using Gurux.Device.Editor;
using System.ComponentModel;
using System.Runtime.Serialization;
using Gurux.Device;
using Gurux.Net;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// Extends Gurux.Device.GXDevice class with the IEC specific properties.
	/// </summary>
	[DataContract()]
	[GXInitialActionMessage(InitialActionType.Connected, "Connect", Index = 1)]
    [GXInitialActionMessage(InitialActionType.Disconnecting, "Disconnect", "DisconnectReply", Index = 1)]
	public class GXIEC62056Device : Gurux.Device.GXDevice
	{
		/// <summary>
		/// Initializes a new instance of the GXIECDevice class.
		/// </summary>
		public GXIEC62056Device()
		{
            this.Mode = Protocol.None;
            SerialNumber = "";
			this.GXClient.Bop = (byte)0x1;
			this.GXClient.Eop = (byte)0x3;
			this.WaitTime = 5000;
			this.ResendCount = 0;
			this.GXClient.ChecksumSettings.Count = -1;
			this.GXClient.ChecksumSettings.Position = -1;
			this.GXClient.ChecksumSettings.Type = Gurux.Communication.ChecksumType.Custom;
			this.GXClient.ChecksumSettings.Size = 8;
			this.GXClient.ChecksumSettings.Polynomial = 0xA001;
			this.GXClient.ChecksumSettings.InitialValue = 0;
			this.GXClient.ChecksumSettings.FinalXOR = 0;
			this.GXClient.ChecksumSettings.ReverseData = false;
			this.GXClient.ChecksumSettings.Reflection = false;
			this.GXClient.ChecksumSettings.Start = 1;
			this.GXClient.ByteOrder = Gurux.Communication.ByteOrder.BigEndian;

			SetAllowedMediaTypes();
		}

		void SetAllowedMediaTypes()
		{
			this.AllowedMediaTypes.Clear();

			GXMediaType media = new GXMediaType();
            GXNet net = new GXNet();
            net.Protocol = NetworkType.Tcp;
            net.Port = 1000;
			media.Name = "Net";
            media.DefaultMediaSettings = net.Settings;
			this.AllowedMediaTypes.Add(media);

			media = new GXMediaType();
			media.Name = "Serial";
			Gurux.Serial.GXSerial serial = new Gurux.Serial.GXSerial();
			serial.BaudRate = 300;
			serial.DataBits = 7;
			serial.Parity = System.IO.Ports.Parity.Even;
			serial.StopBits = System.IO.Ports.StopBits.One;
			media.DefaultMediaSettings = serial.Settings;
			this.AllowedMediaTypes.Add(media);
		}
        
        [DefaultValue(Protocol.None), Browsable(true), ReadOnly(false), System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("Is Programming Mode supported.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        public Protocol Mode
		{
			get;
			set;
		}

		[Browsable(false), ReadOnly(false), 
        System.ComponentModel.Category("Behavior"), 
        System.ComponentModel.Description("Device serial number.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.Edit)]
		public string SerialNumber
        {
			get;
			set;
        }	

		[Browsable(true), ReadOnly(false), System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("The password used in programming mode.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.Edit)]
		public string Password
		{
			get;
			set;
		}

        /// <summary>
        /// If meter is using custom data format.
        /// </summary>
        [Browsable(true), ReadOnly(false), System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("If meter is using custom data format.")]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.Edit)]
        public bool CustomDataFormat
        {
            get;
            set;
        }


	}
}
