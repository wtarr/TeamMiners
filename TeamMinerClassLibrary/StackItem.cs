using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;

namespace TeamMinerClassLibrary
{
    public class StackItem : AbstractPersistenceMgr, IDataAccessObject
    {
        #region Class State
        private int itemID;
        private int itemOrder;
        private string itemName;
        private int toolRequired;        
        private int xcoord;
        private int ycoord;
        private int userid = -1; // no user assigned
        #endregion

        #region Properties
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        public int ItemOrder
        {
            get { return itemOrder; }
            set { itemOrder = value; }
        }

        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public int ToolRequired
        {
            get { return toolRequired; }
            set { toolRequired = value; }
        }

        public int Xcoord
        {
            get { return xcoord; }
            set { xcoord = value; }
        }

        public int Ycoord
        {
            get { return ycoord; }
            set { ycoord = value; }
        }

        public int Userid
        {
            get { return userid; }
            set { userid = value; }
        } 
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            StackItem s = (StackItem)obj;
            if (this.Xcoord == s.Xcoord && this.Ycoord == s.Ycoord && this.itemOrder == s.ItemOrder)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        } 
        #endregion

        /// <summary>
        /// This allows a single stack item to be created
        /// </summary>
        public void Create()
        {
            string sql = "insert into stackitem values(itemidautoseq.nextval, :itemorder, :itemname, :toolrequired, :xcoord, :ycoord, null)";

            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);

            cmd.CommandType = CommandType.Text;

            OracleParameter param1 = cmd.Parameters.Add("itemorder", OracleDbType.Int16);
            param1.Direction = ParameterDirection.Input;
            param1.Value = ItemOrder;

            OracleParameter param2 = cmd.Parameters.Add("itemname", OracleDbType.Varchar2);
            param2.Direction = ParameterDirection.Input;
            param2.Value = ItemName;

            OracleParameter param3 = cmd.Parameters.Add("toolrequired", OracleDbType.Int16);
            param3.Direction = ParameterDirection.Input;
            param3.Value = ToolRequired;

            OracleParameter param4 = cmd.Parameters.Add("xcoord", OracleDbType.Int16);
            param4.Direction = ParameterDirection.Input;
            param4.Value = Xcoord;

            OracleParameter param5 = cmd.Parameters.Add("ycoord", OracleDbType.Int16);
            param5.Direction = ParameterDirection.Input;
            param5.Value = Ycoord;            

            ExecuteNonQuery(cmd, conn);
        }

        /// <summary>
        /// This allows the a single stack item of known coordinates and order to be returned as a StackItem object
        /// </summary>
        public void Read()
        {
            // SQL statement
            String sql = "SELECT * FROM stackitem WHERE xcoord = " + Xcoord + " AND ycoord = " + Ycoord + " AND itemorder = " + ItemOrder;
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];                              
                
                ItemName = (dr.IsNull("itemname")) ? null : (String)dr["itemname"];
                ToolRequired = (dr.IsNull("toolrequired")) ? -1 : (Int16)dr["toolrequired"];
                Xcoord = (dr.IsNull("xcoord")) ? -1 : (Int16)dr["xcoord"];
                Ycoord = (dr.IsNull("ycoord")) ? -1 : (Int16)dr["ycoord"];                
                Userid = (dr.IsNull("userid")) ? -1 : (int)dr["userid"];
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No such item");
            }
        }

        /// <summary>
        /// This allows the a single stack items UserID to be updated allowing the item to become the possession of that user
        /// </summary>
        public bool Update()
        {
            //Only every update the userid of the stack item !!
            String sql = "UPDATE stackitem SET userid = :userid WHERE (xcoord = " + Xcoord + " AND ycoord = " + Ycoord + " AND itemorder = " + ItemOrder + ")";
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);

            cmd.CommandType = CommandType.Text;
            
            OracleParameter param1 = cmd.Parameters.Add("userid", OracleDbType.Int16);
            param1.Direction = ParameterDirection.Input;
            param1.Value = Userid;

            
            return ExecuteNonQuery(cmd, conn);
            
        }

        /// <summary>
        /// This allows the stack of known coordinates to be deleted
        /// </summary>
        public void Delete()
        {
            String sql = "DELETE FROM stackitem WHERE xcoord = " + Xcoord + " AND ycoord = " + Ycoord;
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);

            cmd.CommandType = CommandType.Text;

            ExecuteNonQuery(cmd, conn);
        }

        /// <summary>
        /// This method returns all stack items at a known coordinate as a datatable which can be further iterated through.
        /// </summary>
        public DataTable FindAllStackItemsAtCoordinate()
        {
            // SQL statement
            String sql = "SELECT * FROM stackitem WHERE xcoord = " + Xcoord + " AND ycoord = " + Ycoord;
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            return dt;                       
        }

        /// <summary>
        /// This returns that last item of a stack, this is useful for finding where the lava block is located and also knowing the height of the stack
        /// </summary>
        public StackItem FindLastStackItemAtCoordinate()
        {
            // SQL statement
            String sql = "SELECT * FROM stackitem WHERE xcoord = " + Xcoord + " AND ycoord = " + Ycoord + " AND itemname = 'Lava'";
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            DataTable dt = ExecuteQuery(cmd, conn);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                
                StackItem lavaitem = new StackItem();
                
                lavaitem.ItemName = (dr.IsNull("itemname")) ? null : (String)dr["itemname"];
                lavaitem.ToolRequired = (dr.IsNull("toolrequired")) ? -1 : (Int16)dr["toolrequired"];
                lavaitem.Xcoord = (dr.IsNull("xcoord")) ? -1 : (Int16)dr["xcoord"];
                lavaitem.Ycoord = (dr.IsNull("ycoord")) ? -1 : (Int16)dr["ycoord"];
                lavaitem.Userid = (dr.IsNull("userid")) ? -1 : (int)dr["userid"];
                lavaitem.ItemOrder = (dr.IsNull("itemorder")) ? -1 : (Int16)dr["itemorder"];

                return lavaitem;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("No such item");
                return null;
            }        
        }
    }
}
