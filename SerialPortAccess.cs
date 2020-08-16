using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using HelperInterface;

namespace HelperInterfaceInternal
{
    
    class SerialPortAccess : SerialPortDevice
    {
        public SerialPortAccessType DeviceType;
        public SerialPortAccess(SerialPortAccessType deviceType)
            :base()
        {
            DeviceType = deviceType;
        }
        public static SerialPortAccess AllocDevice(SerialPortAccessType deviceType)
        {
            switch(deviceType)
            {
                case SerialPortAccessType.Type_Initial_D_5:
                    return null;
                case SerialPortAccessType.Type_DDR:
                    return null;
                case SerialPortAccessType.Type_Treasure:
                    return new SerialPortAccess_DDR(deviceType);
            }
            return null;
        }
        //�趨���豸ˢ��ʱ�䣬��λ����
        public virtual int DeviceUpdatetimes { get { return 0; } }
        //����������
        public virtual int CounterAccessCount { get { return 0; } }
        //��ȡ����������λ
        public virtual int GetCounterAccess(int index1, int index2)
        {
            return 0;
        }
        //��ȡ������λ�����ڶ��߳�ԭ����Ҫ��λ����ͬ�����У����������ʱ�����⵼�¶�ʧ������
        public virtual int GetCounterAccessReset(int index1, int index2)
        {
            return 0;
        }
        //��ť����
        public virtual int ButtonAccessCount { get { return 0; } }
        public virtual bool GetButtonAccess(int index1, int index2)
        {
            return false;
        }
        ////�����
        //public virtual int AxisAccessCount { get { return 0; } }
        //public virtual float  GetAxisAccess(int index)
        //{
        //    return 0.0f;
        //}
        ////��ȡ��ֵ��
        //public virtual void GetAxisAccessRange(int index,out int min,out int max)
        //{
        //    min = 0;
        //    max = 0;
        //}
        ////��ȡ��ĵ�ǰ��λֵ
        //public virtual int GetAxisAccessValue(int index)
        //{
        //    return 0;
        //}
        ////����������
        //public virtual void GetAxisAccessAlu(int index,SerialPortInputAxisAlu alu)
        //{
            
        //}
        //public virtual void SetAxisAccessAlu(int index,SerialPortInputAxisAlu alu)
        //{

        //}
        //public virtual void GetAxisAccessAluSteeringWheel(SerialPortInputAxisAluSteeringWheel alu)
        //{

        //}
        //public virtual void SetAxisAccessAluSteeringWheel(SerialPortInputAxisAluSteeringWheel alu)
        //{

        //}
        //public virtual void GetAxisAccessAluAccelerator(SerialPortInputAxisAluAccelerator alu)
        //{

        //}
        //public virtual void SetAxisAccessAluAccelerator(SerialPortInputAxisAluAccelerator alu)
        //{

        //}
        //public virtual void GetAxisAccessAluBrake(SerialPortInputAxisAluBrake alu)
        //{

        //}
        //public virtual void SetAxisAccessAluBrake(SerialPortInputAxisAluBrake alu)
        //{

        //}


        ////�������ť����,4������ť,����ture�����£�����false û�а���
        //public virtual bool ButtonUp { get { return false; } }
        //public virtual bool ButtonDown { get { return false; } }
        //public virtual bool ButtonLeft { get { return false; } }
        //public virtual bool ButtonRight { get { return false; } }

        ////���µİ�ť���Ǽ����Ͱ�ť
        ////�û���һ���ۼ�һ�μ���
        ////��ȡ���ۼƼ������

        ////��ʼ��ť
        //public virtual bool ButtonStart { get { return false; } }
        ////�ڿӰ�ť
        //public virtual bool ButtonDig { get{return false;} }
        ////�����л���ť
        //public virtual bool ButtonSetTimes { get { return false; } }
        ////��Ʊ��ť
        //public virtual bool ButtonGetTickets { get { return false; } }
        ////Ͷ�Ұ�ť
        //public virtual int ButtonInsertCoins { get { return 0; } }
        ////����̨��ť
        //public virtual bool ButtonSystemEnter { get { return false; } }

        ////��չ���壬���������豸��֧��
        //public virtual bool Button_0Click { get { return false; } }
        //public virtual bool Button_1Click { get { return false; } }
        //public virtual bool Button_2Click { get { return false; } }
        //public virtual bool Button_3Click { get { return false; } }
        //public virtual bool Button_4Click { get { return false; } }
        //public virtual bool Button_5Click { get { return false; } }
        //public virtual bool Button_6Click { get { return false; } }
        //public virtual bool Button_7Click { get { return false; } }
        //public virtual bool Button_8Click { get { return false; } }
        //public virtual bool Button_9Click { get { return false; } }

        //ָ�����
        public virtual int CommanderAccessCount { get { return 0; } }
        public virtual bool GetCommanderAccess(int index)
        {
            return false;
        }
        public virtual void SetCommanderAccess(int index,bool v)
        {

        }
        ////����������
        ////��������С0.0~1.0
        ////������û�з���
        //public virtual float BaseForce { get { return 0.0f; } set { } }
        ////������������С�������з���ʹ�С
        //public virtual float shakeForce { get { return 0; } set { } }
        //���豸
        public bool Initialization()
        {
            return Initialization(1);
        }
        public virtual bool Initialization(int portIndex)
        {
            return false;
        }
        
        //�ر��豸
        public virtual void Release()
        {
            
        }
        
    }
}
