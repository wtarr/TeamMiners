using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Windows.Forms;

namespace TeamMinerClassLibrary
{
    public abstract class AbstractPersistenceMgr : IPersistenceMgr
    {

        #region Class State
        private OracleDataAdapter da;
        private OracleCommandBuilder cb;
        private DatabaseAccessInfo databaseAccessInfo; 
        #endregion
        
        /// <summary>
        /// Assembles and returns the connection information required
        /// </summary>
        /// <returns>OracleConnection</returns>
        #region Connect()
        public OracleConnection Connect()
        {
            // Instaniate the DatabaseAccessInfo
            databaseAccessInfo = (DatabaseAccessInfo)System.Configuration.ConfigurationManager.GetSection("DatabaseAccessInfo");

            OracleConnection conn = new OracleConnection("User Id=" + databaseAccessInfo.DBownerName +
                ";Password=" + databaseAccessInfo.DBownerPassword +
                ";Data Source=" + databaseAccessInfo.DBsource + ";Persist Security Info=True;");

            return conn;

        } 
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd">OracleCommand</param>
        /// <param name="conn">OracleConnection</param>
        /// <returns>DataTable</returns>
        #region ExecuteQuery
        public DataTable ExecuteQuery(OracleCommand cmd, OracleConnection conn)
        {

            try
            {

                OracleCommand mycmd = cmd;
                da = new OracleDataAdapter(mycmd);
                cb = new OracleCommandBuilder(da);
                DataSet ds = new DataSet();

                da.Fill(ds);
                DataTable dt = ds.Tables[0];
                return dt;

            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 12154:
                        MessageBox.Show("Check Data Source :- " + ex.Message.ToString());
                        Environment.Exit(0);
                        break;
                    case 12560:
                        MessageBox.Show("The database is not available");
                        Environment.Exit(0);
                        break;                   
                    case 1017:
                        MessageBox.Show("Invalid database login\n\nCheck the settings in the App.config - Now Exiting", "Error");
                        Environment.Exit(0);
                        break;
                    case 12514:
                        MessageBox.Show("TNS: Listener does not currently know of service requested in connect descriptor\n\nCheck that the Database has started - Now Exiting", "Error");
                        Environment.Exit(0);
                        break;
                    default:
                        MessageBox.Show("Unknown :- " + ex.Message.ToString());
                        Environment.Exit(0);
                        break;
                }

                return null;

            }
            finally
            {
                Dispose(conn);
            }
        } 
        #endregion

        /// <summary>
        /// Executes a non Query - Insert, Update or Delete
        /// </summary>
        /// <param name="cmd">OracleCommand</param>
        /// <param name="conn">OracleConnection</param>
        #region ExecuteNonQuery
        public bool ExecuteNonQuery(OracleCommand cmd, OracleConnection conn)
        {
            
            try
            {

                conn.Open();
                OracleCommand mycmd = cmd;
                mycmd.Connection = conn;
                mycmd.ExecuteNonQuery();
                return true;
            }
            catch (OracleException ex)
            {
                switch (ex.Number)
                {
                    case 12154:
                        MessageBox.Show("Check Data Source :- " + ex.Message.ToString());
                        break;
                    case 12560:
                        MessageBox.Show("The database is not available");
                        break;
                    case 1017:
                        MessageBox.Show("Invalid login");
                        break;
                    case 20000:
                        MessageBox.Show("That password does not contain a digit or is too short");
                        break;
                    case 20008:
                        MessageBox.Show("You lack the correct tool for mining this item");
                        break;
                    case 20007:
                        MessageBox.Show("You possess no tool to mine this item");
                        break;
                    case 20001:
                        MessageBox.Show("Instufficent inventory to make this tool");
                        break;
                    case 20002:
                        MessageBox.Show("Instufficent inventory to make make this tool");
                        break;
                    case 20003:
                        MessageBox.Show("Instufficent inventory to make make this tool");
                        break;
                        
                    case 20004:
                        MessageBox.Show("Instufficent inventory to make building");
                        break;
                    case 20006:
                        MessageBox.Show("That cell is occupied");
                        break;


                    default:
                        MessageBox.Show("? :- " + ex.Message.ToString());
                        break;
                }

                return false;

            }
            finally
            {
                Dispose(conn);                
            }
        } 
        #endregion

        /// <summary>
        /// Disposes of the current database connection
        /// </summary>
        /// <param name="conn">OracleConnection</param>
        #region Dispose
        private void Dispose(OracleConnection conn)
        {
            conn.Dispose();
        } 
        #endregion
                
    }
}
