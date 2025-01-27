//------------------------------------------------------------
// Xaz Framework
// 通讯管理中心
// 主要这只是个桥接器，根据项目需要去决定通讯机制
// Feedback: qq515688254
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xaz
{
    public class NetCenter : Singleton<NetCenter>
    {

        private Dictionary<int, NetModuleBase> protocolHandlers;
        /// <summary>
        /// 使用前端模拟服务器
        /// </summary>
        public bool UserClientServer = true;

        public NetCenter()
        {
            protocolHandlers = new Dictionary<int, NetModuleBase>();
        }

        public void Begin()
        {
            ClientServerCenter.Instance.Begin();
        }

        public void RegisterProtocolHandler(int vt, NetModuleBase moudle)
        {
            protocolHandlers[vt] = moudle;
        }

        public void SendProtocolHandler(int vt, INetData data)
        {
            if (UserClientServer)
            {
                ClientServerCenter.Instance.SendProtocolHandler(vt, data);
            }
        }

        public void OnProtocolReceived(int vt, INetData data)
        {
            if(data.retcode != (int)ErrorCode.OK)
            {
                Debug.Log("ssssssssssssssssss"+ data.retcode);
                return;
            }
            if (protocolHandlers.ContainsKey(vt))
            {
                NetModuleBase moudle = protocolHandlers[vt];
                moudle.OnResultHandler(vt, data);
            }
        }

        public void RemoveAllProtocol()
        {
            protocolHandlers.Clear();
        }
    }
}
