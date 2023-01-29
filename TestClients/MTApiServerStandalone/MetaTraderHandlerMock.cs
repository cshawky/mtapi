using MTApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTApiServerStandalone
{
    internal class MetaTraderHandlerMock : IMetaTraderHandler
    {
        public void SendTickToMetaTrader(int handle)
        {
            // this will be called by mql5 via MT5Connector.cpp to dequeue the task
            var commandType = MtAdapter.GetInstance().GetCommandType(handle);

            // this will be called by mql5 via MT5Connector.cpp to send the response based on the given commandtype
            MtAdapter.GetInstance().SendResponse(handle, new MtResponseString($"just a description for command type {commandType}"));
        }
    }
}
