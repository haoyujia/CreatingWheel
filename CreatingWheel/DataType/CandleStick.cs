using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingWheel
{
    class CandleStick
    {
        /// <summary>
        /// Ending time
        /// </summary>
        public DateTime EndTime {get; set;}

        /// <summary>
        /// High price
        /// </summary>
        public double HighP { get; set; }

        /// <summary>
        /// Low price
        /// </summary>
        public double LowP { get; set; }

        /// <summary>
        /// Opening price
        /// </summary>
        public double OpenP { get; set; }

        /// <summary>
        /// Closing price
        /// </summary>
        public double CloseP { get; set; }

        /// <summary>
        /// Total amount during the period
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Total volume during the period
        /// </summary>
        public int Volume { get; set; }

        /// <summary>
        /// Period of the candlestick
        /// </summary>
        public TimeSpan Period { get; set; }

        /// <summary>
        /// Contract code, e.g: SR, RM
        /// </summary>
        public String ContractCode { get; set; }

        /// <summary>
        /// The month of delievery
        /// </summary>
        public String DeliveryMonth { get; set; }

        /// <summary>
        /// Trading house code
        /// </summary>
        public String TradingHouseCode { get; set; }
    }
}
