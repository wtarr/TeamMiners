using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;

namespace TeamMinerClassLibrary
{
    public class InventoryItem: AbstractPersistenceMgr, IDataAccessObject
    {

        #region Class state
        private int inventoryID;
        private string itemName;
        private int isSharable;
        private int userID;
        private int toolID;
        private int buildingID; 
        #endregion

        #region Properties
        public int InventoryID
        {
            get { return inventoryID; }
            set { inventoryID = value; }
        }

        public string ItemName
        {
            get { return itemName; }
            set { itemName = value; }
        }

        public int IsSharable
        {
            get { return isSharable; }
            set { isSharable = value; }
        }

        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public int ToolID
        {
            get { return toolID; }
            set { toolID = value; }
        }

        public int BuildingID
        {
            get { return buildingID; }
            set { buildingID = value; }
        } 
        #endregion

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Read()
        {
            throw new NotImplementedException();
        }

        public bool Update()
        {
            //Only every update the userid of the stack item !!
            String sql = "UPDATE inventoryitem SET isshareable = " + IsSharable + " WHERE userid = " + UserID + " AND inventoryID = " + InventoryID;
            OracleConnection conn = Connect();
            OracleCommand cmd = new OracleCommand(sql, conn);

            cmd.CommandType = CommandType.Text;
                        
            return ExecuteNonQuery(cmd, conn);
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public DataTable ReadAllInventoryAsADataTable()
        {
            if (UserID == 0)
            {
                return null;
            }
            else
            {
                // SQL statement
                String sql = "SELECT * FROM inventoryitem where userid = " + UserID;
                OracleConnection conn = Connect();
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                DataTable dt = ExecuteQuery(cmd, conn);

                return dt;


            }
        }
        
    }
}
