using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;

namespace TeamMinerClassLibrary
{
    public class Tool : AbstractPersistenceMgr, IDataAccessObject
    {

        #region Class State
        private int toolID;
        private string toolName;
        private int toolLife;
        private int userID; 
        #endregion

        #region Properties
        public int ToolID
        {
            get { return toolID; }
            set { toolID = value; }
        }

        public string ToolName
        {
            get { return toolName; }
            set { toolName = value; }
        }

        public int ToolLife
        {
            get { return toolLife; }
            set { toolLife = value; }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        } 
        #endregion

        #region Create
        public void Create()
        {
            //Must be either ..
            //"StoneHammer" WHICH HAS LIFE OF 10
            //"StonePick" WHICH HAS LIFE OF 10
            //"IronHammer" WHICH HAS LIFE OF 20
            //"IronPick" WHICH HAS LIFE OF 20

            string sql = "insert into tool values(toolidautoseq.nextval, :toolname, :toollife, :userid)";

            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);

            cmd.CommandType = CommandType.Text;

            OracleParameter param1 = cmd.Parameters.Add("toolname", OracleDbType.Varchar2);
            param1.Direction = ParameterDirection.Input;
            param1.Value = ToolName;

            OracleParameter param2 = cmd.Parameters.Add("toollife", OracleDbType.Int16);
            param2.Direction = ParameterDirection.Input;
            param2.Value = ToolLife;

            OracleParameter param3 = cmd.Parameters.Add("userid", OracleDbType.Int16);
            param3.Direction = ParameterDirection.Input;
            param3.Value = UserID;

            ExecuteNonQuery(cmd, conn);

        } 
        #endregion

        #region Read
        public void Read()
        {
            // SQL statement
            String sql = "SELECT * FROM tool WHERE toolid = " + ToolID;
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                ToolID = (dr.IsNull("toolid")) ? -1 : (int)dr["toolid"];
                ToolName = (dr.IsNull("toolname")) ? null : (String)dr["toolname"];
                ToolLife = (dr.IsNull("toollife")) ? -1 : (Int16)dr["toollife"];
                UserID = (dr.IsNull("userid")) ? -1 : (int)dr["userid"];


            }
            else
            {
                System.Windows.Forms.MessageBox.Show("That tool does not exist");
            }
        }
        #endregion


        public bool Update()
        {
            return false;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public DataTable ReadAllToolAsDataTable()
        {
            if (UserID == 0)
            {
                return null;
            }
            else
            {
                // SQL statement
                String sql = "SELECT * FROM tool where userid = " + UserID;
                OracleConnection conn = Connect();
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                DataTable dt = ExecuteQuery(cmd, conn);

                return dt;
            }
        }
    }
}
