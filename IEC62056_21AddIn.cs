using System;
using Gurux.Device.Editor;
using Gurux.Device;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Gurux.Common;
using Gurux.Communication;
using System.Xml;
using System.Security.Cryptography;
using System.Linq;
using System.Runtime.InteropServices;

// Old name: IEC1107
namespace Gurux.IEC62056_21.AddIn
{        
	/// <summary>
	/// Implements IEC protocol addin component.
	/// </summary>
	[GXCommunicationAttribute(typeof(IEC62056PacketHandler), typeof(IEC62056PacketParser))]
	internal partial class IEC62056_21 : GXProtocolAddIn
	{
		/// <summary>
		/// Initializes a new instance of the IEC62056_21 class.
		/// </summary>
		public IEC62056_21()
			: base("IEC 62056-21", false, true, false)
		{   
            base.WizardAvailable = VisibilityItems.None;             
        }

		public override VisibilityItems ItemVisibility
		{
			get
			{
				return VisibilityItems.Categories | VisibilityItems.Tables;
			}
		}

		public override Functionalities GetFunctionalities(object target)
		{
			if (target is GXCategoryCollection)
			{
				return Functionalities.Add;
			}
			else if (target is GXTableCollection)
			{
				return Functionalities.None;
			}
			else if (target is GXCategory)
			{
				return Functionalities.Add;
			}
			else
			{
				return Functionalities.Remove | Functionalities.Edit;
			}
		}

		public override Type[] GetPropertyTypes(object parent)
		{
			if (parent == null)
			{
				return new Type[] { typeof(GXIEC62056Property), typeof(GXIEC62056TableProperty), typeof(GXIEC62056ReadoutProperty) };
			}
			if (parent is GXIEC62056ReadoutCategory)
			{
				return new Type[] { typeof(GXIEC62056ReadoutProperty) };
			}
			else if (parent is GXIEC62056Table)
			{
				return new Type[] { typeof(GXIEC62056TableProperty) };
			}
			else
			{
				return new Type[] { typeof(GXIEC62056Property) };
			}
		}

		public override Type[] GetTableTypes(object parent)
		{
			return new Type[] { typeof(GXIEC62056Table) };
		}

		public override Type GetDeviceType()
		{
			return typeof(GXIEC62056Device);
		}

		public override Type[] GetCategoryTypes(object parent)
		{
			if (parent is GXDevice)
			{
				GXIEC62056Device dev = parent as GXIEC62056Device;
				if (dev.ProtocolMode == Protocol.ModeA)
				{
                    return new Type[] { typeof(GXIEC62056ReadoutCategory) };					
				}
				else
				{
                    return new Type[] { typeof(GXIEC62056Category) };
				}
			}
			else 
			{
				return new Type[] { typeof(GXIEC62056ReadoutCategory), typeof(GXIEC62056Category) };
			}
		}

		public override Type[] GetExtraTypes(object parent)
		{
			return new Type[] { };
		}

        public override void ImportFromDevice(Control[] addinPages, GXDevice device, Gurux.Common.IGXMedia media)
        {
            GXIEC62056Device dev = (GXIEC62056Device)device;
            try
            {
                ImportSelectionDlg dlg = addinPages[1] as ImportSelectionDlg;
                string deviceSerialNumber = dlg.DeviceSerialNumber;
                //EOP = 4 when data block is read.
                //EOP = 3 when all data is read.
                //EOP = 0x10 when row is read.
                //EOP = 0x15 when there is an error.
                media.Eop = new byte[] { 0xA, 3, 4, 0x15 };
                media.Open();                
                //Some meters need this, do not remove.
                System.Threading.Thread.Sleep(500);
                List<byte> reply = new List<byte>();
                lock (media.Synchronous)
                {
                    int pos = 1;
                    Progress(pos, 10);
                    TraceLine("Handshake");
                    IEC62056PacketHandler.GetData(media, device.WaitTime, "/?" + deviceSerialNumber + "!\r\n", reply, '\0');
                    char baudRate = (char)reply[4];
                    string CModeBauds = "0123456789";
                    string BModeBauds = "ABCDEFGHI";
                    char mode;
                    if (CModeBauds.Contains(baudRate.ToString()))
                    {
                        dev.ProtocolMode = Protocol.ModeC;
                        mode = 'C';
                    }
                    else if (BModeBauds.Contains(baudRate.ToString()))
                    {
                        dev.ProtocolMode = Protocol.ModeB;
                        mode = 'B';
                    }
                    else
                    {
                        dev.ProtocolMode = Protocol.ModeA;
                        mode = 'A';
                        if (baudRate == ' ')
                        {
                            baudRate = '0';
                        }
                    }

                    string data = ASCIIEncoding.ASCII.GetString(reply.ToArray());
                    TraceLine("Found IEC device from Manufacturer: " + data.Substring(1, 3));
                    TraceLine("Device model is: " + data.Substring(6).Trim());
                    TraceLine("Device is supporting mode : " + mode);
                    reply.Clear();
                    if (dev.ProtocolMode != Protocol.ModeA)
                    {
                        ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
                        {
                            Count = 1,
                            WaitTime = 500
                        };
                        data = (char)0x06 + "0" + baudRate + "1\r\n";
                        IEC62056PacketHandler.GetData(media, device.WaitTime, data, reply, '\0');
                        //Read crc if exists.
                        media.Receive(p);
                        //Try to read tables.
                        //Read Load Profile.
                        ReadTableColumns(device, media, "P.01", false);
                        //Read Event Log.
                        ReadTableColumns(device, media, "P.98", false);
                    }
                    else
                    {
                        byte[] tmp = reply.ToArray();
                        //Read until EOP.
                        do
                        {
                            Progress(++pos % 10, 10);
                            if (tmp.Length != 0 && tmp[tmp.Length - 1] != 3)
                            {
                                string str = ASCIIEncoding.ASCII.GetString(tmp);
                                int index = str.IndexOf('(');
                                if (index != -1)
                                {
                                    str = str.Substring(0, index);
                                }
                                TraceLine("Reading Object " + str);
                            }
                            tmp = IEC62056PacketHandler.GetData(media, device.WaitTime, null, reply, '\0');
                        }
                        while (reply[reply.Count - 1] != 3);
                        media.Eop = null;
                        IEC62056PacketHandler.GetData(media, device.WaitTime, null, reply, '\0');
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
                                throw new Exception("Import failed. Data is corrupted.");
                            }
                        }
                    }
                    //Remove Eop.
                    reply.RemoveAt(reply.Count - 1);
                    //Send quit.
                    if (dev.ProtocolMode != Protocol.ModeA)
                    {
                        data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
                        media.Send(data, null);
                    }
                    media.Close();
                    List<Parser.IECDataObject> items = Parser.IEC62056Parser.ParseReadout(reply);
                    GXCategory defaultCategory = null;
                    if (dev.ProtocolMode == Protocol.ModeA)
                    {
                        defaultCategory = new GXIEC62056ReadoutCategory();
                        defaultCategory.Name = "Readout";
                        device.Categories.Add(defaultCategory);
                    }
                    else
                    {
                        defaultCategory = new GXIEC62056Category();
                        defaultCategory.Name = "Properties";
                        device.Categories.Add(defaultCategory);
                    }

                    for (int i = 0; i < items.Count; ++i)
                    {
                        Parser.IECDataObject item = items[i];
                        if (item.Address.Contains("*"))
                        {
                            items.RemoveAt(i--);
                            continue;
                        }
                        if (dev.ProtocolMode == Protocol.ModeA)
                        {
                            GXIEC62056ReadoutProperty prop = new GXIEC62056ReadoutProperty();
                            prop.AccessMode = AccessMode.Read;
                            prop.Name = item.Address;
                            prop.Address = item.Address;
                            prop.Unit = item.Unit;
                            prop.ValueType = typeof(string);
                            defaultCategory.Properties.Add(prop);
                        }
                        else
                        {
                            GXIEC62056Property prop = new GXIEC62056Property();
                            prop.AccessMode = AccessMode.Read;
                            prop.Name = item.Address;
                            prop.Address = item.Address;
                            prop.Unit = item.Unit;
                            prop.ValueType = typeof(string);
                            defaultCategory.Properties.Add(prop);
                        }
                    }
                }
            }
            finally
            {
                //Send quit if not send yet.
                if (media.IsOpen && dev.ProtocolMode != Protocol.ModeA)
                {
                    string data = (char)0x01 + "B0" + (char)0x03 + "\r\n";
                    media.Send(data, null);
                }
            }
        }

        private static void ReadTableColumns(GXDevice device, Gurux.Common.IGXMedia media, string tableAddress, bool all)
        {
            try
            {
                List<byte> reply = new List<byte>();
                ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
                {
                    Count = 1,
                    WaitTime = 500
                };
                string data;
                if (all)
                {
                    data = (char)1 + "R6" + (char)2 + tableAddress + "(;;1)" + (char)3;
                }
                else
                {
                    data = (char)1 + "R6" + (char)2 + tableAddress + "(" + Parser.IEC62056Parser.DateTimeToString(DateTime.Now.Date, false) + ";" +
                        Parser.IEC62056Parser.DateTimeToString(DateTime.Now.AddDays(1).Date, false) + ";1)" + (char)3;
                }
                List<byte> bytes = new List<byte>(ASCIIEncoding.ASCII.GetBytes(data));
                bytes.RemoveAt(0);
                char checksum = (char)Parser.IEC62056Parser.CalculateChecksum(bytes.ToArray());
                data += checksum;
                List<byte> allData = new List<byte>();
                List<byte> block = new List<byte>();
                while (true)
                {
                    reply.Clear();
                    IEC62056PacketHandler.GetData(media, device.WaitTime, data, reply, '\0');
                    //Remove Bop if exists.
                    byte bop = reply[0];
                    if (bop == 0x2)
                    {
                        reply.RemoveAt(0);
                    }
                    byte eop = reply[reply.Count - 1];
                    //If more data available.
                    if (eop == 0xA)
                    {
                        block.AddRange(reply);
                        data = null;
                    }
                    else
                    {
                        if (eop == 3)
                        {
                            block.AddRange(reply);
                            //Read crc.
                            p.Reply = null;
                            p.Count = 1;
                            if (media.Receive(p))
                            {
                                byte checksum2 = Parser.IEC62056Parser.CalculateChecksum(block.ToArray());
                                if (checksum2 != p.Reply[0])
                                {
                                    throw new Exception("Import failed. Data is corrupted.");
                                }
                            }
                            allData.AddRange(block);
                            block.Clear();
                            List<Parser.IECDataObject> columns = Parser.IEC62056Parser.ParseTable(tableAddress, allData, false);
                            GXIEC62056Table table = new GXIEC62056Table();
                            table.Address = tableAddress;
                            table.Name = tableAddress;
                            foreach (Parser.IECDataObject it in columns)
                            {
                                GXIEC62056TableProperty tp = new GXIEC62056TableProperty();
                                if (string.IsNullOrEmpty(it.Address))
                                {
                                    it.Address = "Unknown";
                                }
                                tp.ValueType = typeof(string);
                                tp.Name = it.Address;
                                table.Columns.Add(tp);
                            }
                            device.Tables.Add(table);
                            break;
                        }
                        else if (eop == 4)
                        {
                            block.AddRange(reply);
                            //Read crc.
                            p.Reply = null;
                            p.Count = 1;
                            if (media.Receive(p))
                            {
                                byte checksum2 = Parser.IEC62056Parser.CalculateChecksum(block.ToArray());
                                if (checksum2 != p.Reply[0])
                                {
                                    throw new Exception("Import failed. Data is corrupted.");
                                }
                            }
                            allData.AddRange(block);
                            block.Clear();
                            data = (char)6 + "";
                        }
                        else
                        {
                            block.AddRange(reply);
                        }
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                if (!all)
                {
                    ReadTableColumns(device, media, tableAddress, true);
                }
            }
        }

        public override void ModifyWizardPages(object source, GXPropertyPageType type, List<Control> pages)
		{
            if (type == GXPropertyPageType.Property)
            {
                pages.Insert(1, new AddressDlg(source as GXProperty));
            }            
            else if (type == GXPropertyPageType.Import)
            {
                pages.Insert(1, new ImportSelectionDlg());
            }
            else if (type == GXPropertyPageType.Device)
            {
                pages.Insert(1, new IEC62056DeviceModeWizardDlg(source as GXDevice));
            }                
		}
	}
}
