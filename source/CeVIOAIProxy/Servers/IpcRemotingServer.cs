using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Lifetime;

namespace CeVIOAIProxy.Servers
{
    public class IpcRemotingServer : IDisposable
    {
        private IpcServerChannel _IpcServerChannel;

        private List<MarshalByRefObject> _RemotingObjects = new List<MarshalByRefObject>();

        private string _ChannelName;

        private bool _IsRunning;

        public string ChannelName => _ChannelName;

        public bool IsRunning => _IsRunning;

        public IpcRemotingServer(string sChannel)
        {
            try
            {
                _ChannelName = sChannel;
                _IpcServerChannel = new IpcServerChannel(sChannel, sChannel);
                ChannelServices.RegisterChannel(_IpcServerChannel, ensureSecurity: false);
                _IsRunning = true;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        ~IpcRemotingServer()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(disposing: true);
        }

        protected void Dispose(bool disposing)
        {
            if (_RemotingObjects != null)
            {
                foreach (MarshalByRefObject remotingObject in _RemotingObjects)
                {
                    RemotingServices.Disconnect(remotingObject);
                }
                _RemotingObjects = null;
            }
            if (_IpcServerChannel != null)
            {
                ChannelServices.UnregisterChannel(_IpcServerChannel);
                _IpcServerChannel = null;
            }
            _IsRunning = false;
        }

        public void RegisterRemotingObject(MarshalByRefObject mRemotingObj, string sUri)
        {
            RemotingServices.Marshal(mRemotingObj, sUri);
            _RemotingObjects.Add(mRemotingObj);
            var lease = (ILease)RemotingServices.GetLifetimeService(mRemotingObj);
            lease.Renew(new TimeSpan(999999, 0, 0, 0));
        }
    }
}
