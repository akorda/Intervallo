using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intervallo
{
    public interface IOptions
    {
        string AudioPath { get; set; }
        int ImageDurationInSeconds { get; set; }
        string Font { get; set; }
        IEnumerable<string> ImageFiles { get; set; }
    }
}
