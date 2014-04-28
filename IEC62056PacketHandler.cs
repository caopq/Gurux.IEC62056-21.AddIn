using System;
using System.Collections.Generic;
using System.Text;
using Gurux.Device;
using System.Windows.Forms;
using Gurux.Device.Editor;
using Gurux.Communication;
using Gurux.Common;
using Gurux.IEC62056_21.AddIn.Parser;

namespace Gurux.IEC62056_21.AddIn
{
	class IEC62056PacketHandler : Gurux.Device.IGXPacketHandler
	{
        /// <summary>
        /// Last status.
        /// </summary>
        int Status = 0;

        /// <summary>
        /// Date time of table.
        /// </summary>
        DateTime Time;

        /// <summary>
        /// Add time in minutes.
        /// </summary>
        int Add = 0;

        string[] Columns;

        public object Parent
        {
            get;
            set;
        }

        /// <inheritdoc cref="IGXPacketHandler.Connect"/>
        public void Connect(object sender)
        {
        }

        /// <inheritdoc cref="IGXPacketHandler.Disconnect"/>
        public void Disconnect(object sender)
        {

        }
		
		public void ExecuteSendCommand(object sender, string command, GXPacket packet)
		{
			switch (command)
			{
				case "ReadTableData":
                    Status = 0;
                    Add = 0;
                    Columns = null;
                    GXIEC62056Table table = (GXIEC62056Table)sender;
                    table.ClearRows();
					ReadTableData(sender, packet);
					break;
				case "ReadTableDataNext":
					ReadTableDataNext(sender, packet);
					break;
                case "Connect":
                    GXIEC62056Device dev = GetDevice(sender);
                    Connect(dev.SerialNumber, dev.Password, dev.Mode, dev.GXClient.Media, dev.WaitTime);
					break;
                case "Disconnect":
                    Disconnect(GetDevice(sender), packet);
                    break;
                case "Readout":
					Readout(sender, packet);
					break;
				case "ReadData":
					ReadData(sender, packet);
					break;
                case "KeepAlive":
                    KeepAlive(sender as GXIEC62056Device, packet);
					break;                    
				default:
					throw new Exception("ExecuteCommand failed. Unknown command: " + command);
			}
		}

		public void ExecuteParseCommand(object sender, string command, GXPacket[] packets)
		{
			switch (command)
			{
				case "ReadTableDataReply":
					ReadTableDataReply(sender, packets);
					break;
				case "ReadDataReply":
					ReadDataReply(sender, packets);
					break;
                case "DisconnectReply":
                    DisconnectReply(sender, packets);
					break;
                case "KeepAliveReply":
                    KeepAliveReply(sender as GXIEC62056Device, packets);
					break;
				default:
					throw new Exception("ExecuteCommand failed. Unknown command: " + command);
			}
		}

		public bool IsTransactionComplete(object sender, string command, GXPacket packet)
		{
			switch (command)
			{
				case "IsTableRead":
				return IsTableRead(sender, packet);
				default:
					throw new Exception("IsTransactionComplete failed. Unknown command: " + command);
			}
		}

		public object UIValueToDeviceValue(GXProperty sender, object value)
		{
            if (sender.ValueType == typeof(DateTime))
			{
                return Parser.IEC62056Parser.DateTimeToString((DateTime)value, true);
			}
			return value;
		}

		public object DeviceValueToUIValue(GXProperty sender, object value)
		{
            DataType dt = (sender as GXIEC62056Property).DataType;
            if (!(value is DateTime))
            {                
                if (dt == DataType.DateTime)
                {
                    return Parser.IEC62056Parser.StringToDateTime(value.ToString());
                }
                else if (dt == DataType.Date)
                {
                    return Parser.IEC62056Parser.StringToDateTime(value.ToString()).ToShortDateString();
                }
                else if (dt == DataType.Time)
                {
                    return Parser.IEC62056Parser.StringToTime(value.ToString());
                }
            }
            if (dt == DataType.OctetString)
            {
                value = ASCIIEncoding.ASCII.GetString(GXCommon.HexToBytes(value.ToString(), false));
            }
            return value;
		}

        private void KeepAlive(GXIEC62056Device device, GXPacket packet)
        {
            if (device.AliveMode == 0)
            {
                packet.AppendData("R2");
                packet.AppendData((byte)2);
                packet.AppendData("0.0.0()");
            }
            else
            {
                packet.AppendData("R" + device.AliveMode.ToString());
                packet.AppendData((byte)2);
                packet.AppendData(device.AliveData + "(" + device.AliveParameters + ")");
            }
        }

        private void KeepAliveReply(GXIEC62056Device sender, GXPacket[] packets)
        {
            List<byte> fullData = new List<byte>();
            foreach (GXPacket packet in packets)
            {
                object tmp = null;
                tmp = packet.ExtractData(typeof(byte[]), 0, -1);
                if (tmp is byte[])
                {
                    fullData.AddRange(tmp as byte[]);
                }
            }

            string val = ASCIIEncoding.ASCII.GetString(fullData.ToArray());
            int start = val.IndexOf('(');
            int end = val.LastIndexOf(')');
            if (start != -1 && end != -1)
            {
                val = val.Substring(start + 1, end - start - 1);
            }
        }

		private void ReadData(object sender, GXPacket packet)
		{
			if (!(sender is GXIEC62056Property))
			{
				return;
			}
            GXIEC62056Property prop = sender as GXIEC62056Property;
            packet.AppendData("R" + prop.ReadMode.ToString());
            packet.AppendData((byte)2);
            packet.AppendData(prop.Data + "(" + prop.Parameters + ")");
		}

		private void ReadDataReply(object sender, GXPacket[] packets)
		{
			if (sender is GXIEC62056Property)
			{
				GXIEC62056Property prop = sender as GXIEC62056Property;
				List<byte> fullData = new List<byte>();
				foreach (GXPacket packet in packets)
				{
					object tmp = null;
					tmp = packet.ExtractData(typeof(byte[]), 0, -1);
					if (tmp is byte[])
					{
						fullData.AddRange(tmp as byte[]);
					}
				}

				string val = ASCIIEncoding.ASCII.GetString(fullData.ToArray());
                int start = val.IndexOf('(');
                int end = val.LastIndexOf(')');
                if (start != -1 && end != -1)
                {
                    val = val.Substring(start + 1, end - start - 1);
                }
                prop.ReadTime = DateTime.Now;
				prop.SetValue(val, false, PropertyStates.ValueChangedByDevice);                
			}
		}

		private void Readout(object sender, GXPacket packet)
		{
			GXDevice dev = null;
			if (sender is GXDevice)
			{
				dev = sender as GXDevice;
			}
			else if (sender is GXTable)
			{
				dev = (sender as GXTable).Device;
			}
			else if (sender is GXCategory)
			{
				dev = (sender as GXCategory).Device;
			}
			else if (sender is GXProperty)
			{
				dev = (sender as GXProperty).Device;
			}
			else
			{
				throw new Exception("Readout failed. Unknown sender: " + sender.ToString());
			}
            /* TODO: Readout
			List<byte> readoutBytes = Handshake(sender, packet, true);
			List<Parser.IECDataObject> items = Parser.IEC62056Parser.ParseReadout(readoutBytes);
            foreach (Parser.IECDataObject it in items)
            {
                if (!it.Address.Contains("*"))
                {
                    List<object> results = new List<object>();
                    dev.FindByPropertyValue("OBISCode", it.Address, results);
                    if (results.Count > 0)
                    {
                        GXProperty prop = results[0] as GXProperty;
                        prop.ReadTime = DateTime.Now;
                        prop.SetValue(it.Value, false, PropertyStates.ValueChangedByDevice);
                    }
                }
            }
             * */
		}

		private void ReadoutReply(object sender, GXPacket[] packets)
		{
			List<byte> fullData = new List<byte>();
			foreach (GXPacket packet in packets)
			{
				object tmp = null;
				tmp = packet.ExtractData(typeof(byte[]), 0, -1);
				if (tmp is byte[])
				{
					fullData.AddRange(tmp as byte[]);
				}
			}
		}

		private void ReadTableDataNext(object sender, GXPacket packet)
		{
			packet.Bop = null;
			packet.Eop = null;
			packet.ChecksumSettings.Type = ChecksumType.None;
			packet.AppendData((byte)0x06);
		}

        GXIEC62056Device GetDevice(object sender)
        {
            GXIEC62056Device dev = null;
            if (sender is GXIEC62056Device)
            {
                dev = sender as GXIEC62056Device;
            }
            else if (sender is GXProperty)
            {
                dev = (sender as GXProperty).Device as GXIEC62056Device;
            }
            else
            {
                throw new Exception("InitRead failed. Unknown sender: " + sender.ToString());
            }
            return dev;
        }

        private void Disconnect(object sender, GXPacket packet)
		{
			GXIEC62056Device dev = null;
			if (sender is GXIEC62056Device)
			{
				dev = sender as GXIEC62056Device;
			}
			else if (sender is GXProperty)
			{
				dev = (sender as GXProperty).Device as GXIEC62056Device;
			}
			else
			{
				throw new Exception("InitRead failed. Unknown sender: " + sender.ToString());
			}
			if (dev.Mode != Protocol.ModeA)
			{
                packet.ResendCount = -1;
                packet.Eop = null;
                packet.ChecksumSettings.Position = -3;
                packet.ChecksumSettings.Count = -3;
                packet.AppendData("B0");
                packet.AppendData((byte) 3);
			}
		}

        private void DisconnectReply(object sender, GXPacket[] packets)
		{
            string header, data;
            List<byte> buff = new List<byte>(packets[0].ExtractPacket());            
            IEC62056Parser.GetPacket(buff, true, out header, out data);        
        }
        

        private void Connect(string serialNumber, string password, Protocol protocol, IGXMedia media, int waittime)
		{
            string data = "/?" + serialNumber + "!\r\n";
            byte[] reply = IEC62056Parser.Identify(media, data, '\0', waittime);            
            System.Diagnostics.Debug.WriteLine(ASCIIEncoding.ASCII.GetString(reply));
            if (reply[0] != '/')
            {
                throw new Exception("Invalid reply.");
            }
            char baudRate = (char)reply[4];
            string CModeBauds = "0123456789";
            string BModeBauds = "ABCDEFGHI";
            Protocol mode;
            if (CModeBauds.IndexOf(baudRate) != -1)
            {
                mode = Protocol.ModeC;
            }
            else if (BModeBauds.IndexOf(baudRate) != -1)
            {
                mode = Protocol.ModeB;
            }
            else
            {
                mode = Protocol.ModeA;
                baudRate = '0';
            }
            if (reply[0] != '/')
            {
                throw new Exception("Import failed. Invalid reply.");
            }            
            string manufacturer = new string(new char[] { (char)reply[1], (char)reply[2], (char)reply[3] });
            if (protocol == Protocol.ModeA)
            {
                data = (char)0x06 + "0" + baudRate + "0\r\n";
            }
            else
            {
                data = (char)0x06 + "0" + baudRate + "1\r\n";
            }            
            //Note this sleep is in standard. Do not remove.
            System.Threading.Thread.Sleep(300);
            reply = IEC62056Parser.ParseHandshake(media, data, baudRate, waittime);
            string header, frame;            
            IEC62056Parser.GetPacket(new List<byte>(reply), true, out header, out frame);
            System.Diagnostics.Debug.WriteLine(frame);
            if (header == "B0")
            {
                throw new Exception("Connection failed. Meter do not accept connection at the moment.");
            }
            //Password is asked and it is given.
            if (header == "P0" && !string.IsNullOrEmpty(password))
            {
                System.Diagnostics.Debug.WriteLine("Password is asked.");
                //If Elster A1700
                if (manufacturer.Equals("GEC"))
                {
                    password = ElsterEncrypt(password, frame);
                }
                data = (char)1 + "P2" + (char)2 + "(" + password + ")" + (char)3;
                List<byte> tmp = new List<byte>(ASCIIEncoding.ASCII.GetBytes(data));
                data = data + (char) IEC62056Parser.CalculateChecksum(tmp, 1, -1);
                reply = IEC62056Parser.Read(media, data, baudRate, waittime, true);
                if (reply == null || (reply.Length != 1 && reply[0] != 6))
                {
                    throw new Exception("Connection failed. Invalid password.");
                }
            }
		}

        /// <summary>
        /// Encrypt Elster PW for A1700.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        string ElsterEncrypt(string password, string seed)
        {
            //Convert hex string to byte array.
            byte[] s = GXCommon.HexToBytes(seed, false);
            byte[] crypted = new byte[8];
            for (int pos = 0; pos != 8; ++pos)
            {
                crypted[pos] = (byte)(password[pos] ^ s[pos]);
            }
            int last = crypted[7];
            for (int pos = 0; pos != 8; ++pos)
            {
                long tmp = (crypted[pos] + last) & 0x800000FF;
                crypted[pos] = (byte)((crypted[pos] + last) & 0xFF);
                last = crypted[pos];
            }
            return GXCommon.ToHex(crypted, false);
        }

		private void ReadTableData(object sender, GXPacket packet)
		{
            GXIEC62056Table table = sender as GXIEC62056Table;		            
			DateTime start, end;
            (sender as IGXPartialRead).GetStartEndTime(out start, out end);
            packet.AppendData(IEC62056Parser.GenerateReadTable(table.Data, start, end, table.Parameters, table.ReadMode, table.ReadCount));
		}
        		
		private void ReadTableDataReply(object sender, GXPacket[] packets)
		{
            if ((sender as GXIEC62056Table).ReadMode != 6)
            {
                GXIEC62056Table table = (GXIEC62056Table)sender;
                object[][] rows = Parser.IEC62056Parser.ParseTableData(packets[packets.Length - 1].ExtractPacket(), ref Columns, ref Status, ref Time, ref Add, table.Data);
                table.AddRows(table.RowCount, new List<object[]>(rows), false);
            }
		}

		private bool IsTableRead(object sender, GXPacket packet)
		{
			if ((sender as GXIEC62056Table).ReadMode == 6)
			{
                GXIEC62056Table table = (GXIEC62056Table)sender;
                object[][] rows = Parser.IEC62056Parser.ParseTableData(packet.ExtractPacket(), ref Columns, ref Status, ref Time, ref Add, table.Data);
                System.Diagnostics.Debug.WriteLine("Mikko " + rows.Length);
                table.AddRows(table.RowCount, new List<object[]>(rows), false);                
            }            
            return (byte) packet.Eop != 0x4;
		}
	}
}
