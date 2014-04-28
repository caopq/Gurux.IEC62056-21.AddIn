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
using Gurux.IEC62056_21.AddIn.Parser;

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
				return new Type[] { typeof(GXIEC62056Property), typeof(GXIEC62056ReadoutProperty) };
			}
			if (parent is GXIEC62056ReadoutCategory)
			{
				return new Type[] { typeof(GXIEC62056ReadoutProperty) };
			}
			else if (parent is GXIEC62056Table)
			{
                return new Type[] { typeof(GXIEC62056Property) };
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
				if (dev.Mode == Protocol.ModeA)
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
            ImportSelectionDlg dlg = addinPages[1] as ImportSelectionDlg;
            string deviceSerialNumber = dlg.DeviceSerialNumber;
            int waittime = dev.WaitTime;
            media.Open();
            try
            {
                string data = "/?" + deviceSerialNumber + "!\r\n";
                byte[] reply = IEC62056Parser.Identify(media, data, '\0', waittime);
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
                //If mode is not given.
                if (dev.Mode == Protocol.None)
                {
                    dev.Mode = mode;
                }
                data = ASCIIEncoding.ASCII.GetString(reply.ToArray());                
                string manufacturer = new string(new char[] { (char)reply[1], (char)reply[2], (char)reply[3] });
                if (dev.Mode == Protocol.ModeA)
                {
                    data = (char)0x06 + "0" + baudRate + "0\r\n";
                }
                else
                {
                    data = (char)0x06 + "0" + baudRate + "1\r\n";
                }
                //Note this sleep is in standard. Do not remove.
                if (media.MediaType == "Serial")
                {
                    System.Threading.Thread.Sleep(200);
                }
                reply = IEC62056Parser.ParseHandshake(media, data, baudRate, waittime);
                string header, frame;                
                IEC62056Parser.GetPacket(new List<byte>(reply), true, out header, out frame);
                System.Diagnostics.Debug.WriteLine(frame);
                if (header == "B0")
                {
                    throw new Exception("Connection failed. Meter do not accept connection at the moment.");
                }
                //Password is asked.
                if (header == "P0")
                {
                    System.Diagnostics.Debug.WriteLine("Password is asked.");
                }
                //Note this sleep is in standard. Do not remove.
                if (media.MediaType == "Serial")
                {
                    System.Threading.Thread.Sleep(200);
                }
                if (dev.Mode == Protocol.ModeA)
                {
                    GXCategory defaultCategory = new GXIEC62056ReadoutCategory();
                    defaultCategory.Name = "Readout";
                    device.Categories.Add(defaultCategory);
                }
                else
                {
                    GXCategory defaultCategory = null;
                    defaultCategory = new GXIEC62056Category();
                    defaultCategory.Name = "Properties";
                    device.Categories.Add(defaultCategory);
                    foreach (string it in IEC62056Parser.GetGeneralOBISCodes())
                    {
                        try
                        {
                            //Note this sleep is in standard. Do not remove.
                            if (media is Gurux.Serial.GXSerial)
                            {
                                System.Threading.Thread.Sleep(200);
                            }
                            if (!it.StartsWith("P."))
                            {
                                string value = IEC62056Parser.ReadValue(media, waittime, it + "()", 2);
                                if (!Convert.ToString(value).StartsWith("ER"))
                                {
                                    GXIEC62056Property prop = new GXIEC62056Property();
                                    prop.AccessMode = AccessMode.Read;
                                    prop.ReadMode = dev.ReadMode;
                                    prop.WriteMode = dev.WriteMode;
                                    prop.Name = IEC62056Parser.GetDescription(it);
                                    prop.Data = it;
                                    prop.DataType = IEC62056Parser.GetDataType(it);
                                    if (prop.DataType == DataType.DateTime ||
                                        prop.DataType == DataType.Date ||
                                        prop.DataType == DataType.Time)
                                    {
                                        prop.ValueType = typeof(DateTime);
                                    }
                                    defaultCategory.Properties.Add(prop);
                                    TraceLine("Property " + prop.Name + " added.");
                                }
                            }
                            else
                            {
                                object[][] rows;
                                //Try to read last hour first.
                                TimeSpan add = new TimeSpan(1, 0, 0);
                                DateTime start = DateTime.Now.Add(-add);
                                string[] columns = null;
                                do
                                {
                                    try
                                    {
                                        rows = IEC62056Parser.ReadTable(media, waittime, it, start, DateTime.Now, null, 5, 1, out columns);
                                    }
                                    catch
                                    {
                                        //If media is closed.
                                        if (!media.IsOpen)
                                        {
                                            break;
                                        }
                                        rows = new object[0][];
                                    }
                                    if (rows.Length == 0)
                                    {
                                        if (add.TotalHours == 1)
                                        {
                                            //Try to read last day.
                                            add = new TimeSpan(1, 0, 0, 0);
                                            start = DateTime.Now.Add(-add).Date;
                                        }
                                        else if (add.TotalHours == 24)
                                        {
                                            //Try to read last week.
                                            add = new TimeSpan(7, 0, 0, 0);
                                            start = DateTime.Now.Add(-add).Date;
                                        }
                                        else if (add.TotalDays == 7)
                                        {
                                            //Try to read last month.
                                            add = new TimeSpan(31, 0, 0, 0);
                                            start = DateTime.Now.Add(-add).Date;
                                        }
                                        else if (add.TotalDays == 31)
                                        {
                                            //Read all.
                                            add = new TimeSpan(100, 0, 0, 0);
                                            start = DateTime.MinValue;                                            
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        //Note this sleep is in standard. Do not remove.
                                        if (media is Gurux.Serial.GXSerial)
                                        {
                                            System.Threading.Thread.Sleep(200);
                                        }
                                    }
                                    else
                                    {
                                        GXIEC62056Table table = new GXIEC62056Table();
                                        table.Name = IEC62056Parser.GetDescription(it);
                                        table.AccessMode = AccessMode.Read;
                                        table.Data = it;
                                        table.ReadMode = 6;
                                        int index = -1;
                                        foreach (string col in columns)
                                        {
                                            GXIEC62056Property prop = new GXIEC62056Property();
                                            prop.Name = col;
                                            //Mikko prop.Name = IEC62056Parser.GetDescription(col);
                                            prop.Data = col;
                                            prop.ValueType = rows[0][++index].GetType();
                                            table.Columns.Add(prop);
                                        }
                                        device.Tables.Add(table);
                                        TraceLine("Property " + table.Name + " added.");
                                        break;
                                    }
                                }
                                while (rows.Length == 0);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);                            
                        }
                    }
                }
            }
            finally
            {
                if (media.MediaType == "Serial" || media.MediaType == "Terminal")
                {
                   IEC62056Parser.Disconnect(media, 2);
                }
                media.Close();
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
