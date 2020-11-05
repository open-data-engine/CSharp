using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDataEngine.Schema
{
    public class Default : Base
    {
        public override String ResolvePath(String path)
        {
            return path;
        }
    }
}
