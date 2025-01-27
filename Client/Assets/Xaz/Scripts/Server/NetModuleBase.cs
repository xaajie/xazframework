//------------------------------------------------------------
// Xaz Framework
// 模块协议的基类
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Xaz
{
    public class NetModuleBase
    {
        private Dictionary<int, Action<INetData>> protocols = new Dictionary<int, Action<INetData>>();
        public NetModuleBase()
        {

        }

        protected void Register(ProtocolEnum protocolID, Action<INetData> handler)
        {
            protocols.Add((int)protocolID, handler);
            NetCenter.Instance.RegisterProtocolHandler((int)protocolID, this);
        }

        public void Request(ProtocolEnum protocolID, INetData vt)
        {
            NetCenter.Instance.SendProtocolHandler((int)protocolID, vt);
        }

        public void OnResultHandler(int resId, INetData vt)
        {
            if (protocols.ContainsKey(resId))
            {
                protocols[resId].Invoke(vt);
            }
        }

        public virtual void Release()
        {
            protocols.Clear();
        }
    }
}
