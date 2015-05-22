using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingWheel.Database
{
    class ContractTable : DatabaseConnection
    {
        /// <summary>
        /// Save candle sticks to the database
        /// </summary>
        /// <param name="candleSticks"></param>
        /// <returns></returns>
        public static bool SaveCandleSticks(List<CandleStick> candleSticks)
        {
            if (candleSticks.Count == 0)
            {
                return true;
            }
            
            // Create the contract table, and add to contract index table
            bool success = CreateContractTable(candleSticks.First().ContractCode, candleSticks.First().DeliveryMonth);
            if (!success)
            {
                throw new Exception(String.Format("Failed to create table for {0}{1}", candleSticks.First().ContractCode, candleSticks.First().DeliveryMonth));
            }
            // Write to the index table if newly created
            if (!Database.ContractIndexTable.ContractTableExists(candleSticks.First().ContractCode, candleSticks.First().DeliveryMonth))
            {
                Database.ContractIndexTable.InsertRow(candleSticks.First().ContractCode, candleSticks.First().DeliveryMonth);
            }

            // Insert candle sticks to the table
            foreach (var candleStick in candleSticks)
            {
                SaveCandleStick(candleStick);
            }

            return true;
        }

        public static bool SaveCandleStick(CandleStick candleStick)
        {
            string sqlCmd = @"INSERT INTO futures.{0} (EndTime, HighP, LowP, OpenP, CloseP, Amount, Volume, Period, ContractCode, DeliveryMonth, TradingHouseCode) VALUES ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')";

            return RunCommand(
                String.Format(sqlCmd,
                    candleStick.ContractCode + candleStick.DeliveryMonth,
                    candleStick.EndTime,
                    candleStick.HighP,
                    candleStick.LowP,
                    candleStick.OpenP,
                    candleStick.CloseP,
                    candleStick.Amount,
                    candleStick.Volume,
                    candleStick.Period.TotalMinutes,
                    candleStick.ContractCode,
                    candleStick.DeliveryMonth,
                    candleStick.TradingHouseCode));
        }

        private static bool CreateContractTable(String contractName, String deliveryMonth)
        {
            string sqlCmd = @"CREATE TABLE IF NOT EXISTS futures.{0} (EndTime DATETIME, HighP DOUBLE, LowP DOUBLE, OpenP DOUBLE, CloseP DOUBLE, Amount BIGINT, Volume BIGINT, Period INT, ContractCode VARCHAR(5), DeliveryMonth VARCHAR(2), TradingHouseCode VARCHAR(5), PRIMARY KEY (EndTime, Period), FOREIGN KEY (ContractCode, DeliveryMonth) REFERENCES futures.ContractIndex(ContractCode, DeliveryMonth));";

            return RunCommand(String.Format(sqlCmd, contractName + deliveryMonth));
        }
    }
}
