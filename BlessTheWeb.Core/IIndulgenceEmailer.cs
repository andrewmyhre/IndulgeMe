using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlessTheWeb.Core
{
    public interface IIndulgenceEmailer
    {
        Task Send(Indulgence indulgence, string indulgenceFilePath);
    }
}
