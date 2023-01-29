using MTApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTApiServerStandalone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var handlerMock = new MetaTraderHandlerMock();

            Console.WriteLine("Starting server...");

            var mtApiServiceAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "MTApiService");
            if (mtApiServiceAssembly.GlobalAssemblyCache)
            {
                Console.WriteLine("WARNING: MtApiService dependency is loaded from GAC and may not contain local changes.");
            }

            Task.Factory.StartNew(() =>
            {
                Mt5Expert expert = new Mt5Expert(0, "[ANY100]", 10000, 9999, handlerMock, true);                
                MtAdapter.GetInstance().AddExpert(8222, expert);
                Console.WriteLine("Server started");
            });
            Console.ReadLine();
        }
    }

    class CustomMt5Expert : Mt5Expert
    {
        public CustomMt5Expert(int handle, string symbol, double bid, double ask, IMetaTraderHandler mtHandler, bool isTestMode) : base(handle, symbol, bid, ask, mtHandler, isTestMode)
        {
        }
    }
}
