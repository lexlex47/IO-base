using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;
using HelperInterface;

namespace HelperInterfaceInternal
{
    class SerialPortAccess_DDR : SerialPortAccess
    {
        public SerialPortAccess_DDR(SerialPortAccessType deviceType)
            :base(deviceType)
        {
            
        }
        ////默认的轴值范围
        ////private const int AxisMinValue = 0x004d;
        ////private const int AxisMaxValue = 0xffb6;
        //private const int AxisMinValue = 0x00;
        //private const int AxisMaxValue = 0xff;
        ////基础力值范围
        //private const int BaseForceMinValue = 0x08;
        //private const int BaseForceMaxValue = 0xff;
        ////震动力值范围
        //private const int ShakeForceMinValue = 0x10;
        //private const int ShakeForceMaxValue = 0xf0;
        //private const int ShakeForceMidValue = 0x80;
        private class Senior_MCU_PC_Data
        {
            //24个按钮Key,用户按下触发，弹去解除,6用户,方向键类
            public int[] ButtonAccess = new int[4];
            //48个计数Key,这种类型的键位，用户按下弹起后计算为一次输入，6用户，功能键类
            public int[] CounterAccess = new int[8];
        }
        private class MCU_PC_Data
        {
            ////24个按钮Key,用户按下触发，弹去解除,6用户,方向键类
            //public int[] ButtonAccess = new int[24];
            ////48个计数KEY,这种类型的键位，用户按下弹起后计算为一次输入，6用户，功能键类
            //public int[] CounterAccess = new int[48];

            //6个玩家 总计72个键位
            public Senior_MCU_PC_Data[] senior_MCU_PC_Data = new Senior_MCU_PC_Data[6];

            ////三轴输出值
            //public float[] AxisAccess = new float[3];
            ////三个轴当前点位值，这个值在调校的时候会用
            //public int[] AxisCurrentValue = new int[3];
            ////三个轴的算子
            //public SerialPortInputAxisAlu[] AxisAlu = new SerialPortInputAxisAlu[3];
            ////根据类型分配的轴算子
            //public SerialPortInputAxisAlu[] AxisTypeAlu = new SerialPortInputAxisAlu[3];
            //public void SetAxisTypeAlu(SerialPortInputAxisAlu alu)
            //{
            //    AxisTypeAlu[(int)alu.type] = alu;
            //}
            //public SerialPortInputAxisAlu GetAxisTypeAlu(HelperInterface.AxisFunctionType type)
            //{
            //    return AxisTypeAlu[(int)type];
            //}
        }
        //private enum PC_MCU_Command
        //{
        //    Command_ResetForceFeedback,     //重置力反馈
        //    Command_AdjustForceFeedback    //矫正力反馈
        //}
        private struct PC_MCU_CommandData
        {
            //public PC_MCU_Command command;
            public object[] parameterList;
        }

        private class PC_MCU_Data
        {
            //输入量
            //24路输入量
            public int[] CommanderAccess = new int[24];
            //力反馈控制
            ////基础力大小0.0~1.0
            //public float BaseForce = 0.75f;
            ////震动力
            //public float shakeForce = 0.0f;
            //指令队列
            public List<PC_MCU_CommandData> commandList = new List<PC_MCU_CommandData>();
            public void CopyTo(PC_MCU_Data data,bool isClearCommandList)
            {
                for (int i = 0; i < 24;i++ )
                {
                    data.CommanderAccess[i] = CommanderAccess[i];
                }
                //data.BaseForce = BaseForce;
                //data.shakeForce = shakeForce;
                if (commandList.Count != 0)
                {
                    //foreach (PC_MCU_CommandData cd in commandList)
                    for (int i = 0; i < commandList.Count;i++ )
                    {
                        data.commandList.Add(commandList[i]);
                    }
                    if (isClearCommandList)
                    {
                        commandList.Clear();
                    }
                }
                
            }
            //是否发生了修改
            public bool IsModify = false;
        }
        private MCU_PC_Data mcu_pc_data=new MCU_PC_Data();
        private MCU_PC_Data mcu_pc_databuffer = new MCU_PC_Data();


        private PC_MCU_Data pc_mcu_data = new PC_MCU_Data();
        private PC_MCU_Data pc_mcu_databuffer = new PC_MCU_Data();


        //设定的设备刷新时间，单位毫秒
        public override int DeviceUpdatetimes { get { return (int)Device_Updatetimes; } }
        //计数器管线
        public override int CounterAccessCount 
        { 
            get 
            {
                int ret = 0;
                Monitor.Enter(mcu_pc_data);
                for (int i = 0; i < mcu_pc_data.senior_MCU_PC_Data.Length;i++)
                {
                    ret += mcu_pc_data.senior_MCU_PC_Data[i].CounterAccess.Length;
                }
                Monitor.Exit(mcu_pc_data);
                return ret;
            } 
        }
        public override int GetCounterAccess(int index1, int index2)
        {
            int ret = 0;
            Monitor.Enter(mcu_pc_data);
            if (index1 >= 0 && index1 < mcu_pc_data.senior_MCU_PC_Data.Length)
            {
                if (index2 >= 0 && index2 < mcu_pc_data.senior_MCU_PC_Data[index1].CounterAccess.Length)
                {
                    ret = mcu_pc_data.senior_MCU_PC_Data[index1].CounterAccess[index2];
                }                  
            }
            Monitor.Exit(mcu_pc_data);
            return ret;
        }
        public override int GetCounterAccessReset(int index1, int index2)
        {
            int ret = 0;
            Monitor.Enter(mcu_pc_data);
            if (index1 >= 0 && index1 < mcu_pc_data.senior_MCU_PC_Data.Length)
            {
                if (index2 >= 0 && index2 < mcu_pc_data.senior_MCU_PC_Data[index1].CounterAccess.Length)
                {
                    ret = mcu_pc_data.senior_MCU_PC_Data[index1].CounterAccess[index2];
                    mcu_pc_data.senior_MCU_PC_Data[index1].CounterAccess[index2] = 0;
                }
            }
            Monitor.Exit(mcu_pc_data);
            return ret;
        }
        //按钮管线
        public override int ButtonAccessCount 
        { 
            get 
            {
                int ret = 0;
                Monitor.Enter(mcu_pc_data);
                for (int i = 0; i < mcu_pc_data.senior_MCU_PC_Data.Length;i++ )
                {
                    ret += mcu_pc_data.senior_MCU_PC_Data[i].ButtonAccess.Length;
                }
                Monitor.Exit(mcu_pc_data);
                return ret;
            } 
        }
        public override bool GetButtonAccess(int index1, int index2)
        {
            bool ret = false;
            Monitor.Enter(mcu_pc_data);
            if (index1 >= 0 && index1 < mcu_pc_data.senior_MCU_PC_Data.Length)
            {
               if (index2 >= 0 && index2 < mcu_pc_data.senior_MCU_PC_Data[index1].ButtonAccess.Length)
               {
                   ret = (mcu_pc_data.senior_MCU_PC_Data[index1].ButtonAccess[index2] == 1);
               }             
            }
            Monitor.Exit(mcu_pc_data);
            return ret;
        }
        ////轴管线
        //public override int AxisAccessCount 
        //{ 
        //    get 
        //    {
        //        Monitor.Enter(mcu_pc_data);
        //        int ret = mcu_pc_data.AxisAccess.Length;
        //        Monitor.Exit(mcu_pc_data);
        //        return ret;
        //    } 
        //}
        //public override float GetAxisAccess(int index)
        //{
        //    float ret = 0.0f;
        //    Monitor.Enter(mcu_pc_data);
        //    if (index >= 0 && index < mcu_pc_data.AxisAccess.Length)
        //    {
        //        ret = mcu_pc_data.AxisAccess[index];
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //    return ret;
        //}
        ////获取轴值域
        //public override void GetAxisAccessRange(int index, out int min, out int max)
        //{
        //    min = AxisMinValue;
        //    max = AxisMaxValue;
            
        //}
        ////获取轴的当前电位值
        //public override int GetAxisAccessValue(int index)
        //{
        //    int ret = 0;
        //    Monitor.Enter(mcu_pc_data);
        //    if (index >= 0 && index < mcu_pc_data.AxisAccess.Length)
        //    {
        //        ret = mcu_pc_data.AxisCurrentValue[index];
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //    return ret;
        //}
        ////获取轴算子
        //public override void GetAxisAccessAlu(int index, SerialPortInputAxisAlu alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    if (index >= 0 && index < mcu_pc_data.AxisAccess.Length)
        //    {
        //        mcu_pc_data.AxisAlu[index].CopyTo(alu);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //}
        //public override void SetAxisAccessAlu(int index, SerialPortInputAxisAlu alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    if (index >= 0 && index < mcu_pc_data.AxisAccess.Length)
        //    {
        //        alu.CopyTo(mcu_pc_data.AxisAlu[index]);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //    if (alu.type == HelperInterface.AxisFunctionType.FunctionType_SteeringWheel)
        //    {
        //        //方向轴在获得新的范围值后需要重新定位一次中心点
        //        AdjustForceFeedback(alu.MidValue);
        //    }
        //}
        //public override void GetAxisAccessAluSteeringWheel(SerialPortInputAxisAluSteeringWheel alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    SerialPortInputAxisAlu tAul = mcu_pc_data.GetAxisTypeAlu(AxisFunctionType.FunctionType_SteeringWheel);
        //    if (tAul != null)
        //    {
        //        tAul.CopyTo(alu);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //}
        //public override void SetAxisAccessAluSteeringWheel(SerialPortInputAxisAluSteeringWheel alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    SerialPortInputAxisAlu tAul = mcu_pc_data.GetAxisTypeAlu(AxisFunctionType.FunctionType_SteeringWheel);
        //    if (tAul != null)
        //    {
        //        alu.CopyTo(tAul);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //    //方向轴在获得新的范围值后需要重新定位一次中心点
        //    AdjustForceFeedback(alu.MidValue);
        //}
        //public override void GetAxisAccessAluAccelerator(SerialPortInputAxisAluAccelerator alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    SerialPortInputAxisAlu tAul = mcu_pc_data.GetAxisTypeAlu(AxisFunctionType.FunctionType_Accelerator);
        //    if (tAul != null)
        //    {
        //        tAul.CopyTo(alu);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //}
        //public override void SetAxisAccessAluAccelerator(SerialPortInputAxisAluAccelerator alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    SerialPortInputAxisAlu tAul = mcu_pc_data.GetAxisTypeAlu(AxisFunctionType.FunctionType_Accelerator);
        //    if (tAul != null)
        //    {
        //        alu.CopyTo(tAul);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //}
        //public override void GetAxisAccessAluBrake(SerialPortInputAxisAluBrake alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    SerialPortInputAxisAlu tAul = mcu_pc_data.GetAxisTypeAlu(AxisFunctionType.FunctionType_Brake);
        //    if (tAul != null)
        //    {
        //        tAul.CopyTo(alu);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //}
        //public override void SetAxisAccessAluBrake(SerialPortInputAxisAluBrake alu)
        //{
        //    Monitor.Enter(mcu_pc_data);
        //    SerialPortInputAxisAlu tAul = mcu_pc_data.GetAxisTypeAlu(AxisFunctionType.FunctionType_Brake);
        //    if (tAul != null)
        //    {
        //        alu.CopyTo(tAul);
        //    }
        //    Monitor.Exit(mcu_pc_data);
        //}
        //指令管线
        public override int CommanderAccessCount 
        { 
            get 
            {
                Monitor.Enter(pc_mcu_data);
                int ret = pc_mcu_data.CommanderAccess.Length;
                Monitor.Exit(pc_mcu_data);
                return ret;
            } 
        }
        public override bool GetCommanderAccess(int index)
        {
            bool ret = false;
            Monitor.Enter(pc_mcu_data);
            if (index >= 0 && index < pc_mcu_data.CommanderAccess.Length)
            {
                ret = (pc_mcu_data.CommanderAccess[index] == 1);
            }
            Monitor.Exit(pc_mcu_data);
            return ret;
        }
        public override void SetCommanderAccess(int index, bool v)
        {
            Monitor.Enter(pc_mcu_data);
            if (index >= 0 && index < pc_mcu_data.CommanderAccess.Length)
            {
                pc_mcu_data.CommanderAccess[index] = (v ? 1 : 0);
                pc_mcu_data.IsModify = true;
            }
            Monitor.Exit(pc_mcu_data);
        }
        ////力反馈控制
        ////基础力大小0.0~1.0
        //public override float BaseForce 
        //{ 
        //    get 
        //    {
        //        Monitor.Enter(pc_mcu_data);
        //        float ret = pc_mcu_data.BaseForce;
        //        Monitor.Exit(pc_mcu_data);
        //        return ret;
        //    } 
        //    set 
        //    {
        //        Monitor.Enter(pc_mcu_data);
        //        //限制在0.0~1.0
        //        pc_mcu_data.BaseForce = SerialPortAccessMath.Clamp(value, 0.0f, 1.0f);
        //        pc_mcu_data.IsModify = true;
        //        Monitor.Exit(pc_mcu_data);
        //    } 
        //}
        ////力反馈震动力大小
        //public override float shakeForce 
        //{
        //    get
        //    {
        //        Monitor.Enter(pc_mcu_data);
        //        float ret = pc_mcu_data.shakeForce;
        //        Monitor.Exit(pc_mcu_data);
        //        return ret;
        //    }
        //    set
        //    {
        //        Monitor.Enter(pc_mcu_data);
        //        //限制在-1.0~1.0
        //        pc_mcu_data.shakeForce = SerialPortAccessMath.Clamp(value, -1.0f, 1.0f);
        //        pc_mcu_data.IsModify = true;
        //        Monitor.Exit(pc_mcu_data);
        //    }
        //}
        ////重置力反馈
        //private void ResetForceFeedback() 
        //{
        //    PC_MCU_CommandData data = new PC_MCU_CommandData();
        //    data.command = PC_MCU_Command.Command_ResetForceFeedback;
        //    Monitor.Enter(pc_mcu_data);
        //    pc_mcu_data.commandList.Add(data);
        //    pc_mcu_data.IsModify = true;
        //    Monitor.Exit(pc_mcu_data);
        //}
        ////矫正力反馈
        //private void AdjustForceFeedback(int minValue) 
        //{
        //    PC_MCU_CommandData data = new PC_MCU_CommandData();
        //    data.command = PC_MCU_Command.Command_AdjustForceFeedback;
        //    data.parameterList = new object[1];
        //    data.parameterList[0] = minValue;
        //    Monitor.Enter(pc_mcu_data);
        //    pc_mcu_data.commandList.Add(data);
        //    pc_mcu_data.IsModify = true;
        //    Monitor.Exit(pc_mcu_data);
        //}


        ////按钮定义,4个方向按钮,返回ture被按下，返回false 没有按下
        ////方向键同时也支持键盘操作
        //public override bool ButtonUp
        //{
        //    get
        //    {
        //        return GetButtonAccess(0);
        //    }
        //}
        //public override bool ButtonDown
        //{
        //    get
        //    {
        //        return GetButtonAccess(1);
        //    }
        //}
        //public override bool ButtonLeft
        //{
        //    get
        //    {
        //        return GetButtonAccess(2);
        //    }
        //}
        //public override bool ButtonRight
        //{
        //    get
        //    {
        //        return GetButtonAccess(3);
        //    }
        //}


        ////开始按钮
        //private int ButtonStartConter = 0;
        //public override bool ButtonStart
        //{
        //    get
        //    {
        //        if (GetCounterAccessReset(0) != 0)
        //        {
        //            ButtonStartConter = 0;
        //            return true;
        //        }
        //        if (ButtonStartConter != 0)
        //        {
        //            ButtonStartConter = 0;
        //            return true;
        //        }
        //        return false;
        //    } 
        //}

        ////挖坑按钮
        //public override bool ButtonDig
        //{
        //    get
        //    {
        //        return GetCounterAccessReset(1)!=0;
        //    }
        //}

        ////倍率切换按钮
        //public override bool ButtonSetTimes
        //{
        //    get
        //    {
        //        return GetCounterAccessReset(2) != 0;
        //    }
        //}

        ////退票按钮
        //public override bool ButtonGetTickets
        //{
        //    get
        //    {
        //        return GetCounterAccessReset(3) != 0;
        //    }
        //}

        ////投币按钮
        //public override int ButtonInsertCoins
        //{
        //    get
        //    {
        //        return GetCounterAccessReset(4);
        //    }
        //}

        ////控制台按钮
        //private int StateButtonSystemConter = 0;
        //public override bool ButtonSystemEnter
        //{
        //    get
        //    {
        //        if (GetCounterAccessReset(5) != 0)
        //        {
        //            StateButtonSystemConter = 0;
        //            return true;
        //        }
        //        if (StateButtonSystemConter != 0)
        //        {
        //            StateButtonSystemConter = 0;
        //            return true;
        //        }
        //        return false;
        //    } 
        //}

        ////扩展定义，不是所有设备都支持
        //public override bool Button_0Click { get { return GetCounterAccessReset(6) != 0; } }
        //public override bool Button_1Click { get { return GetCounterAccessReset(7) != 0; } }
        //public override bool Button_2Click { get { return GetCounterAccessReset(8) != 0; } }
        //public override bool Button_3Click { get { return GetCounterAccessReset(9) != 0; } }
        //public override bool Button_4Click { get { return GetCounterAccessReset(10) != 0; } }
        //public override bool Button_5Click { get { return GetCounterAccessReset(11) != 0; } }
        //public override bool Button_6Click { get { return GetCounterAccessReset(12) != 0; } }
        //public override bool Button_7Click { get { return GetCounterAccessReset(13) != 0; } }
        //public override bool Button_8Click { get { return GetCounterAccessReset(14) != 0; } }
        //public override bool Button_9Click { get { return GetCounterAccessReset(15) != 0; } }

        
        //打开设备
        private const int Device_DaudRate = 115200;
        private const Parity Device_Parity = Parity.None;
        private const StopBits Device_StopBits = StopBits.One;
        private const Handshake Device_Handshake = Handshake.None;
        //设置为毫秒
        //private const double Device_Updatetimes = 16.0;
        //private const int ReadTimeout = 16;
        //private const int WriteTimeout = 16;
        private const double Device_Updatetimes = 12.0;
        private const int ReadTimeout = 20;
        private const int WriteTimeout = 20;
        private const int ReadBufferSize = Device_DaudRate / 8;
        private const int WriteBufferSize = Device_DaudRate / 8;

        //工作处理周期
        System.Timers.Timer accessworkCycleTimer = null;
        //MemoryStream sendBuffer = null;
        MemoryStream recvBuffer = null;
        public override bool Initialization(int portIndex)
        {

            if (!OpenDevice(string.Format("COM{0}", portIndex),
                                Device_DaudRate,
                                Device_Parity,
                                Device_StopBits,
                                Device_Handshake,
                                Device_Updatetimes,
                                ReadTimeout,
                                WriteTimeout,
                                ReadBufferSize,
                                WriteBufferSize, false))
            {
                return false;
            }
            ////全部恢复到默认值
            //ResetForceFeedback();
            ////构造轴算子
            //mcu_pc_data.AxisAlu[0] = SerialPortInputAxisAlu.AllocAlu(axis1);
            //mcu_pc_data.AxisAlu[0].Initialization(AxisMinValue, AxisMaxValue);
            //mcu_pc_data.AxisAlu[1] = SerialPortInputAxisAlu.AllocAlu(axis2);
            //mcu_pc_data.AxisAlu[1].Initialization(AxisMinValue, AxisMaxValue);
            //mcu_pc_data.AxisAlu[2] = SerialPortInputAxisAlu.AllocAlu(axis3);
            //mcu_pc_data.AxisAlu[2].Initialization(AxisMinValue, AxisMaxValue);

            //mcu_pc_data.SetAxisTypeAlu(mcu_pc_data.AxisAlu[0]);
            //mcu_pc_data.SetAxisTypeAlu(mcu_pc_data.AxisAlu[1]);
            //mcu_pc_data.SetAxisTypeAlu(mcu_pc_data.AxisAlu[2]);

            ////矫正力反馈中心力矩
            //AdjustForceFeedback(mcu_pc_data.GetAxisTypeAlu(HelperInterface.AxisFunctionType.FunctionType_SteeringWheel).MidValue);
            //sendBuffer = new MemoryStream(1024);
            recvBuffer = new MemoryStream(1024);
            //启动处理周期
            accessworkCycleTimer = new System.Timers.Timer();
            accessworkCycleTimer.Elapsed += new System.Timers.ElapsedEventHandler(SerialPortAccess_DDR_WorkUpdate);
            accessworkCycleTimer.Interval = Device_Updatetimes;
            accessworkCycleTimer.AutoReset = false;
            accessworkCycleTimer.Enabled = true;
            return true;
        }
        //关闭设备
        public override void Release()
        {
            if (accessworkCycleTimer != null)
            {
                accessworkCycleTimer.Close();
                accessworkCycleTimer = null;
            }
            if (recvBuffer != null)
            {
                recvBuffer.Close();
                recvBuffer = null;
            }
            CloseDevice();
        }

        //处理过程为每次接收到的数据需要跟在上一次结余数据的末尾。
        //然后进行整包处理，在处理之后如果发现有不完成的包则重新缓冲等待下一次的数据
        private const byte DataHead1 = 0xaa;
        private const byte DataHead2 = 0x55;
        private const byte DataCommand = 0x01;
        private const byte DataLast = 0xcd;
        //封包尺寸
        private const int MCU_PC_DATASIZE = 22;
        private const int PC_MCU_DATASIZE = 24;

        //分包
        private bool SplitPackage(byte[] data, ref int index, ref byte[] package)
        {
            //这里首先做判断，如果已经索引到包尾了就直接退出
            if (index >= data.Length)
                return false;
            //首先定位包头
            int i;
            for (i = index; i < data.Length;i++ )
            {
                if (data[i] == DataHead1)
                {
                    if ((i+1) == data.Length)//有包头的一位数据,需要保留
                    {
                        index=i;
                        return false;
                    }
                    if (data[i+1] == DataHead2)
                    {
                        //索引指向包头
                        index=i;
                        //检测数据是否足够
                        if ((index+MCU_PC_DATASIZE) > data.Length)
                        {
                            //数据不完整，返回
                            return false;
                        }
                        //需要检测包尾是否符合要求
                        if (data[index+MCU_PC_DATASIZE-1] != DataLast)
                        {
                            //数据包不完整，跳过这个头，需要继续向下遍历其他头
                            if (traceObject != null)
                            {
                                traceObject.Trace(string.Format("不完整数据包.len:{0} index:{1}", data.Length, index));
                            }
                            continue;
                        }
                        //发现一个完整的数据包，需要取出这个数据包
                        Array.Copy(data, index, package, 0,MCU_PC_DATASIZE);
                        //修正索引指向下一个数据开始位置
                        index += MCU_PC_DATASIZE;
                        return true;
                    }
                }
            }
            //查找所有的数据都没有找到包头标记，所有数据都被废弃掉
            index = data.Length;
            if (traceObject != null)
            {
                traceObject.Trace(string.Format("本次数据无效，废弃.len:{0}", data.Length));
            }
            return false;
        }
        private void HandlePackage(byte[] data)
        {
            //检测包尺寸
            if (data.Length != MCU_PC_DATASIZE)
                return;
            //检测包头
            if (data[0] != DataHead1 || data[1] != DataHead2)
                return;
            //检测包尾
            if (data[MCU_PC_DATASIZE - 1] != DataLast)
                return;
            //处理命令
            if (data[2] != DataCommand)
                return;
            //首先将计算结果写入缓冲数据区内
            //这样保证在线程互锁的区域内保持最小运算量
            //24个方向按钮键
            mcu_pc_databuffer.senior_MCU_PC_Data[0].ButtonAccess[0] = ((data[4] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].ButtonAccess[1] = ((data[4] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].ButtonAccess[2] = ((data[4] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].ButtonAccess[3] = ((data[4] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].ButtonAccess[4] = ((data[4] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].ButtonAccess[5] = ((data[4] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].ButtonAccess[6] = ((data[4] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].ButtonAccess[7] = ((data[4] & 0x80) == 0x80) ? 1 : 0;

            mcu_pc_databuffer.senior_MCU_PC_Data[2].ButtonAccess[8] = ((data[5] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].ButtonAccess[9] = ((data[5] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].ButtonAccess[10] = ((data[5] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].ButtonAccess[11] = ((data[5] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].ButtonAccess[12] = ((data[5] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].ButtonAccess[13] = ((data[5] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].ButtonAccess[14] = ((data[5] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].ButtonAccess[15] = ((data[5] & 0x80) == 0x80) ? 1 : 0;

            mcu_pc_databuffer.senior_MCU_PC_Data[4].ButtonAccess[16] = ((data[6] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].ButtonAccess[17] = ((data[6] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].ButtonAccess[18] = ((data[6] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].ButtonAccess[19] = ((data[6] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].ButtonAccess[20] = ((data[6] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].ButtonAccess[21] = ((data[6] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].ButtonAccess[22] = ((data[6] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].ButtonAccess[23] = ((data[6] & 0x80) == 0x80) ? 1 : 0;

            //48个计数器
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[0] = ((data[7] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[1] = ((data[7] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[2] = ((data[7] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[3] = ((data[7] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[4] = ((data[7] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[5] = ((data[7] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[6] = ((data[7] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[0].CounterAccess[7] = ((data[7] & 0x80) == 0x80) ? 1 : 0;

            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[8] = ((data[8] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[9] = ((data[8] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[10] = ((data[8] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[11] = ((data[8] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[12] = ((data[8] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[13] = ((data[8] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[14] = ((data[8] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[1].CounterAccess[15] = ((data[8] & 0x80) == 0x80) ? 1 : 0;

            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[16] = ((data[9] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[17] = ((data[9] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[18] = ((data[9] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[19] = ((data[9] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[20] = ((data[9] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[21] = ((data[9] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[22] = ((data[9] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[2].CounterAccess[23] = ((data[9] & 0x80) == 0x80) ? 1 : 0;

            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[24] = ((data[10] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[25] = ((data[10] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[26] = ((data[10] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[27] = ((data[10] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[28] = ((data[10] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[29] = ((data[10] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[30] = ((data[10] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[3].CounterAccess[31] = ((data[10] & 0x80) == 0x80) ? 1 : 0;

            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[32] = ((data[11] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[33] = ((data[11] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[34] = ((data[11] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[35] = ((data[11] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[36] = ((data[11] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[37] = ((data[11] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[38] = ((data[11] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[4].CounterAccess[39] = ((data[11] & 0x80) == 0x80) ? 1 : 0;

            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[40] = ((data[12] & 0x01) == 0x01) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[41] = ((data[12] & 0x02) == 0x02) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[42] = ((data[12] & 0x04) == 0x04) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[43] = ((data[12] & 0x08) == 0x08) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[44] = ((data[12] & 0x10) == 0x10) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[45] = ((data[12] & 0x20) == 0x20) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[46] = ((data[12] & 0x40) == 0x40) ? 1 : 0;
            mcu_pc_databuffer.senior_MCU_PC_Data[5].CounterAccess[47] = ((data[12] & 0x80) == 0x80) ? 1 : 0;

            ////处理轴数据
            ////mcu_pc_data.AxisCurrentValue[0]=(((int)data[14]) << 8) | ((int)data[15]);
            ////mcu_pc_data.AxisCurrentValue[1]=(((int)data[16]) << 8) | ((int)data[17]);
            ////mcu_pc_data.AxisCurrentValue[2]=(((int)data[18]) << 8) | ((int)data[19]);
            //mcu_pc_databuffer.AxisCurrentValue[0] = data[14];
            //mcu_pc_databuffer.AxisCurrentValue[1] = data[16];
            //mcu_pc_databuffer.AxisCurrentValue[2] = data[18];


            Monitor.Enter(mcu_pc_data);
            //24个按钮键
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4;j++ )
                {
                    mcu_pc_data.senior_MCU_PC_Data[i].ButtonAccess[j] = mcu_pc_databuffer.senior_MCU_PC_Data[i].ButtonAccess[j];
                }        
            }
            //48个计数器
            for (int i = 0; i < 6;i++ )
            {
                for (int j = 0; j < 8;j++ )
                {
                    mcu_pc_data.senior_MCU_PC_Data[i].CounterAccess[j] += mcu_pc_databuffer.senior_MCU_PC_Data[i].CounterAccess[j];
                }   
            }
            ////处理轴数据
            //for (int i=0;i<3;i++)
            //{
            //    mcu_pc_data.AxisCurrentValue[i] = mcu_pc_databuffer.AxisCurrentValue[i];
            //    mcu_pc_data.AxisAccess[i]=mcu_pc_data.AxisAlu[i].Account(mcu_pc_data.AxisCurrentValue[i]);
            //}
            Monitor.Exit(mcu_pc_data);
            
        }
        private void WorkHande_MCU_PC()
        {
            //接收一次数据
            byte[] data = null;
            if (!RecvPortData(out data))
                return;//没有接收到数据
            //如果缓冲区本身有存在数据需要进行合并
            if (recvBuffer.Length != 0)
            {
                recvBuffer.Seek(0, SeekOrigin.End);
                recvBuffer.Write(data, 0, data.Length);
                data = recvBuffer.ToArray();
                recvBuffer.SetLength(0);
            }
            //开始拆包的过程
            int index = 0;
            byte[] package = new byte[MCU_PC_DATASIZE];
            while(SplitPackage(data,ref index,ref package))
            {
                HandlePackage(package);
            }
            if (index >= data.Length)//数据已经全部处理完了
            {
                return;
            }
            //有不完整的数据需要写入缓冲
            recvBuffer.Write(data, index, data.Length - index);
        }
        private void WorkHande_PC_MCU()
        {
            //产生一个数据包
            byte[] package = new byte[PC_MCU_DATASIZE];
            //包头
            package[0] = DataHead1;
            package[1] = DataHead2;
            //包尾
            package[PC_MCU_DATASIZE - 1] = DataLast;
            //命令
            package[2] = DataCommand;

            Monitor.Enter(pc_mcu_data);
            //只有发生了修改在复制，大部分的时候就不需要复制的
            if (pc_mcu_data.IsModify)
            {
                pc_mcu_data.CopyTo(pc_mcu_databuffer, true);
                pc_mcu_data.IsModify = false;
            }
            Monitor.Exit(pc_mcu_data);

            //输出指令
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[0] == 1) ? 0x01 : 0x00);
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[1] == 1) ? 0x02 : 0x00);
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[2] == 1) ? 0x04 : 0x00);
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[3] == 1) ? 0x08 : 0x00);
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[4] == 1) ? 0x10 : 0x00);
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[5] == 1) ? 0x20 : 0x00);
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[6] == 1) ? 0x40 : 0x00);
            package[4] |= (byte)((pc_mcu_databuffer.CommanderAccess[7] == 1) ? 0x80 : 0x00);

            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[8] == 1) ? 0x01 : 0x00);
            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[9] == 1) ? 0x02 : 0x00);
            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[10] == 1) ? 0x04 : 0x00);
            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[11] == 1) ? 0x08 : 0x00);
            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[12] == 1) ? 0x10 : 0x00);
            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[13] == 1) ? 0x20 : 0x00);
            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[14] == 1) ? 0x40 : 0x00);
            package[5] |= (byte)((pc_mcu_databuffer.CommanderAccess[15] == 1) ? 0x80 : 0x00);

            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[16] == 1) ? 0x01 : 0x00);
            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[17] == 1) ? 0x02 : 0x00);
            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[18] == 1) ? 0x04 : 0x00);
            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[19] == 1) ? 0x08 : 0x00);
            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[20] == 1) ? 0x10 : 0x00);
            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[21] == 1) ? 0x20 : 0x00);
            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[22] == 1) ? 0x40 : 0x00);
            package[6] |= (byte)((pc_mcu_databuffer.CommanderAccess[23] == 1) ? 0x80 : 0x00);
                
            //一直设置力反馈标志位为1，这样力反馈的数据持续启作用
            package[13] = 0x01;
            //14位是控制震动的
            //15位是控制基础力大小的

            //根据当前震动量来计算是否进行震动
            //震动量的符号表示震动的方向，震动量的大小表示震动的力度
            ////只取高8位
            //package[14] = (byte)(ShakeForceMidValue + (int)(pc_mcu_databuffer.shakeForce * (float)(ShakeForceMaxValue - ShakeForceMidValue)));
            ////设置基础力
            //package[15] = (byte)(BaseForceMinValue + (int)(pc_mcu_databuffer.BaseForce * (float)(BaseForceMaxValue - BaseForceMinValue)));


            //处理指令
            if (pc_mcu_databuffer.commandList.Count != 0)
            {
                PC_MCU_CommandData commandData = pc_mcu_databuffer.commandList[0];
                pc_mcu_databuffer.commandList.RemoveAt(0);
                //switch(commandData.command)
                //{
                //    case PC_MCU_Command.Command_ResetForceFeedback:
                //        package[8] = 0xFF;
                //        break;
                //    case PC_MCU_Command.Command_AdjustForceFeedback:
                //        {
                //            if (commandData.parameterList != null && commandData.parameterList.Length == 1)
                //            {
                //                package[8] = 0xF0;
                //                int ValueData = (int)commandData.parameterList[0];
                //                //这里只是用高8位的值，低8位舍弃掉
                //                package[9] = (byte)ValueData;
                //                //package[10] = (byte)(ValueData & 0x00FF);
                //            }
                //        }
                //        break;
                //}
            }
            //发生这个包
            SendPortData(package);
        }
        public void SerialPortAccess_DDR_WorkUpdate()
        {
            //接受数据
            WorkHande_RecvData();
            WorkHande_MCU_PC();
            WorkHande_PC_MCU();
            //处理完毕发送数据
            WorkHande_SendData();
        }
        private void SerialPortAccess_DDR_WorkUpdate(object source, System.Timers.ElapsedEventArgs e)
        {
            SerialPortAccess_DDR_WorkUpdate();
            accessworkCycleTimer.Enabled = true;
        }
    }
}
