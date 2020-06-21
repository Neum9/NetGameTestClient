using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetGameTestClient {
    class Program {
        const int BUFFER_SIZE = 1024;
        Socket socket;
        byte[] readBuff;

        int buffCount = 0;
        byte[] lenBytes = new byte[sizeof(UInt32)];
        Int32 msgLength = 0;
        private string recvStr;

        ProtocolBase proto = new ProtocolBytes();

        static void Main(string[] args) {
            //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ////Connect
            //string host = "127.0.0.1";
            //int port = 1234;
            //byte[] readBuff = new byte[BUFFER_SIZE];
            //socket.Connect(host, port);
            //string str = "Hello Unity!";
            //byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
            //socket.Send(bytes);
            ////Recv
            //int count = socket.Receive(readBuff);
            //str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
            //Console.WriteLine("recv " + str);
            //socket.Close();
            //Console.ReadKey();

            Program p = new Program();
            p.Connection();
            //byte[] bytes = System.Text.Encoding.Default.GetBytes("_GET");
            //p.socket.Send(bytes);


            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("HeartBeat");
            Console.WriteLine("发送" + protocol.GetDesc());
            p.Send(protocol);

            p.OnLoginClick();
            p.OnAddClick();
            p.OnGetClick();

            Console.ReadKey();
        }

        public void Connection() {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Connect
            string host = "127.0.0.1";
            int port = 1234;
            socket.Connect(host, port);
            readBuff = new byte[BUFFER_SIZE];
            socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
        }

        private void ReceiveCb(IAsyncResult ar) {
            try {
                //count是接受数据的大小
                int count = socket.EndReceive(ar);
                //数据处理
                //string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                //Console.WriteLine("recv " + str);
                buffCount += count;
                ProcessData();
                socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
            }
            catch (Exception e) {
                Console.WriteLine("连接已断开");
                socket.Close();
            }
        }

        private void ProcessData() {
            //小于长度字节
            if (buffCount < sizeof(Int32)) {
                return;
            }
            //消息长度
            Array.Copy(readBuff, lenBytes, sizeof(Int32));
            msgLength = BitConverter.ToInt32(lenBytes, 0);
            if (buffCount < msgLength + sizeof(Int32)) {
                return;
            }
            //处理消息
            //string str = System.Text.Encoding.UTF8.GetString(readBuff, sizeof(Int32), (int)msgLength);
            ProtocolBase protocol = proto.Decode(readBuff, sizeof(Int32), msgLength);
            HandleMsg(protocol);
            //recvStr = str;
            //清除已经处理的消息
            int count = buffCount - msgLength - sizeof(Int32);
            Array.Copy(readBuff, msgLength, readBuff, 0, count);
            buffCount = count;
            if (buffCount > 0) {
                ProcessData();
            }
        }

        private void HandleMsg(ProtocolBase protoBase) {
            ProtocolBytes proto = (ProtocolBytes)protoBase;
            //获取数值
            int start = 0;
            string protoName = proto.GetString(start, ref start);
            int ret = proto.GetInt(start, ref start);
            //显示       
            Console.WriteLine("接收" + proto.GetDesc());
            recvStr = " 接收 " + proto.GetName() + " " + ret.ToString();
        }

        private void Send(ProtocolBytes protocol) {
            //string str = "i am client";
            //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            //byte[] length = BitConverter.GetBytes(bytes.Length);
            //byte[] sendBuff = length.Concat(bytes).ToArray();
            //socket.Send(sendBuff);            
            byte[] bytes = protocol.Encode();
            byte[] length = BitConverter.GetBytes(bytes.Length);
            byte[] sendBuff = length.Concat(bytes).ToArray();

            //ProtocolBase pb1 = new ProtocolBytes();
            //ProtocolBase pb = pb1.Decode(sendBuff, sizeof(Int32), bytes.Length);

            socket.Send(sendBuff);
        }

        public void OnLoginClick() {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("Login");
            protocol.AddString("Cjc");
            protocol.AddString("123");
            Console.WriteLine(" 发送 " + protocol.GetDesc());
            Send(protocol);
        }

        public void OnAddClick() {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("AddScore");
            Console.WriteLine(" 发送 " + protocol.GetDesc());
            Send(protocol);
        }

        public void OnGetClick() {
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.AddString("GetScore");
            Console.WriteLine(" 发送 " + protocol.GetDesc());
            Send(protocol);
        }
    }
}
