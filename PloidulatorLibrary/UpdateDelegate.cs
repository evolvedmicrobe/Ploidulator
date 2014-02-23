using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ploidulator
{

    public delegate void ClusterProcessorUpdateHandler(ClusterProcessorUpdate updateInformation);

    public class ClusterProcessorUpdate
    {
        public int ReadsProcessed;
        public int UnMappedReads;
        public string CurrentCluster;
    }

}
