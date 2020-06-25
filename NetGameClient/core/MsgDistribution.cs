using System;
using System.Collections.Generic;
using System.Text;

namespace NetGameClient {
    // 消息分发
    class MsgDistribution {
        // 每帧处理消息的数量
        public int num = 15;
        // 消息列表
        public List<ProtocolBase> msgList = new List<ProtocolBase>();
        // 委托类型
        public delegate void Delegate(ProtocolBase protocol);
        // 事件监听表
        private Dictionary<string, Delegate> eventDict = new Dictionary<string, Delegate>();
        private Dictionary<string, Delegate> onceDict = new Dictionary<string, Delegate>();

        // Update
        public void Update() {
            for (int i = 0; i < num; i++) {
                if (msgList.Count > 0) {
                    DispatchMsgEvent(msgList[0]);
                    lock (msgList) {
                        msgList.RemoveAt(0);
                    }
                } else {
                    break;
                }
            }
        }

        // 消息分发
        // TODOCjc
        public void DispatchMsgEvent(ProtocolBase protocal) {
            string name = protocal.GetName();
            Console.WriteLine("分发处理消息" + name);
            if (eventDict.ContainsKey(name)) {
                eventDict[name](protocal);
            }
            if (onceDict.ContainsKey(name)) {
                onceDict[name](protocal);
                onceDict[name] = null;
                onceDict.Remove(name);
            }
        }
    }


}
