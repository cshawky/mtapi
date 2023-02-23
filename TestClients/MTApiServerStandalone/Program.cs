﻿using MTApiService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

            CancellationTokenSource sendTicksToken = new CancellationTokenSource();

            var tickThread = Task.Factory.StartNew(() =>
            {
                var time = 1658344860.0;
                while (!sendTicksToken.IsCancellationRequested)
                {
                    var bid = new Random().NextDouble() * 3000 + 1000;
                    var ask = new Random().NextDouble() * 3000 + 1000;
                    time += 10;

                    var payload = "{\"ExpertHandle\" : 0,\"Instrument\" : \"[ANY100]\",\"Tick\" : {\"bid\" : " + bid.ToString(CultureInfo.InvariantCulture) + ",\"MtTime\" : " + time.ToString(CultureInfo.InvariantCulture) + ",\"last\" : 13291.67,\"ask\" : " + ask.ToString(CultureInfo.InvariantCulture) + ",\"volume\" : 0,\"volume_real\" : 0}}";

                    MtAdapter.GetInstance().SendEvent(0, 3 /*Mt5EventTypes.OnTick*/, payload);
                    sendTicksToken.Token.WaitHandle.WaitOne(5000);
                }
            });

            Console.ReadLine();
            sendTicksToken.Cancel();
        }
    }

    class CustomMt5Expert : Mt5Expert
    {
        public CustomMt5Expert(int handle, string symbol, double bid, double ask, IMetaTraderHandler mtHandler, bool isTestMode) : base(handle, symbol, bid, ask, mtHandler, isTestMode)
        {
        }
    }
}
