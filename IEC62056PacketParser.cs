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
        /// <summary>
        /// Check is reply data echo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		public void IsReplyPacket(object sender, GXReplyPacketEventArgs e)
		{
            //If abort.
            if (Convert.ToString(e.Received.ExtractData(typeof(string), 0, -1)) == "B0")
            {
                e.Accept = true;
            }
            else
            {
                //Skip echo.
                e.Accept = !Gurux.IEC62056_21.AddIn.Parser.IEC62056Parser.EqualBytes(
                    e.Send.ExtractPacket(), e.Received.ExtractPacket());               
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
            string header, data;
            byte[] reply = Parser.IEC62056Parser.GetPacket(new List<byte>(e.Data), false, out header, out data);            
            if (reply != null)
            {
                List<byte> tmp = new List<byte>(reply);
                e.Packet.Bop = tmp[0];
                if (tmp.Count == 1 && tmp[0] == 6)
                {
                    e.Packet.Eop = null;
                    e.Packet.ChecksumSettings.Type = ChecksumType.None;
                    e.PacketSize = reply.Length;
                }
                else
                {
                    if (e.Packet.ChecksumSettings.Type == ChecksumType.None)
                    {
                        e.Packet.ChecksumSettings.Type = Gurux.Communication.ChecksumType.Custom;
                        e.Packet.ChecksumSettings.Count = -1;
                        e.Packet.ChecksumSettings.Position = -1;                        
                        e.Packet.ChecksumSettings.Size = 8;
                        e.Packet.ChecksumSettings.Polynomial = 0xA001;
                        e.Packet.ChecksumSettings.InitialValue = 0;
                        e.Packet.ChecksumSettings.FinalXOR = 0;
                        e.Packet.ChecksumSettings.ReverseData = false;
                        e.Packet.ChecksumSettings.Reflection = false;
                        e.Packet.ChecksumSettings.Start = 1;
                    }
                    e.Packet.Eop = (byte)tmp[tmp.Count - 2];
                    //Remove BOP.
                    tmp.RemoveAt(0);
                    //Remove EOP and CRC.
                    tmp.RemoveRange(tmp.Count - 2, 2);
                    e.Packet.AppendData(tmp.ToArray());
                    e.PacketSize = reply.Length;
                }
            }
            else
            {
                e.PacketSize = 0;
            }
		}

		#region NotImplemented

        /// <summary>
        /// Skip alive messages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		public void ReceiveData(object sender, GXReceiveDataEventArgs e)
		{
			string str = System.Text.ASCIIEncoding.ASCII.GetString(e.Data);
			e.Accept = str != "<ALIVE>";
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
                /*
                serial.BaudRate = 300;
                serial.DataBits = 7;
                serial.Parity = System.IO.Ports.Parity.Even;
                serial.StopBits = System.IO.Ports.StopBits.One;
                 * */
            }
            else if (media is GXNet)
            {
                GXNet net = media as GXNet;
                /*
                net.Protocol = NetworkType.Tcp;
                net.Port = 1000;
                 * */
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
