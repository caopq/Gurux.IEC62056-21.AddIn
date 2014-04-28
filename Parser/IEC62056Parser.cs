using System;
using System.Collections.Generic;
using System.Text;
using Gurux.Common;

namespace Gurux.IEC62056_21.AddIn.Parser
{
    class IEC62056Parser
    {
        /// <summary>
        /// Return Eop bytes.
        /// </summary>
        /// <returns></returns>
        internal static byte[] GetEops()
        {
            //EOP = 4 when data block is read.
            //EOP = 3 when all data is read.
            //EOP = 10 when row is read.
            //EOP = 0x15 when there is an error.
            //EOP = 0x6 ACK.
            return new byte[] { 10, 3, 4, 0x15, 6 };
        }

        static string GetADescription(string a)
        {
            switch (a)
            {
                case "0":
                    return "Abstract objects";
                case "1":
                    return "Electricity";
                case "4":
                    return "Heating costs";
                case "5":
                    return "Cooling energy";
                case "6":
                    return "Heat";
                case "7":
                    return "Gas";
                case "8":
                    return "Cold Water";
                case "9":
                    return "Warm water";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static string GetCDescription(string c)
        {
            switch (c)
            {
                case "0":
                    return "General purpose objects";
                case "1":
                    return "Σ Li active power + (import)";
                case "2":
                    return "Σ Li active power - (export )";
                case "3":
                    return "Σ Li reactive power +";
                case "4":
                    return "Σ Li reactive power -";
                case "5":
                    return "Σ Li reactive power Q I";
                case "6":
                    return "Σ Li reactive power Q II";
                case "7":
                    return "Σ Li reactive power Q III";
                case "8":
                    return "Σ Li reactive power Q IV";
                case "9":
                    return "Σ Li apparent power +";
                case "10":
                    return "Σ Li apparent power -";
                case "11":
                    return "";
                case "12":
                    return "";
                case "13":
                    return "Power factor";
                case "14":
                    return "Frequency";

                case "21":
                    return "L1 active power +";
                case "22":
                    return "L1 active power -";
                case "23":
                    return "L1 reactive power +";
                case "24":
                    return "L1 reactive power -";
                case "25":
                    return "L1 reactive power Q I";
                case "26":
                    return "L1 reactive power Q II";
                case "27":
                    return "L1 reactive power Q III";
                case "28":
                    return "L1 reactive power Q IV";
                case "29":
                    return "L1 apparent power +";
                case "30":
                    return "L1 apparent power -";
                case "31":
                    return "L1 phase A current";
                case "32":
                    return "L1 phase A voltage";
                case "33":
                    return "L1 power factor";
                case "41":
                    return "L2 active power +";
                case "42":
                    return "L2 active power -";
                case "43":
                    return "L2 reactive power +";
                case "44":
                    return "L2 reactive power -";
                case "45":
                    return "L2 reactive power Q I";
                case "46":
                    return "L2 reactive power Q II";
                case "47":
                    return "L2 reactive power Q III";
                case "48":
                    return "L2 reactive power Q IV";
                case "49":
                    return "L2 apparent power +";
                case "50":
                    return "L2 apparent power -";
                case "51":
                    return "L2 phase B current";
                case "52":
                    return "L2 phase B voltage";
                case "53":
                    return "L2 power factor";
                case "61":
                    return "L3 active power +";
                case "62":
                    return "L3 active power -";
                case "63":
                    return "L3 reactive power +";
                case "64":
                    return "L3 reactive power -";
                case "65":
                    return "L3 reactive power Q I";
                case "66":
                    return "L3 reactive power Q II";
                case "67":
                    return "L3 reactive power Q III";
                case "68":
                    return "L3 reactive power Q IV";
                case "69":
                    return "L3 apparent power +";
                case "70":
                    return "L3 apparent power -";
                case "71":
                    return "L3 phase C current";
                case "72":
                    return "L3 phase C voltage";
                case "73":
                    return "L3 power factor";
                case "94":
                    return "Country specific OBIS codes for different countries";
                case "C":
                    return "Service information";
                case "F":
                    return "Error messages";
                case "L":
                    return "List objects";
                case "P":
                    return "Data profiles";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static string GetDDescription(string d)
        {
            switch (d)
            {
                case "0":
                    return "";
                case "1":
                    return "Cumulative minimum 1";
                case "2":
                    return "Cumulative maximum 1";
                case "3":
                    return "Minimum 1";
                case "4":
                    return "Average value 1 current measuring period";
                case "5":
                    return "Average value 1 last measuring period";
                case "6":
                    return "Maximum 1";
                case "7":
                    return "Instantaneous value";
                case "8":
                    return "Energy";
                case "9":
                    return "Time integral 2";
                case "10":
                    return "Time integral 3";
                case "11":
                    return "Cumulative minimum 2";
                case "12":
                    return "Cumulative maximum 2";
                case "13":
                    return "Minimum 2";
                case "14":
                    return "Average value 2 current measuring period";
                case "15":
                    return "Average value 2 last measuring period";
                case "16":
                    return "Maximum 2";
                case "21":
                    return "Cumulative minimum 3";
                case "22":
                    return "Cumulative maximum 3";
                case "23":
                    return "Minimum 3";
                case "24":
                    return "Average value 3 current measuring period";
                case "25":
                    return "Average value 3 last measuring period";
                case "26":
                    return "Maximum 3";
                case "27":
                    return "";
                case "28":
                    return "";
                case "29":
                    return "Energy feed";
                case "55":
                    return "Test equipment";
                case "58":
                    return "Testing time integral";
                case "F":
                    return "Error message ";
            }
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        static string GetEDescription(string e)
        {
            switch (e)
            {
                case "0":
                    return "Total";
                case "1":
                    return "Tariff 1";
                case "2":
                    return "Tariff 2";
                case "3":
                    return "Tariff 3";
                case "4":
                    return "Tariff 4";
                case "5":
                    return "Tariff 5";
                case "6":
                    return "Tariff 6";
                case "7":
                    return "Tariff 7";
                case "8":
                    return "Tariff 8";
                case "9":
                    return "Tariff 9";
                default:
                    return "";
            }
        }

        static public DataType GetDataType(string obis)
        {
            if (obis == "0.9.1")
            {
                return DataType.Time;
            }
            if (obis == "0.9.2")
            {
                return DataType.Date;
            }
            return DataType.String;
        }

        /// <summary>
        /// Get list of general OBIS Codes.
        /// </summary>
        /// <returns></returns>
        static public string[] GetGeneralOBISCodes()
        {
            List<string> list = new List<string>();
            list.Add("0.0.0");
            list.Add("0.0.1");
            list.Add("0.0.2");
            list.Add("0.0.3");
            list.Add("0.1.0");
            list.Add("0.2.0");
            list.Add("0.2.2");
            list.Add("0.9.0");
            list.Add("0.9.1");
            list.Add("0.9.2");
            list.Add("0.9.5");             
            list.Add("P.01");
            list.Add("P.02");
            list.Add("P.98");
            list.Add("P.99");            
            return list.ToArray();
        }

        static string GetGeneralDescription(string obis)
        {
            switch (obis)
            {
                case "0.0.0":
                    return "Meter number";
                case "0.0.1":
                    return "Identity number 1";
                case "0.0.2":
                    return "Identity number 2";
                case "0.0.3":
                    return "Identity number 3";
                case "0.1.0":
                    return "Billing period counter";
                case "0.2.0":
                    return "Firmware version";
                case "0.2.2":
                    return "Timers program number";
                case "0.9.0":
                    return "Number of days since the last reset";
                case "0.9.1":
                    return "Meter time";
                case "0.9.2":
                    return "Meter date";
                case "0.9.5":
                    return "Day of week";
                case "P.01":
                    return "Load profile 1";
                case "P.02":
                    return "Load profile 2";
                case "P.98":
                    return "Logbook 1";
                case "P.99":
                    return "Logbook 2";
            }
            if (obis.StartsWith("0.1.2*"))
            {
                return "Timestamp of the billing period " + obis.Substring(obis.IndexOf("0.1.2*") + 6);
            }
            if (obis.StartsWith("0.2.1*"))
            {
                return "Parameter set number " + obis.Substring(obis.IndexOf("0.2.1*") + 6);
            }
            if (obis.StartsWith("0.8.6*"))
            {
                return "Duration of the Billing period " + obis.Substring(obis.IndexOf("0.8.6*") + 6);
            }
            return "";
        }

        public static string GetDescription(string obis)
        {
            if (string.IsNullOrEmpty(obis))
            {
                return "";
            }
            string desc = GetGeneralDescription(obis);
            if (!string.IsNullOrEmpty(desc))
            {
                return desc;
            }
            List<string> items = new List<string>(obis.Split(new char[] { '-', '.', ':', '*' }));
            if (items.Count == 1)
            {
                return obis;
            }
            if (items.Count != 6 && items.Count != 4)
            {
                throw new Exception("Invalid OBIS Code.");
            }
            desc = GetADescription(items[0]);
            desc += " " + GetCDescription(items[2]);
            desc += " " + GetDDescription(items[3]);
            if (items.Count > 4)
            {
                desc += " " + GetEDescription(items[4]);
            }
            return desc;
        }

        /// <summary>
        /// Calculate checksum.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte CalculateChecksum(List<byte> data, int index, int count)
        {
            if (count == -1)
            {
                count = data.Count - index;
            }
            byte result = 0;
            for (int pos = index; pos != index + count; ++pos)
            {
                result ^= data[pos];
            }
            return result;
        }

        /// <summary>
        /// Is all data read.
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        static public bool IsAllDataRead(byte[] buff)
        {
            bool crc;
            int eop, soh, stx, etx;
            if (!GetPacketInfo(new List<byte>(buff), false, out eop, out soh, out stx, out etx, out crc))
            {
                return false;
            }
            return buff[etx - 1] == 0x3;
        }

        /// <summary>
        /// Is more rows available.
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        static public bool IsMoreDataAvailable(byte[] buff)
        {
            bool crc;
            int eop, soh, stx, etx;
            if (!GetPacketInfo(new List<byte>(buff), false, out eop, out soh, out stx, out etx, out crc))
            {
                return false;
            }
            return buff[etx - 1] == 0x3;
        }

        /// <summary>
        /// Is packet parsed.
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        static public bool IsPacketReady(byte[] buff)
        {
            bool crc;
            int eop, soh, stx, etx;
            return GetPacketInfo(new List<byte>(buff), false, out eop, out soh, out stx, out etx, out crc);
        }

        static bool GetPacketInfo(List<byte> buff, bool throwException, out int eop, out int soh, out int stx, out int etx, out bool crc)
        {
            crc = true;
            eop = soh = stx = etx = -1;
            if (buff.Count == 0)
            {
                if (throwException)
                {
                    throw new Exception("Data is empty.");
                }
                return false;
            }
            for (int pos = 0; pos != buff.Count; ++pos)
            {
                if (soh == -1 && buff[pos] == 0x15)
                {
                    throw new Exception("Device is return error.");
                }
                if (buff[pos] == 0x1 || buff[pos] == 0x6)
                {
                    if (etx != -1 || soh != -1)
                    {
                        break;
                    }
                    soh = pos;
                    if (buff[pos] == 0x6)
                    {
                        eop = soh;
                        break;
                    }
                }
                else if (buff[pos] == 0x2)
                {
                    if (stx != -1)
                    {
                        break;
                    }
                    stx = pos;
                }
                else if (buff[pos] == 0x3 || buff[pos] == 0x4)
                {
                    if (etx != -1)
                    {
                        break;
                    }
                    etx = pos;
                }
                else if (buff[pos] == 0xA)
                {
                    eop = pos;
                }
            }
            int start = soh;
            if (soh == -1)
            {
                start = stx;
            }
            if (start == -1)
            {
                if (throwException)
                {
                    throw new Exception("Invalid Frame start char.");
                }
                return false;
            }
            if (etx == -1)
            {
                if (buff[start] == 0x6)
                {
                    return true;
                }
                if (eop == -1)
                {
                    if (throwException)
                    {
                        throw new Exception("Invalid Frame end char.");
                    }
                    return false;
                }
            }
            else
            {
                if (buff.Count < etx + 2)
                {
                    if (throwException)
                    {
                        throw new Exception("Invalid CRC.");
                    }
                    return false;
                }
                if (etx - start < 0 || 
                    (buff[etx + 1] != 0 && CalculateChecksum(buff, start + 1, etx - start) != buff[etx + 1]))
                {
                    if ((buff[etx + 1] == 13))
                    {
                        crc = false;
                        return true;
                    }
                    if (throwException)
                    {
                        throw new Exception("Invalid CRC.");
                    }
                    return false;
                }
            }
            return true;
        }


        static public byte[] GetPacket(List<byte> buff, bool throwException, out string header, out string data)
        {            
            header = data = null;
            bool crc;
            int eop, soh, stx, etx;
            if (!GetPacketInfo(buff, throwException, out eop, out soh, out stx, out etx, out crc))
            {
                return null;
            }
            byte[] allData = buff.ToArray();
            int len = 0, start = soh;
            if (etx == -1)
            {
                len = eop - soh + 1;
                if (soh == -1)
                {
                    return null;
                }
            }
            else
            {
                if (start == -1)
                {
                    start = stx;
                    if (start == -1)
                    {
                        return null;
                    }
                }
                if (etx == -1)
                {
                    return null;
                }
                len = etx - start + 2;
            }
            byte[] tmp = new byte[len];
            Array.Copy(allData, start, tmp, 0, len);
            //Get header.
            if (soh != -1)
            {
                if (stx != -1)
                {
                    len = stx - soh - 1;
                }
                else
                {
                    if (etx == -1)
                    {
                        return tmp;
                    }
                    len = etx - soh - 1;
                }
                header = ASCIIEncoding.ASCII.GetString(tmp, soh + 1, len);
            }
            //Get Data
            if (stx != -1)
            {
                data = ASCIIEncoding.ASCII.GetString(tmp, stx + 1, etx - stx - 1);
                if (data != null && data.Length > 2 && data[0] == '(' && data[data.Length - 1] == ')')
                {
                    data = data.Substring(1, data.Length - 2);
                }
            }
            return tmp;
        }

        /// <summary>
        /// Generate read table message
        /// </summary>
        /// <param name="name"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="parameters"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static byte[] GenerateReadTable(string name, DateTime start, DateTime end, string parameters, int level, int count)
        {
            if (level < 0)
            {
                throw new ArgumentOutOfRangeException("Level");
            }
            List<byte> bytes = new List<byte>();
            bytes.Add((byte)'R');
            bytes.Add((byte)(0x30 + level));
            bytes.Add(2);
            bytes.AddRange(ASCIIEncoding.ASCII.GetBytes(name));
            bytes.Add((byte)'(');
            if (start == DateTime.MinValue && end == DateTime.MaxValue)
            {
                bytes.Add((byte)';');
            }
            else
            {
                if (start == DateTime.MinValue)
                {
                    start = new DateTime(2000, 1, 1);
                }
                if (end == DateTime.MaxValue)
                {
                    end = DateTime.Now.AddDays(1).Date;
                }
                bytes.AddRange(ASCIIEncoding.ASCII.GetBytes(DateTimeToString(start, false)));
                bytes.Add((byte)';');
                bytes.AddRange(ASCIIEncoding.ASCII.GetBytes(DateTimeToString(end, false)));
            }
            if (level == 6)
            {
                bytes.Add((byte)';');
                bytes.Add((byte)(0x30 + count));
            }
            bytes.Add((byte)')');
            if (parameters != null)
            {
                bytes.AddRange(ASCIIEncoding.ASCII.GetBytes(parameters));
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Parse table data.
        /// </summary>
        /// <param name="reply">Received data.</param>
        /// <param name="columns"></param>
        /// <param name="status"></param>
        /// <param name="tm"></param>
        /// <param name="add"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object[][] ParseTableData(byte[] reply, ref string[] columns, ref int status, ref DateTime tm, ref int add, string name)
        {
            string frame, header;
            byte[] reply2;            
            if ((reply2 = GetPacket(new List<byte>(reply), true, out header, out frame)) == null)
            {
                throw new Exception("Failed to receive reply from the device in given time.");
            }
            if (string.Compare("B0", header) == 0)
            {
                throw new Exception("Meter is closed the connecion.");
            }
            int start = frame.IndexOf('(');
            int end = frame.IndexOf(')');            
            string error = frame.Substring(start + 1, end - start - 1);
            if (error.StartsWith("ER"))
            {
                //Meter returns error if there are no data.
                if (error == "ERROR")
                {
                    return new object[0][];
                }
                end = error.IndexOf(')');
                if (end != -1)
                {
                    error = error.Substring(0, end);
                }
                throw new Exception(error);
            }
            string[] rows = frame.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<object[]> rowsData = new List<object[]>();
            int offset = 0;
            foreach (string row in rows)
            {
                List<string> cols = new List<string>(row.Split(new string[] { "(" }, StringSplitOptions.None));
                //If Load profile or log book.
                if (name == "P.01" || name == "P.02" || name == "P.98" || name == "P.99")
                {
                    if (row.StartsWith(name))
                    {
                        tm = StringToDateTime(cols[1].Replace(")", ""));
                        status = Convert.ToInt32(cols[2].Replace(")", ""), 16);
                        if (cols[3].Replace(")", "").Trim() != "")
                        {
                            add = Convert.ToInt32(cols[3].Replace(")", ""));
                        }
                        int cnt = Convert.ToInt32(cols[4].Replace(")", ""));
                        if (columns == null)
                        {
                            columns = new string[2 + cnt];
                            columns[0] = "DateTime";
                            columns[1] = "Status";
                            if (cnt == (cols.Count - 4) / 2)
                            {
                                for (int pos = 0; pos != cnt; ++pos)
                                {
                                    columns[2 + pos] = cols[5 + 2 * pos].Replace(")", "");
                                }
                            }
                            else
                            {
                                for (int pos = 0; pos != cnt; ++pos)
                                {
                                    columns[2 + pos] = cols[5 + 2 * pos].Replace(")", "");
                                }
                            }
                        }
                        if (cnt == 0)
                        {
                            object[] data = new object[columns.Length];
                            data[0] = tm;
                            data[1] = status;
                            rowsData.Add(data);
                        }
                        continue;
                    }
                    else
                    {
                        cols.RemoveAt(0);
                        object[] data = new object[columns.Length];
                        data[0] = tm;
                        data[1] = status;
                        tm = tm.AddMinutes(add);
                        if (offset == 0)
                        {
                            offset = (cols.Count / (columns.Length - 2));
                        }
                        for (int pos = 0; pos != cols.Count; pos += offset)
                        {
                            double value;
                            if (double.TryParse(cols[pos], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.GetCultureInfo("en-US"), out value))
                            {
                                data[2 + pos] = value;
                            }
                            else
                            {
                                data[2 + pos] = cols[pos].Replace(")", "");
                            }
                        }
                        rowsData.Add(data);
                    }
                }
            }
            return rowsData.ToArray();
        }

        /// <summary>
        /// Disconnect.
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="tryCount"></param>
        public static void Disconnect(IGXMedia media, int tryCount)
        {
            lock (media.Synchronous)
            {
                ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
                {
                    Eop = GetEops(),
                    WaitTime = 1000
                };
                List<byte> data = new List<byte>();
                data.Add(0x01);
                data.Add((byte)'B');
                data.Add((byte)'0');
                data.Add(0x03);
                data.Add(CalculateChecksum(data, 1, data.Count - 1));
                byte[] data1 = data.ToArray();
                string header, frame;                
                media.Send(data1, null);
                media.Receive(p);
                while (--tryCount > -1)
                {
                    if (media.Receive(p))
                    {                        
                        GetPacket(new List<byte>(p.Reply), false, out header, out frame);
                        if (header == "B0")
                        {
                            break;
                        }
                        if (p.Reply.Length > 11)
                        {
                            p.Reply = null;
                        }
                    }
                    media.Send(data1, null);
                }
            }
        }

        /// <summary>
        /// Read table data from the meter.
        /// </summary>
        /// <remarks>
        /// With commmand R6 data can be read one row at the time and it is parsed on the fly.
        /// With other commands all data must first read before data can be parse.
        /// </remarks>
        /// <param name="media"></param>
        /// <param name="wt"></param>
        /// <param name="name"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static object[][] ReadTable(Gurux.Common.IGXMedia media, int wt, string name, DateTime start, DateTime end, string parameters, int level, int count, out string[] columns)
        {
            columns = null;
            List<byte> reply = new List<byte>();
            string header, frame;
            //Read CRC.
            ReceiveParameters<byte[]> crc = new ReceiveParameters<byte[]>()
            {
                Count = 1,
                WaitTime = wt
            };
            List<byte> data = new List<byte>();
            data.Add(1);
            data.AddRange(GenerateReadTable(name, start, end, parameters, level, count));
            data.Add(3);
            data.Add(CalculateChecksum(data, 1, data.Count - 1));
            lock (media.Synchronous)
            {
                do
                {
                    List<byte> tmp = new List<byte>(SendData(media, data == null ? null : data.ToArray(), '\0', wt, true, level == 6, false));
                    data = null;
                    if (level != 6 && tmp[tmp.Count - 1] == 0x3)
                    {
                        crc.Count = 1;
                        if (!media.Receive(crc))
                        {
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                        tmp.AddRange(crc.Reply);
                    }
                    else if (level == 6)
                    {                        
                        if (GetPacket(tmp, true, out header, out frame) == null)
                        {
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                        if (tmp[tmp.Count - 2] == 0x4)
                        {
                            //Send ACK if more data is left.
                            data = new List<byte>();
                            data.Add(6);
                            System.Threading.Thread.Sleep(200);
                        }
                    }
                    reply.AddRange(tmp);
                }
                while (reply[reply.Count - 2] != 0x3);
            }
            int status = 0;
            DateTime tm = DateTime.MinValue;
            int add = 0;
            return ParseTableData(reply.ToArray(), ref columns, ref status, ref tm, ref add, name);        
        }

        /// <summary>
        /// Convert string to DateTime.
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static TimeSpan StringToTime(string dateStr)
        {
            try
            {
                int hour = 0, minute = 0, second = 0;
                hour = int.Parse(dateStr.Substring(0, 2));
                minute = int.Parse(dateStr.Substring(2, 2));
                second = int.Parse(dateStr.Substring(4, 2));
                return new TimeSpan(hour, minute, second);
            }
            catch (Exception ex)
            {
                throw new Exception("StringToTime conversion failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Convert string to DateTime.
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string dateStr)
        {
            try
            {
                int index = 0;
                if (dateStr.Length % 2 != 0)
                {
                    index = 1;
                }
                int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
                year = int.Parse(dateStr.Substring(index, 2));
                month = int.Parse(dateStr.Substring(index + 2, 2));
                day = int.Parse(dateStr.Substring(index + 4, 2));
                if (dateStr.Length > 7)
                {
                    hour = int.Parse(dateStr.Substring(index + 6, 2));
                    minute = int.Parse(dateStr.Substring(index + 8, 2));
                    if (dateStr.Length > 11)
                    {
                        second = int.Parse(dateStr.Substring(index + 10, 2));
                    }
                }
                if (year > 50)
                {
                    year += 1900;
                }
                else
                {
                    year += 2000;
                }
                //if Season recognition.
                DateTimeKind kind = DateTimeKind.Local;
                if (index != 0)
                {
                    //Normal time
                    if (dateStr[0] == '0')
                    {
                        kind = DateTimeKind.Local;
                    }
                    //Summer time.
                    else if (dateStr[0] == '1')
                    {
                        kind = DateTimeKind.Local;
                    }
                    //UTC
                    else if (dateStr[0] == '2')
                    {
                        kind = DateTimeKind.Utc;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Invalid season recognition.");
                    }
                }
                return new DateTime(year, month, day, hour, minute, second, kind);
            }
            catch (Exception ex)
            {
                throw new Exception("StringToDateTime conversion failed: " + ex.Message);
            }
        }

        public static string DateTimeToString(DateTime dt, bool includeSeconds)
        {
            try
            {
                string year = dt.Year.ToString().Substring(2, 2);
                string month = AddLeadingZero(dt.Month.ToString(), 2);
                string day = AddLeadingZero(dt.Day.ToString(), 2);
                string hour = AddLeadingZero(dt.Hour.ToString(), 2);
                string minute = AddLeadingZero(dt.Minute.ToString(), 2);
                string second = string.Empty;
                if (includeSeconds)
                {
                    second = AddLeadingZero(dt.Second.ToString(), 2);
                }
                return year + month + day + hour + minute + second;
            }
            catch (Exception ex)
            {
                throw new Exception("DateTimeToString conversion failed: " + ex.Message);
            }
        }

        private static string AddLeadingZero(string input, int digitCount)
        {
            if (digitCount == input.Length)
            {
                return input;
            }
            else
            {
                input = "0" + input;
                return AddLeadingZero(input, digitCount);
            }
        }

        static public string ReadValue(Gurux.Common.IGXMedia media, int wt, string data, int level)
        {
            string dec = GetDescription(data.Substring(0, data.IndexOf('(')));
            if (dec != "")
            {
                System.Diagnostics.Debug.WriteLine("Reading property " + dec);
            }
            data = (char)1 + "R" + level.ToString() + (char)2 + data + (char)3;
            List<byte> bytes = new List<byte>(ASCIIEncoding.ASCII.GetBytes(data));
            bytes.RemoveAt(0);
            char checksum = (char)CalculateChecksum(bytes, 0, bytes.Count);
            data += checksum;
            List<byte> reply = new List<byte>();
            byte[] tmp = Read(media, data, '\0', wt, true);
            reply.AddRange(tmp);
            string header, frame;            
            GetPacket(reply, true, out header, out frame);
            int start = frame.IndexOf('(') + 1;
            int end = frame.LastIndexOf(')');
            return frame.Substring(start, end - start);
        }

        static public byte[] Identify(IGXMedia media, string data, char baudRate, int wt)
        {
            return SendData(media, ASCIIEncoding.ASCII.GetBytes(data), baudRate, wt, false, false, true);
        }

        static public byte[] ParseHandshake(IGXMedia media, string data, char baudRate, int wt)
        {
            return SendData(media, ASCIIEncoding.ASCII.GetBytes(data), baudRate, wt, false, true, true);
        }

        static public byte[] Read(IGXMedia media, string data, char baudRate, int wt, bool useCrc)
        {
            return SendData(media, ASCIIEncoding.ASCII.GetBytes(data), baudRate, wt, useCrc, true, true);
        }

        static public byte[] ReadReadout(IGXMedia media, string data, int wt)
        {
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = GetEops(),
                WaitTime = wt
            };
            lock (media.Synchronous)
            {
                if (data != null)
                {
                    media.Send(data, null);
                }
                do
                {
                    if (!media.Receive(p))
                    {
                        //There is no eop or CRC.
                        break;
                    }
                }
                while (p.Reply[p.Reply.Length - 1] != 0x3);
                //Read CRC if EOP is found.
                if (p.Reply[p.Reply.Length - 1] == 0x3)
                {
                    p.Eop = null;
                    p.Count = 1;
                    if (!media.Receive(p))
                    {
                        throw new Exception("Failed to receive reply from the device in given time.");
                    }
                }
            }
            return p.Reply;
        }

        /// <summary>
        /// Compares two byte or byte array values.
        /// </summary>
        public static bool EqualBytes(byte[] a, byte[] b)
        {
            if (a == null)
            {
                return b == null;
            }
            if (b == null)
            {
                return a == null;
            }
            if (a is Array && b is Array)
            {
                int pos = 0;
                if (((Array)a).Length != ((Array)b).Length)
                {
                    return false;
                }
                foreach (byte mIt in (byte[])a)
                {
                    if ((((byte)((byte[])b).GetValue(pos++)) & mIt) != mIt)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return BitConverter.Equals(a, b);
            }
            return true;
        }

        static byte[] SendData(IGXMedia media, byte[] data, char baudRate, int wt, bool useCrcSend, bool useCrcReply, bool readAllDataOnce)
        {
            ReceiveParameters<byte[]> p = new ReceiveParameters<byte[]>()
            {
                Eop = GetEops(),
                WaitTime = wt
            };
            lock (media.Synchronous)
            {
                if (data != null)
                {
                    media.Send(data, null);
                    if (baudRate != '\0' && media.MediaType == "Serial")
                    {
                        Gurux.Serial.GXSerial serial = media as Gurux.Serial.GXSerial;
                        while (serial.BytesToWrite != 0)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        System.Threading.Thread.Sleep(200);
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
                List<byte> reply2 = new List<byte>();
                reply2.AddRange(p.Reply);
                p.Reply = null;
                string header, frame;
                byte[] packet = null;
                if (useCrcSend && data != null)
                {                    
                    while ((packet = GetPacket(reply2, false, out header, out frame)) == null)
                    {
                        p.Eop = null;
                        p.Count = 1;
                        if (!media.Receive(p))
                        {
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                        reply2.AddRange(p.Reply);
                        p.Reply = null;
                    }
                    p.Eop = GetEops();
                }
                else
                {
                    for (int pos = 0; pos != reply2.Count; ++pos)
                    {
                        if (reply2[pos] == 0xA)
                        {
                            ++pos;
                            packet = new byte[pos];
                            reply2.CopyTo(0, packet, 0, pos);
                            break;
                        }
                    }
                }
                //Remove echo.
                if (data != null && EqualBytes(data, packet))
                {
                    reply2.RemoveRange(0, data.Length);
                    if (useCrcReply && reply2.Count != 0)// && !(data != null && data[data.Length - 1] == 0xA))
                    {
                        while (GetPacket(reply2, false, out header, out frame) == null)
                        {
                            p.Eop = null;
                            p.Count = 1;
                            if (!media.Receive(p))
                            {
                                throw new Exception("Failed to receive reply from the device in given time.");
                            }
                            reply2.AddRange(p.Reply);
                            p.Reply = null;
                        }
                    }
                    else
                    {
                        if (GetPacket(reply2, false, out header, out frame) == null)
                        {
                            reply2.AddRange(SendData(media, null, baudRate, wt, useCrcSend, useCrcReply, readAllDataOnce));
                        }
                    }
                    //If there is more data available.
                    if (readAllDataOnce && reply2[reply2.Count - 2] == 0x4)
                    {                        
                        reply2.AddRange(SendData(media, new byte[] { 6 }, '\0', wt, useCrcSend, useCrcReply, readAllDataOnce));
                    }
                    return reply2.ToArray();
                }
                if (useCrcReply && !(data != null && data[data.Length - 1] == 0xA))
                {
                    while (GetPacket(reply2, false, out header, out frame) == null)
                    {
                        p.Eop = null;
                        p.Count = 1;
                        if (!media.Receive(p))
                        {
                            throw new Exception("Failed to receive reply from the device in given time.");
                        }
                        reply2.AddRange(p.Reply);
                        p.Reply = null;
                    }
                }
                return reply2.ToArray();
            }
        }
    }
}
