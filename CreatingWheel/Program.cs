using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace CreatingWheel
{
    static class Program
    {
        /****************************************Constants Starts Here****************************************************/

        /// <summary>
        /// Folder containing all the past data
        /// </summary>
        static private String dataFolderName = @"F:\Data\1min";

        /***************************************Main Program Starts Here*****************************************************/
        /// <summary>
        /// Main program
        /// Read all the data files and save them to database
        /// </summary>
        static void Main()
        {
            // Create table Index
            // This table has to exist and no re-creation is allowed
            Database.DatabaseConnection.CreateCommodityIndexTable();

            // Traver the folder and find all data files
            var dataFileList = TraversDataFolder(dataFolderName);

            bool success = false;
            foreach (var filePath in dataFileList)
            {
                success = ProcessFile(filePath);
            }
        }

        /**************************************Helper Functions Starts Here******************************************************/
        /// <summary>
        /// Read a file, validate it, and save data into database
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static private bool ProcessFile(FileInfo file)
        {
            // Read data from file and compose candle sticks
            var candleStickList = IO.Parser.ReadFile(file);

            // Save candle sticks into database
            bool success = Database.DatabaseConnection.SaveCandleSticks(candleStickList);
            if (!success)
            {
                throw new Exception(String.Format("Error: save data from file {0} failed", file));
            }

            return success;
        }

        /// <summary>
        /// Travers a given folder and find all files in it
        /// Depth First Travers!
        /// </summary>
        /// <returns></returns>
        static private List<FileInfo> TraversDataFolder(String dataFolder)
        { 
            DirectoryInfo theFolder=new DirectoryInfo(dataFolder);

            var folderStack = new Stack<DirectoryInfo>();
            var fileList = new List<FileInfo>();

            folderStack.Push(theFolder);
            while (folderStack.Count != 0)
            {
                var folder = folderStack.Pop();
                foreach (var file in folder.GetFiles())
                {
                    fileList.Add(file);
                }
                
                foreach (var subfolder in folder.GetDirectories())
                {
                    folderStack.Push(subfolder);
                }
            }

            return fileList;
        }
    }
}
