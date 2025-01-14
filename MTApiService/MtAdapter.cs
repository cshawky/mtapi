﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using log4net;

namespace MTApiService
{
    public class MtAdapter
    {
        #region Fields
        private const string LogProfileName = "MtApiService";

        private static readonly ILog Log = LogManager.GetLogger(typeof(MtAdapter));
        private static readonly MtAdapter Instance = new MtAdapter();

        private readonly Dictionary<int, MtServer> _servers = new Dictionary<int, MtServer>();
        private readonly Dictionary<int, MtExpert> _experts = new Dictionary<int, MtExpert>();
        #endregion

        #region Init Instance

        private MtAdapter()
        {
            CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            LogConfigurator.Setup(LogProfileName);
            Log.Info($"Assembly version: {Assembly.GetExecutingAssembly().GetName().Version}");
            Log.Info($"Assembly location: {Assembly.GetExecutingAssembly().Location}");
            Log.Info($"Assembly was loaded from GAC: {Assembly.GetExecutingAssembly().GlobalAssemblyCache}");
        }
        
        static MtAdapter() 
        {
        }

        public static MtAdapter GetInstance()
        {
            return Instance;
        }

        #endregion


        #region Public Methods
        public void AddExpert(int port, MtExpert expert)
        {
            if (expert == null)
                throw new ArgumentNullException(nameof(expert));

            Log.InfoFormat($"AddExpert: begin. port = {port}, expert = {expert}");

            MtServer server;
            lock (_servers)
            {
                if (_servers.ContainsKey(port))
                {
                    server = _servers[port];
                }
                else
                {
                    server = new MtServer(port);
                    server.Stopped += server_Stopped;
                    _servers[port] = server;

                    server.Start();
                }
            }

            lock (_experts)
            {
                _experts[expert.Handle] = expert;
            }

            server.AddExpert(expert);
            expert.Deinited += ExpertOnDeinited;

            Log.Info("AddExpert: end");
        }

        public void RemoveExpert(int expertHandle)
        {
            Log.InfoFormat("RemoveExpert: begin. expertHandle = {0}", expertHandle);

            MtExpert expert = null;

            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert != null)
            {
                expert.Deinit();
            }
            else
            {
                Log.WarnFormat("RemoveExpert: expert with id {0} has not been found.", expertHandle);
            }

            Log.Info("RemoveExpert: end");
        }

        public void SendQuote(int expertHandle, string symbol, double bid, double ask)
        {
            Log.DebugFormat("UpdateQuote: begin. symbol = {0}, bid = {1}, ask = {2}", symbol, bid, ask);

            MtExpert expert;
            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert != null)
            {
                expert.UpdateQuote(new MtQuote { Instrument = symbol, Bid = bid, Ask = ask, ExpertHandle = expertHandle });
            }
            else
            {
                Log.WarnFormat("UpdateQuote: expert with id {0} has not been found.", expertHandle);
            }

            Log.Debug("UpdateQuote: end");
        }

        public void SendEvent(int expertHandle, int eventType, string payload)
        {
            Log.DebugFormat("SendEvent: begin. eventType = {0}, payload = {1}", eventType, payload);

            MtExpert expert;
            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert != null)
            {
                expert.SendEvent(new MtEvent { EventType = eventType, Payload = payload, ExpertHandle = expertHandle });
            }
            else
            {
                Log.WarnFormat("SendEvent: expert with id {0} has not been found.", expertHandle);
            }

            Log.Debug("SendEvent: end");
        }

        public void SendResponse(int expertHandle, MtResponse response)
        {
            Log.DebugFormat("SendResponse: begin. id = {0}, response = {1}", expertHandle, response);

            MtExpert expert;
            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert != null)
            {
                expert.SendResponse(response);
            }
            else
            {
                Log.WarnFormat("SendResponse: expert with id {0} has not been found.", expertHandle);
            }

            Log.Debug("SendResponse: end");
        }

        public int GetCommandType(int expertHandle)
        {
            Log.DebugFormat("GetCommandType: begin. expertHandle = {0}", expertHandle);

            MtExpert expert;
            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert == null)
            {
                Log.WarnFormat("GetCommandType: expert with id {0} has not been found.", expertHandle);
            }

            var retval = expert?.GetCommandType() ?? 0;

            Log.DebugFormat("GetCommandType: end. retval = {0}", retval);

            return retval;
        }

        public T GetCommandParameter<T>(int expertHandle, int index)
        {
            Log.DebugFormat("GetCommandParameter: begin. expertHandle = {0}, index = {1}", expertHandle, index);

            MtExpert expert;
            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert == null)
            {
                Log.WarnFormat("GetCommandParameter: expert with id {0} has not been found.", expertHandle);
            }

            var retval = expert?.GetCommandParameter(index);

            Log.DebugFormat("GetCommandParameter: end. retval = {0}", retval);

            return (T)retval;
        }

        public object GetNamedParameter(int expertHandle, string name)
        {
            Log.DebugFormat("GetNamedParameter: begin. expertHandle = {0}, name = {1}", expertHandle, name);

            MtExpert expert;
            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert == null)
            {
                Log.WarnFormat("GetNamedParameter: expert with id {0} has not been found.", expertHandle);
            }

            var retval = expert?.GetNamedParameter(name);

            Log.DebugFormat("GetNamedParameter: end. retval = {0}", retval);

            return retval;
        }

        public bool ContainsNamedParameter(int expertHandle, string name)
        {
            Log.DebugFormat("ContainsNamedParameter: begin. expertHandle = {0}, name = {1}", expertHandle, name);

            MtExpert expert;
            lock (_experts)
            {
                _experts.TryGetValue(expertHandle, out expert);
            }

            if (expert == null)
            {
                Log.WarnFormat("ContainsNamedParameter: expert with id {0} has not been found.", expertHandle);
            }

            bool retval = expert != null ? expert.ContainsNamedParameter(name) : false;

            Log.DebugFormat("ContainsNamedParameter: end. retval = {0}", retval);

            return retval;
        }

        public void LogError(string message)
        {
            Log.Error(message);
        }
        #endregion

        #region Private Methods
        private void server_Stopped(object sender, EventArgs e)
        {
            var server = (MtServer)sender;
            server.Stopped -= server_Stopped;

            var port = server.Port;

            Log.InfoFormat("server_Stopped: port = {0}", port);

            lock (_servers)
            {
                if (_servers.ContainsKey(port))
                {
                    _servers.Remove(port);
                }
            }
        }


        private void ExpertOnDeinited(object sender, EventArgs eventArgs)
        {
            Log.Debug("ExpertOnDeinited: begin.");

            var expert = sender as MtExpert;
            if (expert == null)
            {
                Log.Warn("expert_Deinited: end. Expert is not defined.");
                return;
            }

            lock (_experts)
            {
                if (_experts.ContainsKey(expert.Handle))
                {
                    _experts.Remove(expert.Handle);
                }
            }

            Log.DebugFormat("ExpertOnDeinited: removed expert {0}", expert.Handle);

            Log.Debug("ExpertOnDeinited: end.");
        }
        #endregion
    }
}
