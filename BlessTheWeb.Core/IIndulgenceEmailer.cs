using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlessTheWeb.Core
{
    public interface IIndulgenceEmailer
    {
        void Send(Indulgence indulgence, string indulgenceFilePath);
    }
}
