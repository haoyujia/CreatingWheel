using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CreatingWheel.Database
{
    class DatabaseConnection
    {
        /**************************************************Define Constants************************************************************/
        /// <summary>
        /// Connection string
        /// </summary>
        static string connectionString = "server=127.0.0.1;user id=root;Password=qwerqwer;database=futures";
        /// <summary>
        /// Command used to Create a table
        /// </summary>
        static string createContractTableCommand = "create table {0}(Id BIGINT, commodity varChar(10), contract varChar(10), period int, eTime DateTime, highP double, lowP double, openP double, closeP double, vol BIGINT, amt BIGINT, house varChar(10));";
        static string alterTableCommand = "alter table {0} add primary key({1});";
        /// <summary>
        /// Command used to drop a table
        /// </summary>
        static string dropTableCommand = "drop table {0};";
        /// <summary>
        /// Command used to insert a row to a table
        /// 
        /// </summary>
        static string insertRowToContractTableCommand = "insert into {0} (Id, commodity, contract, period, eTime, highP, lowP, openP, closeP, vol, amt, house) values ({1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12});";
        /// <summary>
        /// Command used to get the maximum for a specified column in a table
        /// </summary>
        static string getMaxCommand = "SELECT Max({0}) FROM {1}";
        /// <summary>
        /// Command used to run selection
        /// </summary>
        static string selectCommand = "SELECT {0} FROM {1}";

        /**************************************************Public Functions Starts Here************************************************************/
        /// <summary>
        /// Save a list of candle sticks to the correct contract table
        /// </summary>
        /// <param name="candleStickList"></param>
        /// <returns></returns>
        public static bool SaveCandleSticks(List<CandleStick> candleStickList)
        {
            if (candleStickList == null)
            {
                return true;
            }

            // Check if the contract table exists for the commodity
            bool exist = CheckContractTable(candleStickList.First().ContractCode);
            return false;
        }

        /// <summary>
        ///  Table ContractIndex - which commodity is contained in the DB and that commodity contains which contract 
        ///  Schema: Commodity varChar(10), Contract varChar(10)
        ///  Primary Key: (Commodity, Contract)
        /// </summary>
        /// <returns></returns>
        public static bool CreateCommodityIndexTable()
        {
            bool success = false;

            try
            {
                success = Database.DatabaseConnection.CreateTable("ContractIndex", "Commodity varChar(10), Contract varChar(10)", "Commodity, Contract");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return success;
        }

        /// <summary>
        /// Get the max number in field Id
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static int GetMaxId(String tableName)
        {
            int maximum = -1;

            //define the connection reference and initialize it
            var msqlConnection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            //define the command reference
            var msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
            msqlCommand.Connection = msqlConnection;
            //define the command text
            msqlCommand.CommandText = String.Format(getMaxCommand, "Id", tableName);
            try
            {
                //open the connection
                msqlConnection.Open();
                //use a DataReader to process each record
                MySql.Data.MySqlClient.MySqlDataReader msqlReader = msqlCommand.ExecuteReader();

                while (msqlReader.Read())
                {
                    Console.WriteLine(msqlReader["Max(Id)"].ToString());
                }
                Console.ReadLine();
                return maximum;
            }
            catch (Exception er)
            {
                Console.WriteLine("Something wrong is going on...");
                Console.WriteLine(er.Message);
                Console.ReadLine();
                //do something with the exception
                return maximum;
            }
            finally
            {
                //always close the connection
                msqlConnection.Close();
            }
        }

        /**************************************************Private Functions Starts Here************************************************************/
        private static bool CheckContractTable(String contractCode)
        {
            bool exist = false;

            return exist;
        }

        /// <summary>
        /// Create a table with the given name and primary key
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="PrimaryKeyName"></param>
        private static bool CreateContractTable(String tableName, String PrimaryKeyName = null)
        {

            var commands = new List<String>();

            commands.Add(String.Format(createContractTableCommand, tableName));
            if (PrimaryKeyName != null)
            {
                commands.Add(String.Format(alterTableCommand, tableName, PrimaryKeyName));
            }

            return RunCommands(commands);
        }

        /// <summary>
        /// Insert a row to contract table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private static bool InsertRowToContractTable(String tableName, CandleStick row)
        {
            var command =
                String.Format(
                    insertRowToContractTableCommand,
                    tableName,
                    String.Format("\"{0}\"", row.EndTime),
                    row.HighP,
                    row.LowP,
                    row.OpenP,
                    row.CloseP,
                    row.Volume,
                    row.Amount,
                    String.Format("\"{0}\"", row.Period.TotalMinutes.ToString()),
                    String.Format("\"{0}\"", row.ContractCode));

            return RunCommand(command);
        }

        /// <summary>
        /// Drop a table with the given name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static bool DropTable(String tableName)
        {
            var command = String.Format(dropTableCommand, tableName);
            return RunCommand(command);
        }

        /// <summary>
        /// Create a table with a given name and command
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="command"></param>
        /// <param name="PrimaryKeyName"></param>
        /// <returns></returns>
        private static bool CreateTable(String tableName, String schema, String PrimaryKeyName = null)
        {
            try
            {
                var commands = new List<String>();

                commands.Add(String.Format("create table {0} ({1})", tableName, schema));
                if (PrimaryKeyName != null)
                {
                    commands.Add(String.Format(alterTableCommand, tableName, PrimaryKeyName));
                }

                return RunCommands(commands);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Insert a row into a table
        /// </summary>
        /// <returns></returns>
        private static bool InsertRow(String tableName, String schema, String row)
        {
            var command = String.Format("insert into {0} ({1}) values ({2})", tableName, schema, row);

            return RunCommand(command);
        }

        /// <summary>
        /// Run Database Commands
        /// </summary>
        /// <param name="icmd"></param>
        /// <param name="strCommend"></param>
        /// <returns></returns>
        private static bool ExecuteCommand(MySqlCommand icmd, string strCommend)
        {
            try
            {
                icmd.CommandText = strCommend;
                icmd.ExecuteNonQuery();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return false;
            }
            return true;

        }

        /// <summary>
        /// Run MySql command
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        private static bool RunCommand(String command)
        {
            //define the connection reference and initialize it
            var msqlConnection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            //define the command reference
            var msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
            //define the connection used by the command object
            msqlCommand.Connection = msqlConnection;

            try
            {
                msqlConnection.Open();

                bool check = ExecuteCommand(msqlCommand, command);
                Console.WriteLine("MySql Command \"{0}\" succeeded? {1}", command, check);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Creating Table: {0}", e.Message);
                return false;
            }
            finally
            {
                // Close the connection
                msqlConnection.Close();
            }
        }

        /// <summary>
        /// Run a list of  MySql commands in order
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        private static bool RunCommands(List<String> commands)
        {
            //define the connection reference and initialize it
            var msqlConnection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            //define the command reference
            var msqlCommand = new MySql.Data.MySqlClient.MySqlCommand();
            //define the connection used by the command object
            msqlCommand.Connection = msqlConnection;

            try
            {
                msqlConnection.Open();

                foreach (var command in commands)
                {
                    bool check = false;
                    if (command != null)
                    {
                        check = ExecuteCommand(msqlCommand, command);
                        Console.WriteLine("MySql Command \"{0}\" succeeded? {1}", command, check);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Creating Table: {0}", e.Message);
                return false;
            }
            finally
            {
                // Close the connection
                msqlConnection.Close();
            }
        }
    }
}
