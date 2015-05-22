using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CreatingWheel.Database
{
    class ContractIndexTable : DatabaseConnection
    {
        /// <summary>
        /// Create the ContractIndex table
        /// </summary>
        /// <returns></returns>
        public static bool CreateContractIndexTable()
        {
            string sqlCmd = "CREATE TABLE IF NOT EXISTS futures.ContractIndex (ContractCode VARCHAR(5), DeliveryMonth VARCHAR(2), PRIMARY KEY (ContractCode, DeliveryMonth));";
            
            return RunCommand(sqlCmd);
        }

        /// <summary>
        /// Insert a row into the table
        /// </summary>
        /// <param name="contractName"></param>
        /// <param name="deliveryMonth"></param>
        /// <returns></returns>
        public static bool InsertRow(String contractName, String deliveryMonth)
        {
            string sqlCmd = "INSERT INTO futures.ContractIndex (ContractCode, DeliveryMonth) VALUES (\"{0}\", \"{1}\");";

            return RunCommand(String.Format(sqlCmd, contractName, deliveryMonth));
        }

        /// <summary>
        /// Check whether the contract exists
        /// </summary>
        /// <returns></returns>
        public static bool ContractTableExists(String contractName, String deliveryMonth)
        {
            string sqlCmd = String.Format("SELECT * FROM futures.ContractIndex WHERE (ContractCode=\"{0}\" AND DeliveryMonth=\"{1}\");", contractName, deliveryMonth);

            var objConn = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
            objConn.Open();

            try
            {
                var daAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(sqlCmd, objConn);
                var dsContracts = new DataSet("Contracts");
                daAdapter.FillSchema(dsContracts, SchemaType.Source, "Contracts");
                daAdapter.Fill(dsContracts, "Contracts");

                var tblContracts = dsContracts.Tables["Contracts"];

                return (tblContracts.Rows.Count > 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error: CheckContractTableExistance - {0}", e.Message));
                return false;
            }
            finally
            {
                objConn.Close();
            }
        }
    }
}
