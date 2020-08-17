
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Globalization;

namespace SCM_Protocol_10B
{
    public class Soc_Client_10B
    {
        public string IPAdress;
        public bool connected = false;
        public Socket clientSocket;
        private IPEndPoint hostEndPoint;
        private AutoResetEvent autoConnectEvent = new AutoResetEvent(false);
        private SocketAsyncEventArgs lisnterSocketAsyncEventArgs;

        public delegate void StartListeHandler();
        public event StartListeHandler StartListen;

        public delegate void ReceiveMsgHandler(byte[] info);
        public event ReceiveMsgHandler OnMsgReceived;

        private List<SocketAsyncEventArgs> s_lst = new List<SocketAsyncEventArgs>();

        public Soc_Client_10B(string hostName, int port)
        {
            IPAdress = hostName;
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
            this.hostEndPoint = new IPEndPoint(hostAddresses[hostAddresses.Length - 1], port);
            this.clientSocket = new Socket(this.hostEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        /// <summary>
        /// 连接服务端
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            using (SocketAsyncEventArgs args = new SocketAsyncEventArgs())
            {
                args.UserToken = this.clientSocket;
                args.RemoteEndPoint = this.hostEndPoint;
                args.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnConnect);
                this.clientSocket.ConnectAsync(args);
                bool flag = autoConnectEvent.WaitOne(1000);
                //SocketError err = args.SocketError;
                if (this.connected)
                {
                    this.lisnterSocketAsyncEventArgs = new SocketAsyncEventArgs();
                    byte[] buffer = new byte[50];
                    this.lisnterSocketAsyncEventArgs.UserToken = this.clientSocket;
                    this.lisnterSocketAsyncEventArgs.SetBuffer(buffer, 0, buffer.Length);
                    this.lisnterSocketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnReceive);
                    this.StartListen();
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 判断有没有连接上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnect(object sender, SocketAsyncEventArgs e)
        {
            this.connected = (e.SocketError == SocketError.Success);
            autoConnectEvent.Set();
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="mes"></param>
        public void Send(Byte[] mes)
        {
            if (this.connected)
            {
                EventHandler<SocketAsyncEventArgs> handler = null;
                byte[] buffer = mes;
                SocketAsyncEventArgs senderSocketAsyncEventArgs = null;
                lock (s_lst)
                {
                    if (s_lst.Count > 0)
                    {
                        senderSocketAsyncEventArgs = s_lst[s_lst.Count - 1];
                        s_lst.RemoveAt(s_lst.Count - 1);
                    }
                }
                if (senderSocketAsyncEventArgs == null)
                {
                    senderSocketAsyncEventArgs = new SocketAsyncEventArgs();
                    senderSocketAsyncEventArgs.UserToken = this.clientSocket;
                    senderSocketAsyncEventArgs.RemoteEndPoint = this.clientSocket.RemoteEndPoint;
                    if (handler == null)
                    {
                        handler = delegate(object sender, SocketAsyncEventArgs _e)
                        {
                            lock (s_lst)
                            {
                                s_lst.Add(senderSocketAsyncEventArgs);
                            }
                        };
                    }
                    senderSocketAsyncEventArgs.Completed += handler;
                }
                senderSocketAsyncEventArgs.SetBuffer(buffer, 0, buffer.Length);
                this.clientSocket.SendAsync(senderSocketAsyncEventArgs);
            }
            else
            {
                this.connected = false;
            }
        }
        /// <summary>
        /// 监听服务端
        /// </summary>
        public void Listen()
        {
            if (this.connected && this.clientSocket != null)
            {
                try
                {
                    (lisnterSocketAsyncEventArgs.UserToken as Socket).ReceiveAsync(lisnterSocketAsyncEventArgs);
                }
                catch (Exception)
                {
                }
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        private int Disconnect()
        {
            int res = 0;
            try
            {
                this.clientSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
            }
            try
            {
                this.clientSocket.Close();
            }
            catch (Exception)
            {
            }
            this.connected = false;
            return res;
        }
        /// <summary>
        /// 数据接受
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReceive(object sender, SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred == 0)
            {
                try
                {
                    this.clientSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception)
                {
                }
                finally
                {
                    if (this.clientSocket.Connected)
                    {
                        this.clientSocket.Close();
                    }
                }
                byte[] info = new Byte[] { 0 };
                this.OnMsgReceived(info);
            }
            else
            {
                byte[] buffer = new byte[e.BytesTransferred];
                for (int i = 0; i < e.BytesTransferred; i++)
                {
                    buffer[i] = e.Buffer[i];
                }
                this.OnMsgReceived(buffer);
                Listen();
            }
        }
        /// <summary>
        /// 接受完成
        /// </summary>
        /// <param name="info"></param>
        private void SimaticSocketClient_OnMsgReceived(byte[] info)
        {
            if (info[0] != 0)
            {
                for (byte len = 0; len < info.Length; len++)
                {
                    if (Offset < 10)
                    {
                        RecvData_Temp[Offset] = info[len];
                        Offset++;
                    }
                }
            }
            else
            {
                RecvData_Temp[0] = 0;
                RecvData_Temp[1] = 0;
                RecvData_Temp[2] = 0;
                RecvData_Temp[3] = 0;
                RecvData_Temp[4] = 0;
                RecvData_Temp[5] = 0;
                RecvData_Temp[6] = 0;
                RecvData_Temp[7] = 0;
                RecvData_Temp[8] = 0;
                RecvData_Temp[9] = 0;
                Offset = 0;
            }
        }
        /// <summary>
        /// 建立连接的方法
        /// </summary>
        /// <returns></returns>
        public bool OpenLinkPLC()
        {
            bool flag = false;
            this.StartListen += new StartListeHandler(SimaticSocketClient_StartListen);
            this.OnMsgReceived += new ReceiveMsgHandler(SimaticSocketClient_OnMsgReceived);
            flag = this.Connect();
            if (!flag)
            {
                return flag;
            }
            return true;
        }

        /// <summary>
        /// 关闭连接的方法
        /// </summary>
        /// <returns></returns>
        public int CloseLinkPLC()
        {
            return this.Disconnect();
        }
        /// <summary>
        /// 监听的方法
        /// </summary>
        private void SimaticSocketClient_StartListen()
        {
            this.Listen();
        }

        #region
        /// <summary>
        /// agv放行
        /// </summary>
        /// <returns></returns>
        public int AgvGo()
        {
            int flag = -1;
            byte[] data = new byte[10];
            data[0] = 0xF3;
            data[1] = 0x1B;//第二位放行1B
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0xCF;
            Thread.Sleep(100);
            int numPro = 0;
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[7] = 0;
            this.Send(data);
            while ((RecvData_Temp[0] == 0 || RecvData_Temp[9] == 0) && numPro < 150)
            {
                Thread.Sleep(5);
                numPro++;
            }
            if (numPro < 150)
            {
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[1] == 0x1C&& RecvData_Temp[2] == 0x02 && RecvData_Temp[3] == 0x01)
                {
                    flag = 0;//成功
                }
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[2] == 0x01 && RecvData_Temp[3] == 0x00 && RecvData_Temp[9] == 0xCF)
                {
                    flag = 1;//失败
                }
            }
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[1] = 0;
            RecvData_Temp[2] = 0;
            RecvData_Temp[3] = 0;
            RecvData_Temp[4] = 0;
            RecvData_Temp[5] = 0;
            RecvData_Temp[6] = 0;
            RecvData_Temp[7] = 0;
            RecvData_Temp[8] = 0;
            RecvData_Temp[9] = 0;
            return flag;
        }
        #endregion
        #region 写入STM数据
        /// <summary>
        /// 写一个数据
        /// </summary>
        /// <param name="Direction">方向</param>
        /// <param name="Target">目标地址</param>
        /// <returns></returns>
        public int WriteInfo(int Direction, int Target)
        {
            int flag = -1;
            byte[] data = new byte[10];
            data[0] = 0xF3;
            data[1] = 0x1A;
            data[2] = 0x00; 
            data[3] = Convert.ToByte(Direction);
            data[4] = Convert.ToByte(Target);
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0x00;
            data[10] = 0x00;
            data[11] = 0x00;
            data[12] = 0x00;
            data[13] = 0x00;
            data[14] = 0x00;
            data[23] = 0xCF;
            
            Thread.Sleep(100);
            int numPro = 0;
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[9] = 0;
            this.Send(data);
            while ((RecvData_Temp[0] == 0 || RecvData_Temp[9] == 0) && numPro < 150)
            {
                Thread.Sleep(5);
                numPro++;
            }
            if (numPro < 150)
            {
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[2] == 0x01 && RecvData_Temp[3] == 0x01 && RecvData_Temp[9] == 0xCF)
                {
                    flag = 0;//成功
                }
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[2] == 0x01 && RecvData_Temp[3] == 0x00 && RecvData_Temp[9] == 0xCF)
                {
                    flag = 1;//失败
                }
            }
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[1] = 0;
            RecvData_Temp[2] = 0;
            RecvData_Temp[3] = 0;
            RecvData_Temp[4] = 0;
            RecvData_Temp[5] = 0;
            RecvData_Temp[6] = 0;
            RecvData_Temp[7] = 0;
            RecvData_Temp[8] = 0;
            RecvData_Temp[9] = 0;
            return flag;
        }
        /// <summary>
        /// 写入牵引棒信号
        /// </summary>
        /// <param name="RaiseValue">牵引棒信号</param>
        /// <returns></returns>
        public int WriteInfoRaise(int RaiseValue)
        {
            int flag = -1;
            byte[] data = new byte[10];
            data[0] = 0xF3;
            data[1] = 0x1A;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = Convert.ToByte(RaiseValue);
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0xCF;

            Thread.Sleep(100);
            int numPro = 0;
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[9] = 0;
            this.Send(data);
            while ((RecvData_Temp[0] == 0 || RecvData_Temp[9] == 0) && numPro < 150)
            {
                Thread.Sleep(5);
                numPro++;
            }
            if (numPro < 150)
            {
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[2] == 0x01 && RecvData_Temp[3] == 0x01 && RecvData_Temp[9] == 0xCF)
                {
                    flag = 0;//成功
                }
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[2] == 0x01 && RecvData_Temp[3] == 0x00 && RecvData_Temp[9] == 0xCF)
                {
                    flag = 1;//失败
                }
            }
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[1] = 0;
            RecvData_Temp[2] = 0;
            RecvData_Temp[3] = 0;
            RecvData_Temp[4] = 0;
            RecvData_Temp[5] = 0;
            RecvData_Temp[6] = 0;
            RecvData_Temp[7] = 0;
            RecvData_Temp[8] = 0;
            RecvData_Temp[9] = 0;
            return flag;
        }
        /// <summary>
        /// 写入放行
        /// </summary>
        /// <returns></returns>
        public int WriteInfoGo()
        {
            int flag = -1;
            byte[] data = new byte[10];
            data[0] = 0xF3;
            data[1] = 0x01;
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x00;
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0xCF;

            Thread.Sleep(100);
            int numPro = 0;
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[9] = 0;
            this.Send(data);
            while ((RecvData_Temp[0] == 0 || RecvData_Temp[9] == 0) && numPro < 150)
            {
                Thread.Sleep(5);
                numPro++;
            }
            if (numPro < 150)
            {
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[2] == 0x01 && RecvData_Temp[3] == 0x01 && RecvData_Temp[9] == 0xCF)
                {
                    flag = 0;//成功
                }
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[2] == 0x01 && RecvData_Temp[3] == 0x00 && RecvData_Temp[9] == 0xCF)
                {
                    flag = 1;//失败
                }
            }
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[1] = 0;
            RecvData_Temp[2] = 0;
            RecvData_Temp[3] = 0;
            RecvData_Temp[4] = 0;
            RecvData_Temp[5] = 0;
            RecvData_Temp[6] = 0;
            RecvData_Temp[7] = 0;
            RecvData_Temp[8] = 0;
            RecvData_Temp[9] = 0;
            return flag;
        }
        #endregion

        int Offset = 0;
        byte[] RecvData_Temp = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        #region 读取STM值
        /// <summary>
        /// 读值
        /// </summary>
        /// <param name="alarmvalue">报警信息</param>
        /// <param name="posvalue">位置</param>
        /// <param name="direction">方向</param>
        /// <returns></returns>
        public int ReadInfo(out int alarmvalue, out int posvalue, out int direction)
        {
            int flag = -1;
            Byte[] data = new Byte[10];
            alarmvalue = 0;
            posvalue = 0;
            direction = 0;
            data[0] = 0xF3;
            data[1] = 0x0A;
            data[2] = 0x01;
            data[3] = 0x02;
            data[4] = 0x03;
            data[5] = 0x04;
            data[6] = 0x00;
            data[7] = 0x00;
            data[8] = 0x00;
            data[9] = 0xCF;

            Thread.Sleep(100);
            int numPro = 0;
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[9] = 0;
            this.Send(data);
            while ((RecvData_Temp[0] == 0 || RecvData_Temp[9] == 0) && numPro < 150)
            {
                Thread.Sleep(5);
                numPro++;
            }
            if (numPro < 150)
            {
                if (RecvData_Temp[0] == 0xF3 && RecvData_Temp[1] == 0x0B && RecvData_Temp[9] == 0xCF)
                {
                    flag = 0;
                    posvalue = RecvData_Temp[2] * 256 + RecvData_Temp[3];
                    alarmvalue = RecvData_Temp[4] * 256 + RecvData_Temp[5];
                    direction = RecvData_Temp[6];
                }
                else
                {
                    if (RecvData_Temp[8] == 0)
                    {
                        flag = -3;
                    }
                }
            }
            Offset = 0;
            RecvData_Temp[0] = 0;
            RecvData_Temp[1] = 0;
            RecvData_Temp[2] = 0;
            RecvData_Temp[3] = 0;
            RecvData_Temp[4] = 0;
            RecvData_Temp[5] = 0;
            RecvData_Temp[6] = 0;
            RecvData_Temp[7] = 0;
            return flag;
        }
        #endregion


        #region IDispose member
        public void Dispose()
        {
            if (this.clientSocket.Connected)
            {
                this.clientSocket.Close();
            }
        }
        #endregion
    }
}
