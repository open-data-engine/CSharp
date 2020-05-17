using System;
using System.Linq;

namespace OpenDataEngine.Adapter
{
    public class Json: Base
    {
        protected override void Emit(String command, dynamic? arguments)
        {
            throw new NotImplementedException();
        }

        public override (String Command, Object[] Arguments) Translate(IAsyncQueryable query)
        {
            throw new NotImplementedException();
        }
    }
}
