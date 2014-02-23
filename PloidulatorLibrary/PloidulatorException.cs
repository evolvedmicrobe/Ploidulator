using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator
{
    public class PloidulatorException : Exception
    {
        public PloidulatorException(string msg) : base(msg) { }
        public PloidulatorException(string msg, Exception innerException) : base(msg, innerException) { }
    }
}
