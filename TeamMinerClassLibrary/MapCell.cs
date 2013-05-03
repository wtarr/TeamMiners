using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace TeamMinerClassLibrary
{
    public class MapCell : AbstractPersistenceMgr, IDataAccessObject
    {
        #region Class State
        private int xCoord;
        private int yCoord;
        private int currentblock;
        private int userid;
        private int buildingid; 
        #endregion

        #region Properties
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

        public int Currentblock
        {
            get { return currentblock; }
            set { currentblock = value; }
        }

        public int Userid
        {
            get { return userid; }
            set { userid = value; }
        }

        public int Buildingid
        {
            get { return buildingid; }
            set { buildingid = value; }
        } 
        #endregion

        /// <summary>
        /// This allows for an entry to be created in the MapCell table.
        /// </summary>
        public void Create()
        {
            string sql = "INSERT INTO mapcell VALUES(:xcoord, :ycoord, :currentblock, :userid, null)";

            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);

            cmd.CommandType = CommandType.Text;

            OracleParameter param1 = cmd.Parameters.Add("xcoord", OracleDbType.Int16);
            param1.Direction = ParameterDirection.Input;
            param1.Value = XCoord;

            OracleParameter param2 = cmd.Parameters.Add("ycoord", OracleDbType.Varchar2);
            param2.Direction = ParameterDirection.Input;
            param2.Value = YCoord;

            OracleParameter param3 = cmd.Parameters.Add("currentblock", OracleDbType.Int16);
            param3.Direction = ParameterDirection.Input;
            param3.Value = Currentblock;

            OracleParameter param4 = cmd.Parameters.Add("userid", OracleDbType.Int16);
            param4.Direction = ParameterDirection.Input;
            param4.Value = Userid;

            ExecuteNonQuery(cmd, conn);
        }

        /// <summary>
        /// This allows for information of a cell item to be read once the coordinates are provided.
        /// </summary>
        public void Read()
        {
            // SQL statement
            String sql = "SELECT * FROM mapcell WHERE xcoord = " + XCoord + " AND ycoord = " + YCoord;
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                                
                XCoord = (dr.IsNull("xcoord")) ? -1 : (Int16)dr["xcoord"];
                YCoord = (dr.IsNull("ycoord")) ? -1 : (Int16)dr["ycoord"];
                Currentblock = (dr.IsNull("currentblock")) ? -1 : (Int16)dr["currentblock"];
                Userid = (dr.IsNull("userid")) ? -1 : (Int16)dr["userid"];
                Buildingid = (dr.IsNull("buildingid")) ? -1 : (Int16)dr["buildingid"];
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No such position in the mapcell table");
            }
        }

        public bool Update()
        {

            return false;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public DataTable ReadAllMapCellItemsAsDataTable()
        {
            // SQL statement
            String sql = "SELECT * FROM mapcell";
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            return dt;

        }
    }
}
