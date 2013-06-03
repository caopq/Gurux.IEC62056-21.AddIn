using System;
using System.Collections.Generic;
using System.Text;

namespace Gurux.IEC62056_21.AddIn.Parser
{
	class IEC62056Parser
	{
		public static List<Parser.IECDataObject> ParseReadout(List<byte> dataBlock)
		{
			List<Parser.IECDataObject> items = new List<Parser.IECDataObject>();
			bool cont = true;
			while (cont)
			{
				List<byte> dataSet = new List<byte>();

				int crIndex = dataBlock.IndexOf(0x0D);
				if (crIndex >= 0 && dataBlock[crIndex + 1] == 0x0A)
				{
					dataSet = dataBlock.GetRange(0, crIndex + 2);
					dataBlock.RemoveRange(0, crIndex + 2);

					IECDataObject data = new IECDataObject();
					int bracketIndex1 = dataSet.IndexOf(0x28);
                    if (bracketIndex1 == -1)
                    {
                        continue;
                    }
					int bracketIndex2 = dataSet.LastIndexOf(0x29);
					int asteriskIndex = dataSet.IndexOf(0x2a, bracketIndex1);

					List<byte> addressBytes = dataSet.GetRange(0, bracketIndex1);
					data.Address = ASCIIEncoding.ASCII.GetString(addressBytes.ToArray());
					data.Address = data.Address.Replace('-', '.');
					data.Address = data.Address.Replace(':', '.');

					if (asteriskIndex == -1)
					{
						List<byte> valueBytes = dataSet.GetRange(bracketIndex1 + 1, bracketIndex2 - (bracketIndex1 + 1));
						data.Value = ASCIIEncoding.ASCII.GetString(valueBytes.ToArray());

						data.Unit = null;
					}
					else
					{
						List<byte> valueBytes = dataSet.GetRange(bracketIndex1 + 1, asteriskIndex - 1 - bracketIndex1);
						data.Value = ASCIIEncoding.ASCII.GetString(valueBytes.ToArray());

						List<byte> unitBytes = dataSet.GetRange(asteriskIndex + 1, bracketIndex2 - 1 - asteriskIndex);
						data.Unit = ASCIIEncoding.ASCII.GetString(unitBytes.ToArray());
					}
					items.Add(data);
				}
				else
				{
					cont = false;
				}
			}
			return items;
		}

		public static List<Parser.IECDataObject> ParseTable(string tableAddress, List<byte> fullData, bool iskra)
		{
			string tableData = ASCIIEncoding.ASCII.GetString(fullData.ToArray());
			tableData = tableData.Replace("\n", "").Replace("\r", "");
			string[] tables = tableData.Split(new string[] { tableAddress }, StringSplitOptions.RemoveEmptyEntries);
			if (tables.Length == 0)
			{
				throw new Exception("Incorrect data.\r\n");
			}

			List<string> values = new List<string>();
			List<Parser.IECDataObject> props = new List<Parser.IECDataObject>();
			int columnCount = 0;

			bool iskraP98 = iskra && tableAddress.ToLower() == "p.98";
			for (int i = 0; i < tables.Length; ++i)
			{

				tableData = tables[i];
				if (tableData.StartsWith("(ER"))
				{
					throw new ArgumentOutOfRangeException(tableData);
				}

				int openBracket = 0, closeBracket = 0, interval = 0;
				string tmpStr, timeStamp = "", statusWord = "";
				if (!iskraP98)
				{
					openBracket = tableData.IndexOf("(", closeBracket);
					closeBracket = tableData.IndexOf(")", openBracket);
					timeStamp = tableData.Substring(openBracket + 1, closeBracket - openBracket - 1);

					openBracket = tableData.IndexOf("(", closeBracket);
					closeBracket = tableData.IndexOf(")", openBracket);
					statusWord = tableData.Substring(openBracket + 1, closeBracket - openBracket - 1);

					openBracket = tableData.IndexOf("(", closeBracket);
					closeBracket = tableData.IndexOf(")", openBracket);
					tmpStr = tableData.Substring(openBracket + 1, closeBracket - openBracket - 1);

					if (tmpStr != "")
					{
						interval = int.Parse(tmpStr);
					}

					openBracket = tableData.IndexOf("(", closeBracket);
					closeBracket = tableData.IndexOf(")", openBracket);
					columnCount = int.Parse(tableData.Substring(openBracket + 1, closeBracket - openBracket - 1));

					if (i == 0)
					{
						props.Add(new Parser.IECDataObject() { Address = "TimeStamp" });
						props.Add(new Parser.IECDataObject() { Address = "StatusWord" });
					}
					for (int j = 0; j < columnCount; ++j)
					{
						Parser.IECDataObject prop = new Parser.IECDataObject();

						openBracket = tableData.IndexOf("(", closeBracket);
						closeBracket = tableData.IndexOf(")", openBracket);
						tmpStr = tableData.Substring(openBracket + 1, closeBracket - openBracket - 1);
						prop.Address = tmpStr.Replace(':', '.').Replace('-', '.');

						openBracket = tableData.IndexOf("(", closeBracket);
						closeBracket = tableData.IndexOf(")", openBracket);
						prop.Unit = tableData.Substring(openBracket + 1, closeBracket - openBracket - 1);
						if (i == 0)
						{
							props.Add(prop);
						}
					}
				}
				else
				{
					if (i == 0)
					{
						columnCount = 2;
						Parser.IECDataObject prop = new Parser.IECDataObject();
						prop.Address = "TimeStamp";
						prop.Unit = "";
						props.Add(prop);

						prop = new Parser.IECDataObject();
						prop.Address = "StatusWord";
						prop.Unit = "";
						props.Add(prop);
					}
					openBracket = -1;
					closeBracket = -1;
				}

				string[] tmpArr = tables[i].Substring(closeBracket + 1).Replace("(", "").Split(new string[] { ")" }, StringSplitOptions.RemoveEmptyEntries);
				for (int valueCnt = 0; valueCnt < tmpArr.Length; ++valueCnt)
				{
					if (tmpArr[valueCnt] != "" && tmpArr[valueCnt][0] == (char)0x03)
					{
						break; //eop
					}

					
					if (valueCnt % (columnCount) == 0 && !iskraP98)
					{
						DateTime tmpDt = StringToDateTime(timeStamp).AddMinutes(interval * valueCnt / columnCount);
						values.Add(DateTimeToString(tmpDt, iskra));
						//values.Add(statusWord); //If this doesn't work properly, use the version in commented below
						if (valueCnt == 0)
						{
							values.Add(statusWord);
						}
						else
						{
							values.Add(AddLeadingZero("0", statusWord.Length));
						}
					}

					values.Add(tmpArr[valueCnt]);
				}
			}

			int colCounter = 0;

			for (int i = 0; i < values.Count; ++i)
			{
				props[colCounter].RowValues.Add(values[i]);

				++colCounter;
				if ((!iskraP98 && colCounter == columnCount + 2) || (iskraP98 && colCounter == columnCount))
				{
					colCounter = 0;
				}
			}
			return props;
		}

		public static byte CalculateChecksum(byte[] list)
		{
			byte result = 0;
			foreach (byte b in list)
			{
				result ^= b;
			}
			return (result);
		}



		public static DateTime StringToDateTime(string dateStr)
		{
			try
			{
				int year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
			    year = int.Parse(dateStr.Substring(0, 3));
				month = int.Parse(dateStr.Substring(3, 2));
				day = int.Parse(dateStr.Substring(5, 2));
				hour = int.Parse(dateStr.Substring(7, 2));
				minute = int.Parse(dateStr.Substring(9, 2));
                if (dateStr.Length > 12)
                {
                    second = int.Parse(dateStr.Substring(11, 2));
                }
				if (year > 50)
				{
					year += 1900;
				}
				else
				{
					year += 2000;
				}
                return new DateTime(year, month, day, hour, minute, second);
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
				string year;
				if (includeSeconds)
				{
					year = dt.Year.ToString().Substring(2, 2);
				}
				else
				{
					year = AddLeadingZero(dt.Year.ToString(), 4).Substring(1, 3);
				}
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

	}
}
