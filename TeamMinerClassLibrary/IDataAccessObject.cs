using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamMinerClassLibrary
{
    public interface IDataAccessObject
    {
        void Create();

        void Read();

        bool Update();

        void Delete();
    }
}
