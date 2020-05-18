using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace FlightCenter
{
    /// <summary>
    /// This class holds global variables that is used by the application.
    /// Most of these variables are stored in the resources file, and some are in the Web.Config file.
    /// </summary>
    public static class GlobalConfig
    {
        public static readonly string adminUserName = Properties.Resources.admin_username;
        public static readonly string adminPassword = Properties.Resources.admin_password;
        public static readonly string databaseExceptionMessage = Properties.Resources.database_exception_message;
        public static readonly string invalidTokenMessage = Properties.Resources.invalid_token_message;
        public static readonly string wrongPasswordMessage = Properties.Resources.wrong_password_message;
        public static string connString;
        public static DateTime flightsArchivingTime;

        static GlobalConfig()
        {
            InitializeFields();
        }

        /// <summary>
        /// This method handles the initialization of some of the fields from the config files.
        /// At first it will try to load the data from the Web.Config file (In case this class is called by the Web API).
        /// If it fails, it tries the to load the data from the App.Config file (In case this class is called by the DB Generator).
        /// </summary>
        static void InitializeFields()
        {
            try
            {
                connString = WebConfigurationManager.AppSettings["ConnectionString"];
                flightsArchivingTime = DateTime.Parse(WebConfigurationManager.AppSettings["FlightsArchivingTime"]);

            }
            catch
            {
                connString = ConfigurationManager.AppSettings["ConnectionString"];
                flightsArchivingTime = DateTime.Parse(ConfigurationManager.AppSettings["FlightsArchivingTime"]);
            }
        }
    }
}
