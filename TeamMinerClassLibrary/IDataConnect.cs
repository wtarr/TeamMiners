using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace TeamMinerClassLibrary
{
    public interface IPersistenceMgr
    {
        
        DataTable ExecuteQuery(OracleCommand cmd, OracleConnection conn);

        bool ExecuteNonQuery(OracleCommand cmd, OracleConnection conn);

    }
}
