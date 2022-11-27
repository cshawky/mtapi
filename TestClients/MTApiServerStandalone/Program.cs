using MTApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTApiServerStandalone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var handlerMock = new MetaTraderHandlerMock();
            Mt5Expert expert = new Mt5Expert(99, "[ANY100]", 10000, 9999, handlerMock, true);
            MtAdapter.GetInstance().AddExpert(8228, expert);
            Console.ReadLine();
        }
    }
}
