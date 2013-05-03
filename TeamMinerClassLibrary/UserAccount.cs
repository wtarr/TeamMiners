using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;
using System.IO;

namespace TeamMinerClassLibrary
{
    public class UserAccount : AbstractPersistenceMgr, IDataAccessObject
    {
        #region Class State
        private byte[] userskin;
        private int userID;
        private string userName;
        private string password;
        private int onlineStatus;
        private int xCoord;
        private int yCoord; 
        #endregion
               
        #region Properties       
        
        public byte[] Userskin
        {
            get { return userskin; }
            set { userskin = value; }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public int OnlineStatus
        {
            get { return onlineStatus; }
            set { onlineStatus = value; }
        }

        public int XCoord
        {
            get { return xCoord; }
            set { xCoord = value; }
        }

        public int YCoord
        {
            get { return yCoord; }
            set { yCoord = value; }
        }

        #endregion

        
        public void Create()
        {
            //Following a combination of tutorials given here at code project and StackOverflow
            //it is possible to add a byte array as a blob into the Oracle db
            //http://www.codeproject.com/Articles/48619/Reading-and-Writing-BLOB-Data-to-Microsoft-SQL-or
            //http://stackoverflow.com/questions/4902250/insert-blob-in-oracle-database-with-c-sharp
            
            string sql = "INSERT INTO useraccount VALUES (:myblob, userIDAUTOSEQ.NEXTVAL, '" + UserName + "', '" + Password + "', 0, null, null)";

            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);         
            
            cmd.CommandType = CommandType.Text;

            // Bind to a parameter
            //http://docs.oracle.com/html/A96160_01/features.htm#1049336
            //http://docs.oracle.com/cd/B28359_01/appdev.111/b28844/building_odp.htm#CEGCGDAB
            OracleParameter param = cmd.Parameters.Add("myblob", OracleDbType.Blob);
            param.Direction = ParameterDirection.Input;

            // Assign Byte Array to Oracle Parameter
            param.Value = Userskin;
            
            ExecuteNonQuery(cmd, conn);                    
                    
        }        

        public void Read()
        {
            // SQL statement
            String sql = "SELECT * FROM useraccount WHERE username = '" + UserName + "'";
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);            
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                Userskin = (dr.IsNull("userskin")) ? null : (byte[])dr["userskin"];
                UserID =   (dr.IsNull("userid")) ? -1 : (int)dr["userid"];
                UserName = (dr.IsNull("username")) ? null : (String)dr["username"];                                      
                Password = (dr.IsNull("password")) ? null : (String)dr["password"];
                OnlineStatus = (dr.IsNull("onlinestatus")) ? (int)3 : (Int16)dr["onlinestatus"];
                XCoord = (dr.IsNull("xcoord")) ? -1 : (Int16)dr["xcoord"];
                YCoord = (dr.IsNull("ycoord")) ? -1 : (Int16)dr["ycoord"];                              
               
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The user does not exist");
            }
        }                     

        public bool Update()
        {
            if (UserID > 0)
            {
                string sql = "UPDATE useraccount SET userskin = :blob, username = :username, password = :password, onlinestatus = :onlinestatus WHERE userid = " + UserID;

                OracleConnection conn = Connect();
                OracleCommand cmd = new OracleCommand(sql, conn);

                cmd.CommandType = CommandType.Text;
                                
                OracleParameter param1 = cmd.Parameters.Add("blob", OracleDbType.Blob);
                param1.Direction = ParameterDirection.Input;                
                param1.Value = Userskin;

                OracleParameter param2 = cmd.Parameters.Add("username", OracleDbType.Varchar2);
                param2.Direction = ParameterDirection.Input;
                param2.Value = UserName;

                OracleParameter param3 = cmd.Parameters.Add("password", OracleDbType.Varchar2);
                param3.Direction = ParameterDirection.Input;
                param3.Value = Password;

                OracleParameter param4 = cmd.Parameters.Add("onlinestatus", OracleDbType.Int16);
                param4.Direction = ParameterDirection.Input;
                param4.Value = OnlineStatus;

                return ExecuteNonQuery(cmd, conn);
            }

            return false;
        }

        public void Delete()
        {
            if (UserID > 0)
            {
                string sql = "DELETE FROM useraccount WHERE username = " + UserID;

                OracleConnection conn = Connect();
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                ExecuteNonQuery(cmd, conn);
            }
        }

        public DataTable ReadAllSecureUsersAsADataTable()
        {
            // SQL statement
            String sql = "SELECT * FROM secureuserview";
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            return dt;
        }
    }
}
