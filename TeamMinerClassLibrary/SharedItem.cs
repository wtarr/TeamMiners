using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMinerClassLibrary
{
    public class SharedItem : AbstractPersistenceMgr, IDataAccessObject
    {
        #region Class State
        private int userID;
        private int inventoryID;
        private int sharedWithUser; 
        #endregion

        #region Properties
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }

        public int InventoryID
        {
            get { return inventoryID; }
            set { inventoryID = value; }
        }

        public int SharedWithUser
        {
            get { return sharedWithUser; }
            set { sharedWithUser = value; }
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
            return false;
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
