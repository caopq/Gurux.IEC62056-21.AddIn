using System;
using System.Collections.Generic;
using System.Text;
using Gurux.Device;
using Gurux.Communication;
using System.Windows.Forms;
using Gurux.Net;
using Gurux.Serial;

namespace Gurux.IEC62056_21.AddIn
{
	class IEC62056PacketParser : Gurux.Communication.IGXPacketParser
	{

		public void IsReplyPacket(object sender, GXReplyPacketEventArgs e)
		{
			object receivedData = null, sentData = null;
			receivedData = e.Received.ExtractData(typeof(string), 0, -1);
			sentData = e.Send.ExtractData(typeof(string), 0, -1);
			if (receivedData != null && sentData.ToString() == receivedData.ToString())
			{
				e.Accept = false;
			}
			else
			{
				e.Accept = true;
			}
		}

		public void Load(object sender)
		{
			((GXClient)sender).ParseReceivedPacket = true;
		}

        /// <inheritdoc cref="IGXPacketParser.Connect"/>
        public void Connect(object sender)
        {
        }

        /// <inheritdoc cref="IGXPacketParser.Disconnect"/>
        public void Disconnect(object sender)
        {

        }

		public void ParsePacketFromData(object sender, GXParsePacketEventArgs e)
		{
			e.Packet.Status = PacketStates.TransactionTimeReset;
			int bopPos = -1, eopPos = -1;
			byte bop = 0, eop = 0;
			for (int i = 0; i < e.Data.Length; ++i)
			{
				byte tmp = e.Data[i];
				if (tmp == 0x01 || tmp == 0x02 || tmp == 0x05 || tmp == 0x06 || tmp == 0x15)
				{
					bop = tmp;
					bopPos = i;
					break;
				}
			}
			if (bopPos == -1)
			{
				e.PacketSize = 0;
			}
			else if (bop == 0x06 || bop == 0x15)
			{
				e.Packet.InsertData(bop, -1, 0, 0);
				e.PacketSize = 1;
			}
			else if (e.Data.Length > bopPos + 2 && (bop == 0x01 || bop == 0x02))
			{
				for (int i = bopPos + 1; i < e.Data.Length; ++i)
				{
					byte tmp = e.Data[i];
					if (tmp == 0x03 || tmp == 0x04)
					{
						eop = tmp;
						eopPos = i;

						//if crc is not present
						if (e.Data.Length < i + 2)
						{
							e.PacketSize = 0;
							return;
						}
						byte replyChecksum = e.Data[i + 1];
						e.Packet.Bop = bop;
						e.Packet.Eop = eop;
						//for (int j = bopPos + 1; j < eopPos - 1; ++j )
						for (int j = bopPos + 1; j < eopPos; ++j)
						{
							e.Packet.InsertData(e.Data[j], -1, 0, 0);
						}
						byte checksum = (byte)e.Packet.CountChecksum(1, -1);
						if (checksum != replyChecksum && replyChecksum != 0x00) //iskra = 0x00
						{
							e.Packet.ClearData();
							e.PacketSize = 0;
							return;
						}
						e.PacketSize = eopPos + 2;
						return;
					}
				}
			}
			else
			{
				e.PacketSize = 0;
			}
		}

		#region NotImplemented

		public void ReceiveData(object sender, GXReceiveDataEventArgs e)
		{
			string str = System.Text.ASCIIEncoding.ASCII.GetString(e.Data);
			if (str == "<ALIVE>")
			{
				e.Accept = false;
			}
			else
			{
				e.Accept = true;
			}
		}

		public void BeforeSend(object sender, GXPacket packet)
		{
		}

		public void AcceptNotify(object sender, GXReplyPacketEventArgs e)
		{
		}

		public void CountChecksum(object sender, GXChecksumEventArgs e)
		{
		}

		public void Received(object sender, GXReceivedPacketEventArgs e)
		{
		}

		public void Unload(object sender)
		{
		}

		public void InitializeMedia(object sender, Gurux.Common.IGXMedia media)
		{
            if (media is GXSerial)
            {
                GXSerial serial = media as GXSerial;
                serial.ConfigurableSettings = Gurux.Serial.AvailableMediaSettings.All;
                serial.BaudRate = 300;
                serial.DataBits = 7;
                serial.Parity = System.IO.Ports.Parity.Even;
                serial.StopBits = System.IO.Ports.StopBits.One;
            }
            else if (media is GXNet)
            {
                GXNet net = media as GXNet;
                net.Protocol = NetworkType.Tcp;
                net.Port = 1000;
                net.ConfigurableSettings = Gurux.Net.AvailableMediaSettings.Port | Gurux.Net.AvailableMediaSettings.Host;
            }
		}

		public void VerifyPacket(object sender, GXVerifyPacketEventArgs e)
		{
			//throw new NotImplementedException();
		}

		#endregion
	}
}
