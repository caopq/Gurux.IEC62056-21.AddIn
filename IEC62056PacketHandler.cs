using System;
using System.Collections.Generic;
using System.Text;
using Gurux.Device;
using System.Windows.Forms;
using Gurux.Device.Editor;
using Gurux.Communication;
using Gurux.Common;

namespace Gurux.IEC62056_21.AddIn
{
	class IEC62056PacketHandler : Gurux.Device.IGXPacketHandler
	{
        [System.Runtime.InteropServices.DllImport("Encrypt.dll")]
        public static extern int ENCRYPT(byte[] szEncryptPwd, byte[] szPassword, byte[] szKey);

		int m_parseCount = 0;
		bool m_iskra = false;

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
					m_parseCount = 0;
					ReadTableData(sender, packet);
					break;
				case "ReadTableDataNext":
					ReadTableDataNext(sender, packet);
					break;
				case "InitRead":
					InitRead(sender, packet);
					break;
                case "Disconnect":
                    Disconnect(sender, packet);
                    break;
                case "Readout":
					Readout(sender, packet);
					break;
				case "ReadData":
					ReadData(sender, packet);
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
				return Parser.IEC62056Parser.DateTimeToString((DateTime)value, m_iskra);
			}
			return value;
		}

		public object DeviceValueToUIValue(GXProperty sender, object value)
		{
            if (sender.ValueType == typeof(DateTime))
			{
				return Parser.IEC62056Parser.StringToDateTime(value.ToString());
			}
			return value;
		}

		private void ReadData(object sender, GXPacket packet)
		{
			if (!(sender is GXIEC62056Property))
			{
				return;
			}
			GXIEC62056Property prop = sender as GXIEC62056Property;
			packet.AppendData("R" + prop.ReadMode);
			packet.AppendData((byte)2);            
            packet.AppendData(prop.Address);
			packet.AppendData("()");
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
			if (sender is GXIEC62056Table && (sender as GXIEC62056Table).ReadMode == "6")
			{
				packet.Bop = null;
				packet.Eop = null;
				packet.ChecksumSettings.Type = ChecksumType.None;
				packet.AppendData((byte)0x06);
			}
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
			if (dev.ProtocolMode != Protocol.ModeA)
			{
                packet.AppendData(ASCIIEncoding.ASCII.GetBytes("B0"));
			}
		}

		private void InitRead(object sender, GXPacket packet)
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
			if (dev.ProtocolMode != Protocol.ModeA)
			{
				Handshake(sender, packet, false);
			}
		}

		void GetStartEndTime(GXIEC62056Table table, out string starttm, out string endtm)
        {
			starttm = string.Empty;
			endtm = string.Empty;
			switch (table.Type)
            {
                case Gurux.Device.Editor.PartialReadType.New:
                    if (table.Start != null)
                    {
						starttm = Parser.IEC62056Parser.DateTimeToString((DateTime)table.Start, false);
                    }
                    break;
                case Gurux.Device.Editor.PartialReadType.All:
					//both empty
                    break;
                case Gurux.Device.Editor.PartialReadType.Last:
					starttm = Parser.IEC62056Parser.DateTimeToString((DateTime)table.Start, false);
                    break;
                case Gurux.Device.Editor.PartialReadType.Range:
					if (table.Start != null)
					{
						starttm = Parser.IEC62056Parser.DateTimeToString((DateTime)table.Start, false);
					}
					else
					{
						starttm = string.Empty;
					}
					if (table.End != null)
					{
						endtm = Parser.IEC62056Parser.DateTimeToString((DateTime)table.End, false);
					}
					else
					{
						endtm = string.Empty;
					}
                    break;
                default:
                    throw new Exception("Invalid start type.");
            }
        }

		private void ReadTableData(object sender, GXPacket packet)
		{
			if (!(sender is GXIEC62056Table))
			{
				return;
			}
			GXIEC62056Table table = sender as GXIEC62056Table;
			//Handshake(table, packet, false);

			string startTime = string.Empty;
			string endTime = string.Empty;
			GetStartEndTime(table, out startTime, out endTime);
			string tableAddress = null;
			if (table.Address != null)
			{
				tableAddress = table.Address;
			}
			packet.AppendData("R" + table.ReadMode);
			packet.AppendData((byte)2);

			packet.AppendData(tableAddress);
			packet.AppendData("(");
			packet.AppendData(startTime + ";" + endTime);
			//packet.AppendData(";");
			if (table.ReadMode == "6")
			{
				packet.AppendData(";" + 1);
			}
			packet.AppendData(")");
		}


        static internal byte[] GetData(Gurux.Common.IGXMedia media, int wt, string data, List<byte> reply, char baudRate)
        {            
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = media.Eop,
                WaitTime = wt
            };
            //Read byte at time if EOP is not set.
            if (media.Eop == null)
            {
                p.Count = 1;
            }
            if (data != null)
            {
                media.Send(data, null);
                if (baudRate != '\0' && media is Gurux.Serial.GXSerial)
                {
                    Gurux.Serial.GXSerial serial = media as Gurux.Serial.GXSerial;
                    while (serial.BytesToWrite != 0)
                    {
                        System.Threading.Thread.Sleep(100);
                    }                    
                    switch (baudRate)
                    {
                        case '0':
                            serial.BaudRate = 300;
                            break;
                        case '1':
                        case 'A':
                            serial.BaudRate = 600;
                            break;
                        case '2':
                        case 'B':
                            serial.BaudRate = 1200;
                            break;
                        case '3':
                        case 'C':
                            serial.BaudRate = 2400;
                            break;
                        case '4':
                        case 'D':
                            serial.BaudRate = 4800;
                            break;
                        case '5':
                        case 'E':
                            serial.BaudRate = 9600;
                            break;
                        case '6':
                        case 'F':
                            serial.BaudRate = 19200;
                            break;
                        default:
                            throw new Exception("Unknown baud rate.");
                    }
                }
            }
            if (!media.Receive(p))
            {
                throw new Exception("Failed to receive reply from the device in given time.");
            }
            if (!(p.Reply.Length != 0 && p.Reply[0] == 3))
            {
                //Remove echo.
                if (data == ASCIIEncoding.ASCII.GetString(p.Reply))
                {
                    p.Reply = null;
                    if (!media.Receive(p))
                    {
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                }                
            }
            reply.AddRange(p.Reply);
            return p.Reply;
        }

		/// <summary>
		/// Returns readout or handshake reply.
		/// </summary>
		/// <returns></returns>
		private List<byte> Handshake(object sender, GXPacket packet, bool readout)
		{
			GXIEC62056Device dev = null;
			if (sender is GXIEC62056Device)
			{
				dev = sender as GXIEC62056Device;
			}
			else if (sender is GXTable)
			{
				dev = (GXIEC62056Device)(sender as GXTable).Device;
			}
			else if (sender is GXCategory)
			{
				dev = (GXIEC62056Device)(sender as GXCategory).Device;
			}
			else if (sender is GXProperty)
			{
				dev = (GXIEC62056Device)(sender as GXProperty).Device;
			}
			IGXMedia media = (IGXMedia)dev.GXClient.Media;
            //EOP = 4 when data block is read.
            //EOP = 3 when all data is read.
            //EOP = 0xA when row is read.
            //EOP = 0x15 when there is an error.
            //EOP = 0x6 ACK.
            media.Eop = new byte[] { 0xA, 3, 4, 6, 0x15 };
			string deviceSerialNumber = Convert.ToString(dev.SerialNumber);
            List<byte> reply = new List<byte>();
            int count = 15;
            int pos = 1;
            lock (media.Synchronous)
            {
                dev.NotifyTransactionProgress(this, new Gurux.Device.GXTransactionProgressEventArgs(dev, pos, count, Gurux.Device.DeviceStates.ReadStart));            
                IEC62056PacketHandler.GetData(media, dev.WaitTime, "/?" + deviceSerialNumber + "!\r\n", reply, '\0');
                char baudRate = (char)reply[4];
                if (!readout && dev.ProtocolMode != Protocol.ModeA)
                {                    
                    string data = (char)0x06 + "0" + baudRate;
                    if (dev.ProtocolMode == Protocol.ModeC)
                    {
                        data += "1\r\n";
                    }
                    else //Set correct mode and baud rate 
                    {
                        data += "0\r\n";
                        //if readout mode.
                        if (dev.ProtocolMode == Protocol.ModeA)
                        {
                            baudRate = '0';
                        }
                    }
                    reply.Clear();
                    dev.NotifyTransactionProgress(this, new Gurux.Device.GXTransactionProgressEventArgs(dev, ++pos, count, Gurux.Device.DeviceStates.ReadStart));
                    IEC62056PacketHandler.GetData(media, dev.WaitTime, data, reply, baudRate);                    
                }
                if (dev.ProtocolMode == Protocol.ModeA)
                {
                    reply.Clear();
                    //Read until EOP.
                    do
                    {
                        dev.NotifyTransactionProgress(this, new Gurux.Device.GXTransactionProgressEventArgs(dev, ++pos, count, Gurux.Device.DeviceStates.ReadStart));
                        IEC62056PacketHandler.GetData(media, dev.WaitTime, null, reply, baudRate);
                    }
                    while (reply[reply.Count - 1] != 3);
                    media.Eop = null;
                    IEC62056PacketHandler.GetData(media, dev.WaitTime, null, reply, baudRate);
                    if (reply[0] == 0x1 || reply[0] == 0x2)//Remove Bop if exista.
                    {
                        reply.RemoveAt(0);
                    }
                    byte crc = reply[reply.Count - 1];
                    if (crc != 3)
                    {
                        //Remove crc.
                        reply.RemoveAt(reply.Count - 1);
                        byte checksum = Parser.IEC62056Parser.CalculateChecksum(reply.ToArray());
                        if (checksum != crc)
                        {
                            throw new Exception("Connection failed. Data is corrupted.");
                        }
                    }
                }
                //Elster spesific behavior.
                else if (dev.Manufacturer == Manufacturer.Elster &&
                    !string.IsNullOrEmpty(dev.Password) && reply[0] == 1)
                {
                    // If password is used.
                    // Get reply. Skip BOP and EOP.
                    int start = reply.IndexOf((byte) '(');
                    int end = reply.LastIndexOf((byte) ')');
                    byte[] seed = reply.GetRange(start + 1, end - start - 1).ToArray();
                    reply.Clear();
                    //Sign onto meter with Level 0 password
                    string data = "P2" + (char)2 + "(0000000000000000)" + (char)3;
                    byte checksum = Parser.IEC62056Parser.CalculateChecksum(ASCIIEncoding.ASCII.GetBytes(data));
                    data = (char)1 + data + (char)checksum;
                    IEC62056PacketHandler.GetData(media, dev.WaitTime, data, reply, baudRate);
                    if (reply[0] == 6)
                    {
                        byte[] pw2 = new byte[17];
                        byte[] pw = ASCIIEncoding.ASCII.GetBytes(dev.Password);
                        int ret = ENCRYPT(pw2, seed, pw);
                        data = "P2" + (char)2 + "(" + ASCIIEncoding.ASCII.GetString(pw2, 0, 16) + ")" + (char)3;
                        checksum = Parser.IEC62056Parser.CalculateChecksum(ASCIIEncoding.ASCII.GetBytes(data));
                        data = (char)1 + data + (char)checksum;
                        IEC62056PacketHandler.GetData(media, dev.WaitTime, data, reply, baudRate);                        
                        if (reply[0] != 6)
                        {
                            throw new Exception("Failed to set password.");
                        }
                    }
                    else
                    {
                        throw new Exception("Failed to set password.");
                    }
                }
            }
            dev.NotifyTransactionProgress(this, new Gurux.Device.GXTransactionProgressEventArgs(dev, 1, 1, Gurux.Device.DeviceStates.ReadEnd));
            return reply;
		}

		private void ReadTableDataReply(object sender, GXPacket[] packets)
		{
			GXIEC62056Table table = (GXIEC62056Table)sender;
			if (table.ReadMode == "5")
			{
				HandleTableReadReplyPacket(table, packets);
			}
		}

		private bool IsTableRead(object sender, GXPacket packet)
		{
			GXIEC62056Table table = (GXIEC62056Table)sender;
			if (table.ReadMode == "5")
			{
				return true;
			}
			else
			{
				object eop = packet.Eop;
				if (packet.Eop == null)
				{
					eop = (byte)0;
				}
				else
				{
					eop = Convert.ToByte(packet.Eop);
				}
				bool retval = true;
				if ((byte)eop == (byte)0x04)
				{
					retval = false;
				}
				HandleTableReadReplyPacket(table, new GXPacket[] { packet });
				return retval;
			}
		}

		private void HandleTableReadReplyPacket(GXIEC62056Table table, GXPacket[] packets)
		{
			DateTime latestStamp = new DateTime(2000, 1, 1);

			object tmp = null;
			List<byte> fullData = new List<byte>();

			foreach (GXPacket pack in packets)
			{
				tmp = pack.ExtractData(typeof(byte[]), 0, -1);
				fullData.AddRange((byte[])tmp);
			}
			if (fullData.Count == 1 && fullData[0] == 0x15)
			{
				throw new Exception("Error returned from Device: NAK, negative acknowledge");
			}
			if (m_parseCount == 0) //readmode == 5 or readmode == 6 && previousTableCount == 0
			{
				table.ClearRows();
			}
			++m_parseCount;
			List<Parser.IECDataObject> dataObjects = Parser.IEC62056Parser.ParseTable(table.Address, fullData, m_iskra);

			List<object[]> rows = new List<object[]>();
			for (int rowIndex = 0; rowIndex < dataObjects[0].RowValues.Count; ++rowIndex)
			{
				List<object> row = new List<object>();
				for (int dataCnt = 0; dataCnt < dataObjects.Count; ++dataCnt)
				{
					if (dataCnt == 0)
					{
						DateTime tmpTime = Parser.IEC62056Parser.StringToDateTime(dataObjects[dataCnt].RowValues[rowIndex]);
						if (tmpTime > latestStamp)
						{
							latestStamp = tmpTime;
						}
					}

					row.Add(dataObjects[dataCnt].RowValues[rowIndex]);
				}
				rows.Add(row.ToArray());
			}
			table.AddRows(table.RowCount, rows, false);

            if (table.Type == PartialReadType.New)
            {
				if (table.Start == null || ((DateTime)table.Start < latestStamp))
				{
					table.Start = latestStamp.AddMinutes(1);
				}
            }

			/*int oldRowCount = m_rowCount;
			for (int dataCnt = 0; dataCnt < dataObjects.Count; ++dataCnt)
			{
				Parser.IECDataObject dataObj = dataObjects[dataCnt];

				

				GXProperty prop = null;

				List<object> findResults = new List<object>();
				table.FindByPropertyValue("OBISCode", dataObj.Address, findResults);

				if (findResults.Count > 0)
				{
					prop = (GXIEC62056TableProperty)findResults[0];

					for (int i = 0; i < dataObj.RowValues.Count; ++i)
					{
						if (prop.ValueType == typeof(DateTime) && dataCnt == 0)
						{
							string dateStr = dataObj.RowValues[i];

							DateTime tmpTime = Parser.IEC62056Parser.StringToDateTime(dateStr, m_iskra);
							if (tmpTime > latestStamp)
							{
								latestStamp = tmpTime;
							}
						}
						prop.Rows[i + oldRowCount].SetValue(dataObj.RowValues[i], VariantType.String, false, PropertyStates.ValueChangedByDevice);
						if (dataCnt == 0)
						{
							++m_rowCount;
						}
					}
				}
			}*/			
		}
	}
}
