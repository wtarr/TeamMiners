using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMinerClassLibrary
{
    public class DatabaseAccessInfo: AppConfigXmlSerializerSectionHandler
    {
        public string DBownerName;
        public string DBownerPassword;
        public string DBsource;
    }
}
