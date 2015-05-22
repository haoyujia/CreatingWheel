using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace CreatingWheel.IO
{
    class Parser
    {
        /***********************************************Constants Starts Here*************************************************************/
        /// <summary>
        /// Used to compose a date time if it's day data
        /// </summary>
        private const string endingTime = " 15:00";

        /***********************************************Public Functions Starts Here*************************************************************/
        /// <summary>
        /// Read a data file and get all candle sticks
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<CandleStick> ReadFile(FileInfo file)
        {
            // Parse file name
            String tradingHouseCode = null, deliveryMonth = null, contractCode = null;
            ParseFileName(file.Name, out tradingHouseCode, out deliveryMonth, out contractCode);

            if (tradingHouseCode == null || deliveryMonth == null || contractCode == null)
            {
                throw new Exception(String.Format("Error: file name parsing is not right for file - {0}", file.Name));
            }

            // Calculate period
            var period = CalculatePeriod(file.FullName);

            var candleStickList = new List<CandleStick>();

            // Parse each line in the file and get candle sticks
            foreach (var line in File.ReadLines(file.FullName))
            {
                var candleStick = ParseLine(line, tradingHouseCode, deliveryMonth, contractCode, period);
                if (candleStick == null)
                {
                    throw new Exception(String.Format("Error: parsing line \"{0}\" in file {1}", line, file.Name));
                }

                candleStickList.Add(candleStick);
            }

            return candleStickList;
        }
        /***********************************************Private Functions Starts Here*************************************************************/
        /// <summary>
        /// Parse a file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static void ParseFileName(String fileName, out String tradingHouseCode, out String deliveryMonth, out String contractCode)
        {
            // Remove extension
            fileName = fileName.Split('.')[0];

            // "TradingHouseCode" + "CommodityName" + "ContractEndingMonth"
            // First two Char are always trading house code
            tradingHouseCode = fileName.Substring(0, 2);
            fileName = fileName.Remove(0, 2);

            // Last two char are always contract ending month. MI if it's a continuous index
            deliveryMonth = fileName.Substring(fileName.Length - 2, 2);
            fileName = fileName.Remove(fileName.Length - 2, 2);

            // what's left is the commodity name
            contractCode = fileName;
        }

        /// <summary>
        /// Find out what period the data is
        /// Assuming all data in one file are in the same period
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static TimeSpan CalculatePeriod(String fileName)
        {
            var lines = File.ReadLines(fileName).ToList();
            var line1 = lines[0];

            // Seven fields means it's day data
            if (line1.Split(',').Length == 7)
            {
                return TimeSpan.FromDays(1);
            }

            // Time diff between two lines is the period
            var line2 = lines[1];
            return Convert.ToDateTime(line2.Split(',')[1]) - Convert.ToDateTime(line1.Split(',')[1]);
        }

        /// <summary>
        /// Parse a line in the file and compose a candle stick
        /// </summary>
        /// <param name="line"></param>
        /// <param name="tradingHouseCode"></param>
        /// <param name="deliveryMonth"></param>
        /// <param name="contractCode"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        private static CandleStick ParseLine(String line, String tradingHouseCode, String deliveryMonth, String contractCode, TimeSpan period)
        {
            var candleStick = new CandleStick();

            var fields = line.Split(',');
            if (fields.Length < 7)
            {
                throw new Exception(String.Format("Incorret data: not enough fileds in \"{0}\"", line));
            }

            // Line Format: date, (time,) open, high, low, close, amount, volume
            // Seven fields means day periodic data, add a ending time
            var eTime = fields[0];
            eTime = String.Format("{0} {1}", eTime, (fields.Length == 7) ? endingTime : fields[1]);

            candleStick.EndTime = Convert.ToDateTime(eTime);

            // Prices: open, high, low close
            double openP = -1;
            Double.TryParse(fields[1], out openP);
            candleStick.OpenP = openP;

            double highP = -1;
            Double.TryParse(fields[2], out highP);
            candleStick.HighP = highP;

            double lowP = -1;
            Double.TryParse(fields[3], out lowP);
            candleStick.LowP = lowP;

            double closeP = -1;
            Double.TryParse(fields[4], out closeP);
            candleStick.CloseP = closeP;

            // Amount and Volume
            int amount = -1;
            Int32.TryParse(fields[5], out amount);
            candleStick.Amount = amount;

            int volume = -1;
            Int32.TryParse(fields[6], out volume);
            candleStick.Volume = volume;

            // Other Fields:
            candleStick.ContractCode = contractCode;
            candleStick.DeliveryMonth = deliveryMonth;
            candleStick.TradingHouseCode = tradingHouseCode;
            candleStick.Period = period;

            return candleStick;
        }
    }
}
