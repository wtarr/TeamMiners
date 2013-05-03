using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace TeamMinerClassLibrary
{
    public class Building : AbstractPersistenceMgr, IDataAccessObject
    {

        #region Class State
        private int buildingID;
        private int length;
        private int width;
        private int forSale;
        private int userID;
        private int xCoord;
        private int yCoord; 
        #endregion
        
        #region Properties
        public int BuildingID
        {
            get { return buildingID; }
            set { buildingID = value; }
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int ForSale
        {
            get { return forSale; }
            set { forSale = value; }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; }
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

        /// <summary>
        /// Creates a single building based on the information provided.
        /// </summary>
        #region Create
        public void Create()
        {
            string sql = "insert into building values(buildingIDAUTOSEQ.nextval, :length, :width, :forSale, :userID, :xCoord, :yCoord)";

            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);

            cmd.CommandType = CommandType.Text;

            OracleParameter param1 = cmd.Parameters.Add("length", OracleDbType.Int16);
            param1.Direction = ParameterDirection.Input;
            param1.Value = Length;

            OracleParameter param2 = cmd.Parameters.Add("width", OracleDbType.Int16);
            param2.Direction = ParameterDirection.Input;
            param2.Value = Width;

            OracleParameter param3 = cmd.Parameters.Add("forSale", OracleDbType.Int16);
            param3.Direction = ParameterDirection.Input;
            param3.Value = ForSale;

            OracleParameter param4 = cmd.Parameters.Add("userID", OracleDbType.Int16);
            param4.Direction = ParameterDirection.Input;
            param4.Value = userID;

            OracleParameter param5 = cmd.Parameters.Add("xCoord", OracleDbType.Int16);
            param5.Direction = ParameterDirection.Input;
            param5.Value = XCoord;

            OracleParameter param6 = cmd.Parameters.Add("yCoord", OracleDbType.Int16);
            param6.Direction = ParameterDirection.Input;
            param6.Value = YCoord;

            ExecuteNonQuery(cmd, conn);
        } 
        #endregion

        /// <summary>
        /// Reads a single building based on a BuildingID
        /// </summary>
        #region Read
        public void Read()
        {
            // SQL statement
            String sql = "SELECT * FROM building WHERE buildingid = " + BuildingID;
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                BuildingID = (dr.IsNull("buildingid")) ? -1 : (int)dr["buildingid"];
                Length = (dr.IsNull("length")) ? -1 : (int)dr["length"];
                Width = (dr.IsNull("width")) ? -1 : (int)dr["width"];
                ForSale = (dr.IsNull("forsale")) ? -1 : (int)dr["forsale"];
                UserID = (dr.IsNull("userid")) ? -1 : (int)dr["userid"];
                XCoord = (dr.IsNull("xcoord")) ? -1 : (Int16)dr["xcoord"];
                YCoord = (dr.IsNull("ycoord")) ? -1 : (Int16)dr["ycoord"];

            }
            else
            {
                System.Windows.Forms.MessageBox.Show("That building does not exist");
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

        /// <summary>
        /// Reads all the buildings stored in the building table that is owned by the user.
        /// </summary>
        /// <returns>Datatable</returns>
        #region ReadAllBuildingDataAsDataTable
        public DataTable ReadAllBuildingDataAsDataTable()
        {
            if (UserID == 0)
            {
                return null;
            }
            else
            {
                // SQL statement
                String sql = "SELECT * FROM building where userid = " + UserID;
                OracleConnection conn = Connect();
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                DataTable dt = ExecuteQuery(cmd, conn);

                return dt;


            }
        } 
        #endregion
    }
}
