using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace TeamMinerClassLibrary
{
    public class AppConfigXmlSerializerSectionHandler: IConfigurationSectionHandler
    {        
        //http://www.codeproject.com/Articles/6730/Custom-Objects-From-the-App-Config-file
        //it allows for the necessary connection information such as username and password and datasource to
        //be kept and maintained in one single place that can be edited externally of the program.
       
            public object Create(
                 object parent,
                 object configContext,
                 System.Xml.XmlNode section)
            {
                XPathNavigator nav = section.CreateNavigator();
                string typename = (string)nav.Evaluate("string(@type)");
                Type t = Type.GetType(typename);
                XmlSerializer ser = new XmlSerializer(t);
                return ser.Deserialize(new XmlNodeReader(section));
            }

        
    }
}
