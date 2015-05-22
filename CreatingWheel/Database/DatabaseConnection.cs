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
        public static string connectionString = "server=127.0.0.1;user id=root;Password=qwerqwer;database=futures";
        /**************************************************Public Functions Starts Here************************************************************/
        /// <summary>
        /// Run MySql command
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        protected static bool RunCommand(String command)
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
                Console.WriteLine("Error in Running Command: {0}", e.Message);
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
        protected static bool RunCommands(List<String> commands)
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
                Console.WriteLine("Error in Running Commands: {0}", e.Message);
                return false;
            }
            finally
            {
                // Close the connection
                msqlConnection.Close();
            }
        }
        /**************************************************Private Functions Starts Here************************************************************/
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
    }
}
