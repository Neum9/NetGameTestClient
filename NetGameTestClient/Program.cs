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
            byte[] bytes = System.Text.Encoding.Default.GetBytes("_GET");
            p.socket.Send(bytes);

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
                string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
                Console.WriteLine("recv " + str);
                socket.BeginReceive(readBuff, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCb, null);
            } catch (Exception e) {
                Console.WriteLine("连接已断开");
                socket.Close();
            }
        }
    }
}
