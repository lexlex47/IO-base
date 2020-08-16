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
        //���õĲ���
        private string PortName;
        private int DaudRate;
        private Parity Parity;
        private StopBits StopBits;
        private Handshake Handshake;
        private int ReadTimeout;
        private int WriteTimeout;
        private int ReadBufferSize;
        private int WriteBufferSize;
        //������������
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
                serialPort = new SerialPort(portName);//���ӵĶ˿�
                serialPort.BaudRate = daudRate;//�����ʣ�ÿ�봫�����λ
                serialPort.Parity = parity;//��żУ��λ
                serialPort.StopBits = stopBits;//ֹͣλ
                serialPort.Handshake = handshake;//����Э��
                serialPort.ReadTimeout = readTimeout;
                serialPort.WriteTimeout = writeTimeout;
                serialPort.ReadBufferSize = readBufferSize;
                serialPort.WriteBufferSize = writeBufferSize;
                //���豸
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
         * portName .PortNameΪCOM������,����"COM1","COM2"��,ע����string����
         * daudRate ������
         * parity ��żУ��λ
         * stopBits ֹͣλ
         * handshake ����Э��
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
                    //������������
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
            //���ȴ����������
            int readLength = 0;
            try
            {
                if (!IsOpen)
                    throw new Exception("cannt open device!");
                //�Ƿ�����Ҫ���յ�����
                readLength = serialPort.Read(recvSwapBuffer, 0, ReadBufferSize);
                if (readLength > 0)
                {
                    //���뻺���ٽ���
                    Monitor.Enter(recvBuffer);
                    try
                    {
                        //���ֻ��������豸�������ĳߴ�һ��
                        if (recvBuffer.Length >= serialPort.ReadBufferSize)
                        {
                            //����֮ǰ������
                            recvBuffer.SetLength(0);
                        }
                        //д�뻺����
                        recvBuffer.Write(recvSwapBuffer, 0, readLength);

                    }
                    catch (System.Exception ex)
                    {
                        //�������������
                        recvBuffer.SetLength(0);
                    }
                    Monitor.Exit(recvBuffer);
                }
            }
            catch (System.Exception ex)
            {
                if (traceObject != null)
                {
                    traceObject.Err(string.Format("ȡ�����쳣!datalen:{0},err:{1}", readLength, ex.ToString()));
                }
                //����������쳣����Ҫ�˿ڵ�ǰ�豸Ȼ����������
                //�´��ڴ�������
                CloseOneDevice(serialPort);
                serialPort = OpenOneDevice(PortName, DaudRate, Parity, StopBits, Handshake, ReadTimeout, WriteTimeout,
                                            ReadBufferSize, WriteBufferSize);
            }
            
            
            ////���ȴ����������
            //int BytesToRead = 0;
            //try
            //{
            //    if (!IsOpen)
            //        throw new Exception("cannt open device!");
            //    //�Ƿ�����Ҫ���յ�����
            //    BytesToRead = serialPort.BytesToRead;
            //    if (BytesToRead > 0)
            //    {
            //        byte[] buffer = new byte[BytesToRead];
            //        int readLength = serialPort.Read(buffer, 0, BytesToRead);
            //        //���뻺���ٽ���
            //        Monitor.Enter(recvBuffer);
            //        try
            //        {
            //            //���ֻ��������豸�������ĳߴ�һ��
            //            if (recvBuffer.Length >= serialPort.ReadBufferSize)
            //            {
            //                //����֮ǰ������
            //                recvBuffer.SetLength(0);
            //            }
            //            //д�뻺����
            //            recvBuffer.Write(buffer, 0, readLength);

            //        }
            //        catch (System.Exception ex)
            //        {
            //            //�������������
            //            recvBuffer.SetLength(0);
            //        }
            //        Monitor.Exit(recvBuffer);
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    if (traceObject != null)
            //    {
            //        traceObject.Err(string.Format("ȡ�����쳣!datalen:{0},err:{1}", BytesToRead, ex.ToString()));
            //    }
            //    //����������쳣����Ҫ�˿ڵ�ǰ�豸Ȼ����������
            //    //�´��ڴ�������
            //    CloseOneDevice(serialPort);
            //    serialPort = OpenOneDevice(PortName, DaudRate, Parity, StopBits, Handshake,ReadTimeout,WriteTimeout,
            //                                ReadBufferSize, WriteBufferSize);
            //}
        }
        protected void WorkHande_SendData()
        {
            //���ȴ����������
            try
            {
                byte[] buffer = null;
                //���뻺���ٽ�����ȡ����
                Monitor.Enter(sendBuffer);
                try
                {
                    if (sendBuffer.Length > 0)//�л�������
                    {
                        if (!IsOpen)//�豸û�п��������������
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
                    traceObject.Err(string.Format("д�����쳣!datalen:{0},err:{1}", serialPort.BytesToRead, ex.ToString()));
                }
                //����������쳣����Ҫ�˿ڵ�ǰ�豸Ȼ����������
                //�´��ڴ�������
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
        
        //���Ի�ȡһ�ν��ܵ�����
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
        //��������
        protected void SendPortData(byte[] data)
        {
            if (!IsOpen)
                return;
            Monitor.Enter(sendBuffer);
            try
            {
               
                //���ֻ��������豸������һ��
                //��������������
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
