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
        //设定的设备刷新时间，单位毫秒
        public virtual int DeviceUpdatetimes { get { return 0; } }
        //计数器管线
        public virtual int CounterAccessCount { get { return 0; } }
        //获取计数器不复位
        public virtual int GetCounterAccess(int index1, int index2)
        {
            return 0;
        }
        //获取计数复位，由于多线程原因，需要复位操作同步进行，否则会由于时序问题导致丢失计数器
        public virtual int GetCounterAccessReset(int index1, int index2)
        {
            return 0;
        }
        //按钮管线
        public virtual int ButtonAccessCount { get { return 0; } }
        public virtual bool GetButtonAccess(int index1, int index2)
        {
            return false;
        }
        ////轴管线
        //public virtual int AxisAccessCount { get { return 0; } }
        //public virtual float  GetAxisAccess(int index)
        //{
        //    return 0.0f;
        //}
        ////获取轴值域
        //public virtual void GetAxisAccessRange(int index,out int min,out int max)
        //{
        //    min = 0;
        //    max = 0;
        //}
        ////获取轴的当前电位值
        //public virtual int GetAxisAccessValue(int index)
        //{
        //    return 0;
        //}
        ////拷贝轴算子
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


        ////方向键按钮定义,4个方向按钮,返回ture被按下，返回false 没有按下
        //public virtual bool ButtonUp { get { return false; } }
        //public virtual bool ButtonDown { get { return false; } }
        //public virtual bool ButtonLeft { get { return false; } }
        //public virtual bool ButtonRight { get { return false; } }

        ////以下的按钮都是计数型按钮
        ////用户按一下累计一次计数
        ////获取后累计计数清除

        ////开始按钮
        //public virtual bool ButtonStart { get { return false; } }
        ////挖坑按钮
        //public virtual bool ButtonDig { get{return false;} }
        ////倍率切换按钮
        //public virtual bool ButtonSetTimes { get { return false; } }
        ////退票按钮
        //public virtual bool ButtonGetTickets { get { return false; } }
        ////投币按钮
        //public virtual int ButtonInsertCoins { get { return 0; } }
        ////控制台按钮
        //public virtual bool ButtonSystemEnter { get { return false; } }

        ////扩展定义，不是所有设备都支持
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

        //指令管线
        public virtual int CommanderAccessCount { get { return 0; } }
        public virtual bool GetCommanderAccess(int index)
        {
            return false;
        }
        public virtual void SetCommanderAccess(int index,bool v)
        {

        }
        ////力反馈控制
        ////基础力大小0.0~1.0
        ////基础力没有方向
        //public virtual float BaseForce { get { return 0.0f; } set { } }
        ////力反馈震动力大小，震动力有方向和大小
        //public virtual float shakeForce { get { return 0; } set { } }
        //打开设备
        public bool Initialization()
        {
            return Initialization(1);
        }
        public virtual bool Initialization(int portIndex)
        {
            return false;
        }
        
        //关闭设备
        public virtual void Release()
        {
            
        }
        
    }
}
