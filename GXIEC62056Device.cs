using System;
using Gurux.Device.Editor;
using System.ComponentModel;
using System.Runtime.Serialization;
using Gurux.Device;

namespace Gurux.IEC62056_21.AddIn
{
	/// <summary>
	/// Extends Gurux.Device.GXDevice class with the IEC specific properties.
	/// </summary>
	[DataContract()]
    [GXInitialActionMessage(InitialActionType.KeepAlive, "KeepAlive", "KeepAliveReply")]
	[GXInitialActionMessage(InitialActionType.Connected, "Connect", Index = 1)]
    [GXInitialActionMessage(InitialActionType.Disconnecting, "Disconnect", "DisconnectReply", Index = 1)]
	public class GXIEC62056Device : Gurux.Device.GXDevice
	{
		/// <summary>
		/// Initializes a new instance of the GXIECDevice class.
		/// </summary>
		public GXIEC62056Device()
		{
            this.Keepalive.Ignore = KeepaliveFieldsIgnored.Reset;
            MaximumBaudRate = 0;
            ReadMode = WriteMode = 2;
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
			media.Name = "Net";
            media.DefaultMediaSettings = "<Port>1000</Port>";
			this.AllowedMediaTypes.Add(media);
			media = new GXMediaType();
			media.Name = "Serial";
            media.DefaultMediaSettings = "<Bps>300</Bps><StopBits>1</StopBits><Parity>2</Parity><ByteSize>7</ByteSize>";
			this.AllowedMediaTypes.Add(media);

            media = new GXMediaType();
            media.Name = "Terminal";
            media.DefaultMediaSettings = "<Bps>9600</Bps>";
            this.AllowedMediaTypes.Add(media);
		}

        public override bool ImportFromDeviceEnabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// TransactionDelay is the minimum transaction delay time, in milliseconds, between transactions.
        /// </summary>
        [System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("TransactionDelay is the minimum transaction delay time, in milliseconds, between transactions."),
        DefaultValue(0)]
        [GXUserLevelAttribute(UserLevelType.Experienced)]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public override int TransactionDelay
        {
            get
            {
                return base.TransactionDelay;
            }
            set
            {
                base.TransactionDelay = value;
            }
        }

        /// <summary>
        /// Used Mode.
        /// </summary>
        [DefaultValue(Protocol.None), Browsable(true), ReadOnly(false), System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("Used Mode.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        public Protocol Mode
		{
			get;
			set;
		}

        /// <summary>
        /// Read command type.
        /// </summary>
        [Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("Read command type.")]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        public int ReadMode
        {
            get;
            set;
        }

        /// <summary>
        /// Write command type.
        /// </summary>
        [Browsable(true), ReadOnly(false), System.ComponentModel.Category("Data"), System.ComponentModel.Description("Write command type.")]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        public int WriteMode
        {
            get;
            set;
        }

        /// <summary>
        /// Device serial number.
        /// </summary>
		[Browsable(false), ReadOnly(false), 
        System.ComponentModel.Category("Behavior"), 
        System.ComponentModel.Description("Device serial number.")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.None, ValueAccessType.Edit)]
		public string SerialNumber
        {
			get;
			set;
        }	

        /// <summary>
        /// Meter password.
        /// </summary>
		[Browsable(true), ReadOnly(false), System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("Meter password")]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[ValueAccess(ValueAccessType.Edit, ValueAccessType.Edit)]
		public string Password
		{
			get;
			set;
		}

        /// <summary>
        /// Mamimum baud rate used. Zero if ignored.
        /// </summary>
        [Browsable(true), ReadOnly(false), System.ComponentModel.Category("Behavior"), System.ComponentModel.Description("Mamimum baud rate used. Zero if ignored.")]
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        [GXUserLevelAttribute(UserLevelType.Experienced)]
        [ValueAccess(ValueAccessType.Edit, ValueAccessType.None)]
        public int MaximumBaudRate
        {
            get;
            set;
        }       

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        internal int AliveMode
        {
            get;
            set;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        internal int AliveData
        {
            get;
            set;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        internal int AliveParameters
        {
            get;
            set;
        }
	}
}
