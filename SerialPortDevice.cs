using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace HelperInterfaceInternal
{
    class SerialPortDevice
    {
        private SerialPort serialPort = null;
        private MemoryStream sendBuffer = null;
        private MemoryStream recvBuffer = null;
        private byte[] recvSwapBuffer = null;
        //设置的参数
        private string PortName;
        private int DaudRate;
        private Parity Parity;
        private StopBits StopBits;
        private Handshake Handshake;
        private int ReadTimeout;
        private int WriteTimeout;
        private int ReadBufferSize;
        private int WriteBufferSize;
        //工作处理周期
        System.Timers.Timer deviceworkCycleTimer = null;

        private HelperInterface.SerialPortInputTrack m_TrackObject = null;
        public HelperInterface.SerialPortInputTrack traceObject { get { return m_TrackObject; } set { m_TrackObject = value; } }


        public bool IsOpen 
        { 
            get 
            {
                if (serialPort == null)
                    return false;
                return serialPort.IsOpen;
            } 
        }
        private static SerialPort OpenOneDevice(string portName,int daudRate,Parity parity,StopBits stopBits,Handshake handshake,
                                        int readTimeout, int writeTimeout, int readBufferSize, int writeBufferSize)
        {
            SerialPort serialPort = null;
            try
            {
                Thread.Sleep(500);
                serialPort = new SerialPort(portName);//连接的端口
                serialPort.BaudRate = daudRate;//比特率，每秒传输多少位
                serialPort.Parity = parity;//奇偶校验位
                serialPort.StopBits = stopBits;//停止位
                serialPort.Handshake = handshake;//控制协议
                serialPort.ReadTimeout = readTimeout;
                serialPort.WriteTimeout = writeTimeout;
                serialPort.ReadBufferSize = readBufferSize;
                serialPort.WriteBufferSize = writeBufferSize;
                //打开设备
                serialPort.Open();
                return serialPort;
            }
            catch (System.Exception ex)
            {
                CloseOneDevice(serialPort);
                return null;
            }
        }
        private static void CloseOneDevice(SerialPort serialPort)
        {
            try
            {
                if (serialPort == null)
                    return;
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    Thread.Sleep(500);
                }
            }
            catch (System.Exception ex)
            {
            	
            }
            
        }

        public SerialPortDevice()
        {

        }
        /*
         *
         * portName .PortName为COM口名称,例如"COM1","COM2"等,注意是string类型
         * daudRate 波特率
         * parity 奇偶校验位
         * stopBits 停止位
         * handshake 控制协议
         * */
        protected bool OpenDevice(string portName,int daudRate,Parity parity,StopBits stopBits,Handshake handshake,
                                double updatetimes, int readTimeout, int writeTimeout,
                                int readBufferSize, int writeBufferSize,bool isSelfWork)
        {
            if (IsOpen)
                CloseDevice();
            PortName=portName;
            DaudRate=daudRate;
            Parity=parity;
            StopBits=stopBits;
            Handshake = handshake;
            ReadTimeout = readTimeout;
            WriteTimeout = writeTimeout;
            ReadBufferSize = readBufferSize;
            WriteBufferSize = writeBufferSize;
            recvSwapBuffer = new byte[ReadBufferSize];
            serialPort = OpenOneDevice(PortName, DaudRate, Parity, StopBits, Handshake, ReadTimeout, WriteTimeout,
                                    ReadBufferSize, WriteBufferSize);
            if (serialPort == null)
                return false;
            try
            {
                sendBuffer = new MemoryStream(1024);
                recvBuffer = new MemoryStream(1024);
                if (isSelfWork)
                {
                    //启动处理周期
                    deviceworkCycleTimer = new System.Timers.Timer();
                    deviceworkCycleTimer.Elapsed += new System.Timers.ElapsedEventHandler(SerialPortDevice_WorkUpdate);
                    deviceworkCycleTimer.Interval = updatetimes;
                    deviceworkCycleTimer.AutoReset = false;
                    deviceworkCycleTimer.Enabled = true;
                }
                
            }
            catch (System.Exception ex)
            {
                CloseDevice();
                return false;
            }
            return true;
        }
        protected void CloseDevice()
        {
            if (deviceworkCycleTimer != null)
            {
                deviceworkCycleTimer.Close();
                deviceworkCycleTimer = null;
            }
            if (sendBuffer != null)
            {
                sendBuffer.Close();
                sendBuffer = null;
            }
            if (recvBuffer != null)
            {
                recvBuffer.Close();
                recvBuffer = null;
            }
            CloseOneDevice(serialPort);
            serialPort = null;
        }
        protected void WorkHande_RecvData()
        {
            //首先处理接受数据
            int readLength = 0;
            try
            {
                if (!IsOpen)
                    throw new Exception("cannt open device!");
                //是否有需要接收的数据
                readLength = serialPort.Read(recvSwapBuffer, 0, ReadBufferSize);
                if (readLength > 0)
                {
                    //进入缓冲临界区
                    Monitor.Enter(recvBuffer);
                    try
                    {
                        //保持缓冲区与设备缓冲区的尺寸一致
                        if (recvBuffer.Length >= serialPort.ReadBufferSize)
                        {
                            //丢弃之前的数据
                            recvBuffer.SetLength(0);
                        }
                        //写入缓冲区
                        recvBuffer.Write(recvSwapBuffer, 0, readLength);

                    }
                    catch (System.Exception ex)
                    {
                        //出错清除缓冲区
                        recvBuffer.SetLength(0);
                    }
                    Monitor.Exit(recvBuffer);
                }
            }
            catch (System.Exception ex)
            {
                if (traceObject != null)
                {
                    traceObject.Err(string.Format("取数据异常!datalen:{0},err:{1}", readLength, ex.ToString()));
                }
                //如果发生了异常则需要端口当前设备然后重新连接
                //下次在处理数据
                CloseOneDevice(serialPort);
                serialPort = OpenOneDevice(PortName, DaudRate, Parity, StopBits, Handshake, ReadTimeout, WriteTimeout,
                                            ReadBufferSize, WriteBufferSize);
            }
            
            
            ////首先处理接受数据
            //int BytesToRead = 0;
            //try
            //{
            //    if (!IsOpen)
            //        throw new Exception("cannt open device!");
            //    //是否有需要接收的数据
            //    BytesToRead = serialPort.BytesToRead;
            //    if (BytesToRead > 0)
            //    {
            //        byte[] buffer = new byte[BytesToRead];
            //        int readLength = serialPort.Read(buffer, 0, BytesToRead);
            //        //进入缓冲临界区
            //        Monitor.Enter(recvBuffer);
            //        try
            //        {
            //            //保持缓冲区与设备缓冲区的尺寸一致
            //            if (recvBuffer.Length >= serialPort.ReadBufferSize)
            //            {
            //                //丢弃之前的数据
            //                recvBuffer.SetLength(0);
            //            }
            //            //写入缓冲区
            //            recvBuffer.Write(buffer, 0, readLength);

            //        }
            //        catch (System.Exception ex)
            //        {
            //            //出错清除缓冲区
            //            recvBuffer.SetLength(0);
            //        }
            //        Monitor.Exit(recvBuffer);
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    if (traceObject != null)
            //    {
            //        traceObject.Err(string.Format("取数据异常!datalen:{0},err:{1}", BytesToRead, ex.ToString()));
            //    }
            //    //如果发生了异常则需要端口当前设备然后重新连接
            //    //下次在处理数据
            //    CloseOneDevice(serialPort);
            //    serialPort = OpenOneDevice(PortName, DaudRate, Parity, StopBits, Handshake,ReadTimeout,WriteTimeout,
            //                                ReadBufferSize, WriteBufferSize);
            //}
        }
        protected void WorkHande_SendData()
        {
            //首先处理接受数据
            try
            {
                byte[] buffer = null;
                //进入缓冲临界区读取数据
                Monitor.Enter(sendBuffer);
                try
                {
                    if (sendBuffer.Length > 0)//有缓冲数据
                    {
                        if (!IsOpen)//设备没有开，清除缓冲数据
                            throw new Exception("cannt open device!");
                        buffer = sendBuffer.ToArray();
                        sendBuffer.SetLength(0);
                    }
                }
                catch (System.Exception ex)
                {
                    sendBuffer.SetLength(0);
                    buffer = null;
                }
                Monitor.Exit(sendBuffer);
                if (buffer != null && IsOpen)
                {
                    serialPort.Write(buffer, 0, buffer.Length);
                }
            }
            catch (System.Exception ex)
            {
                if (traceObject != null)
                {
                    traceObject.Err(string.Format("写数据异常!datalen:{0},err:{1}", serialPort.BytesToRead, ex.ToString()));
                }
                //如果发生了异常则需要端口当前设备然后重新连接
                //下次在处理数据
                CloseOneDevice(serialPort);
                serialPort = OpenOneDevice(PortName, DaudRate, Parity, StopBits, Handshake,ReadTimeout,WriteTimeout,
                                            ReadBufferSize, WriteBufferSize);
            }
        }
        public void SerialPortDevice_WorkUpdate()
        {
            WorkHande_RecvData();
            WorkHande_SendData();
        }
        private void SerialPortDevice_WorkUpdate(object source, System.Timers.ElapsedEventArgs e)
        {
            SerialPortDevice_WorkUpdate();
            deviceworkCycleTimer.Enabled = true;
        }
        
        //尝试获取一次接受的数据
        protected bool RecvPortData(out byte[] data)
        {
            data = null;
            if (!IsOpen)
                return false;
            Monitor.Enter(recvBuffer);
            try
            {
                if (recvBuffer.Length > 0)
                {
                    data = recvBuffer.ToArray();
                    recvBuffer.SetLength(0);
                }
            }
            catch (System.Exception ex)
            {
                recvBuffer.SetLength(0);
                data = null;
            }
            Monitor.Exit(recvBuffer);
            return data != null;
        }
        //发送数据
        protected void SendPortData(byte[] data)
        {
            if (!IsOpen)
                return;
            Monitor.Enter(sendBuffer);
            try
            {
               
                //保持缓冲区和设备缓冲区一致
                //如果溢出丢弃数据
                if (sendBuffer.Length >= serialPort.WriteBufferSize)
                    sendBuffer.SetLength(0);
                sendBuffer.Write(data, 0, data.Length);
            }
            catch (System.Exception ex)
            {
                sendBuffer.SetLength(0);
            }
            Monitor.Exit(sendBuffer);
        }
    }
}
