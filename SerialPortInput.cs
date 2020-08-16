using System;
using System.Collections.Generic;
using System.Text;

namespace HelperInterface
{
    public enum SerialPortAccessType
    {
        Type_Initial_D_5,       //头文字D5
        Type_DDR,              //疯狂飙车
        Type_Treasure          //仓鼠的宝藏
    }
    //轴类型分配
    public enum AxisFunctionType
    {
        FunctionType_Accelerator        =0,         //油门
        FunctionType_Brake              =1,         //刹车
        FunctionType_SteeringWheel      =2          //方向盘
    }
    //按键类型分配
    public class SerialPortInput
    {
        private HelperInterfaceInternal.SerialPortAccess serialPortAccess = null;

        private HelperInterface.SerialPortInputTrack m_TraceObject = null;
        public HelperInterface.SerialPortInputTrack traceObject 
        { 
            get 
            {
                return m_TraceObject;
            } 
            set 
            {
                m_TraceObject = value;
                if (serialPortAccess != null)
                {
                    serialPortAccess.traceObject = m_TraceObject;
                }
            } 
        }

        public bool Initialization(SerialPortAccessType type)
        {
            return Initialization(type, 1);
        }
        public bool Initialization(SerialPortAccessType type, int comPortIndex)
        {
            serialPortAccess = HelperInterfaceInternal.SerialPortAccess.AllocDevice(type);
            if (serialPortAccess == null)
                return false;
            serialPortAccess.traceObject = traceObject;
            return serialPortAccess.Initialization(comPortIndex);
        }
        public void Release()
        {
            if (serialPortAccess != null)
            {
                serialPortAccess.Release();
                serialPortAccess = null;
            }
        }
        public bool IsOpen 
        { 
            get 
            {
                if (serialPortAccess == null)
                    return false;
                return serialPortAccess.IsOpen;
            } 
        }

        //设定的设备刷新时间，单位毫秒
        public int DeviceUpdatetimes { get { return serialPortAccess.DeviceUpdatetimes; } }
        //计数管线控制,计数管线的值是累计型的，不主动清除则会一直累计
        public int CounterAccessCount { get { return serialPortAccess.CounterAccessCount; } }
        public int GetCounterAccess(int index1, int index2)
        {
            return serialPortAccess.GetCounterAccess(index1, index2);
        }
        public int GetCounterAccessReset(int index1, int index2)
        {
            return serialPortAccess.GetCounterAccessReset(index1, index2);
        }
        //按钮管线
        public int ButtonAccessCount { get { return serialPortAccess.ButtonAccessCount; } }
        public bool GetButtonAccess(int index1, int index2)
        {
            return serialPortAccess.GetButtonAccess(index1, index2);
        }
        ////轴值管线
        //public int AxisAccessCount { get { return serialPortAccess.AxisAccessCount; } }
        //public float GetAxisAccess(int index)
        //{
        //    return serialPortAccess.GetAxisAccess(index);
        //}
        ////获取轴值域
        //public void GetAxisAccessRange(int index, out int min, out int max)
        //{
        //    serialPortAccess.GetAxisAccessRange(index, out min, out max);
        //}
        ////获取轴的当前电位值
        //public int GetAxisAccessValue(int index)
        //{
        //    return serialPortAccess.GetAxisAccessValue(index);
        //}
        ////拷贝轴算子
        //public void GetAxisAccessAlu(int index, SerialPortInputAxisAlu alu)
        //{
        //    serialPortAccess.GetAxisAccessAlu(index, alu);
        //}
        //public void SetAxisAccessAlu(int index, SerialPortInputAxisAlu alu)
        //{
        //    serialPortAccess.SetAxisAccessAlu(index, alu);
        //}
        //public void GetAxisAccessAluSteeringWheel(SerialPortInputAxisAluSteeringWheel alu)
        //{
        //    serialPortAccess.GetAxisAccessAluSteeringWheel(alu);
        //}
        //public void SetAxisAccessAluSteeringWheel(SerialPortInputAxisAluSteeringWheel alu)
        //{
        //    serialPortAccess.SetAxisAccessAluSteeringWheel(alu);
        //}
        //public void GetAxisAccessAluAccelerator(SerialPortInputAxisAluAccelerator alu)
        //{
        //    serialPortAccess.GetAxisAccessAluAccelerator(alu);
        //}
        //public void SetAxisAccessAluAccelerator(SerialPortInputAxisAluAccelerator alu)
        //{
        //    serialPortAccess.SetAxisAccessAluAccelerator(alu);
        //}
        //public void GetAxisAccessAluBrake(SerialPortInputAxisAluBrake alu)
        //{
        //    serialPortAccess.GetAxisAccessAluBrake(alu);
        //}
        //public void SetAxisAccessAluBrake(SerialPortInputAxisAluBrake alu)
        //{
        //    serialPortAccess.SetAxisAccessAluBrake(alu);
        //}
        //输出指令管线
        public int CommanderAccessCount { get { return serialPortAccess.CounterAccessCount; } }
        public bool GetCommanderAccess(int index)
        {
            return serialPortAccess.GetCommanderAccess(index);
        }
        public void SetCommanderAccess(int index, bool v)
        {
            serialPortAccess.SetCommanderAccess(index, v);
        }
        //力反馈控制
        //基础力大小0.0~1.0
        //public float BaseForce 
        //{ 
        //    get 
        //    {
        //        return serialPortAccess.BaseForce; 
        //    } 
        //    set 
        //    {
        //        serialPortAccess.BaseForce = value;
        //    } 
        //}
        ////震动力，大小-1.0~1.0,符合为方向
        //public float shakeForce
        //{
        //    get
        //    {
        //        return serialPortAccess.shakeForce;
        //    }
        //    set
        //    {
        //        serialPortAccess.shakeForce = value;
        //    }
        //}
        //刷新震动函数，这个函数需要在每个周期内持续调用，以驱动计算
        //private HelperInterfaceInternal.ShakeBase currentShakeInfo = null;
        //public void UpdateShakeParameter(HelperInterface.ShakeType type, ref HelperInterface.ShakeParameter shakeParameter)
        //{
        //    try
        //    {
        //        if (type == ShakeType.ShakeType_None)//停止震动
        //        {
        //            currentShakeInfo = null;
        //            shakeForce = 0.0f;
        //        }
        //        else
        //        {
        //            //对象没有改变则直接刷新一次参数
        //            if (currentShakeInfo != null && currentShakeInfo.shakeType == type)
        //            {
        //                currentShakeInfo.ResetParameter(ref shakeParameter);
        //            }
        //            else
        //            {
        //                HelperInterfaceInternal.ShakeBase old = currentShakeInfo;
        //                currentShakeInfo = HelperInterfaceInternal.ShakeBase.AllocShakeObject(type);
        //                if (currentShakeInfo != null)
        //                {
        //                    currentShakeInfo.Initialization(ref shakeParameter, old);
        //                }
        //            }
        //            //计算震动力
        //            shakeForce = currentShakeInfo.Account();
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        shakeForce = 0.0f;
        //    }
        //}
        //public void StopShake()
        //{
        //    currentShakeInfo = null;
        //    shakeForce = 0.0f;
        //}
        
    }
}
