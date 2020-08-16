using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HelperInterface;

namespace InfinitiSerialPortDevelop
{
    public partial class Form1 : Form
    {
        private SerialPortInput serialPortInput = null;
        private SerialPortInputTrack serialPortInputTrack = null;
        private Timer updateTimer = null;
        ////轴校正算子
        //private SerialPortInputAxisAluAccelerator AcceleratorAlu = null;
        //private SerialPortInputAxisAluBrake brakeAlu = null;
        //private SerialPortInputAxisAluSteeringWheel steerAlu = null;
        //震动
        //private ShakeInterface shakeInterface = null;
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 256; i++)
            {
                this.comboBox1.Items.Add(string.Format("COM{0}", i));
            }
            this.comboBox1.SelectedIndex = 3;
            updateTimer = new Timer();
            updateTimer.Tick += new EventHandler(UpdateTimerEventProcessor);
            updateTimer.Interval = 16;
        }
        void UpdateTimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            if (serialPortInput == null)
                return;
            //计数器显示
            int counterCount = serialPortInput.CounterAccessCount;
            string[] lines = new string[counterCount+1];
            lines[0] = string.Format("{0} 路输入.", counterCount);
            for (int i = 0; i < counterCount;i++ )
            {
                if(i>=0 && i<= 3)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetCounterAccess(0,i).ToString());
                }
                if (i >= 4 && i <= 7)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetCounterAccess(1, i).ToString());
                }
                if (i >= 8 && i <= 11)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetCounterAccess(2, i).ToString());
                }
                if (i >= 12 && i <= 15)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetCounterAccess(3, i).ToString());
                }
                if (i >= 16 && i <= 19)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetCounterAccess(4, i).ToString());
                }
                if (i >= 20 && i <= 23)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetCounterAccess(5, i).ToString());
                }
                
                //int v = serialPortInput.GetCounterAccessReset(i);
            }
            richTextBox_Counter.Lines = lines;

            
            int buttonCount = serialPortInput.ButtonAccessCount;
            lines = new string[buttonCount + 1];
            lines[0] = string.Format("{0} 路输入.", buttonCount);
            for (int i = 0; i < buttonCount;i++ )
            {
                if (i >= 0 && i <= 7)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetButtonAccess(0, i).ToString());
                }
                if (i >= 8 && i <= 15)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetButtonAccess(1, i).ToString());
                }
                if (i >= 16 && i <= 23)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetButtonAccess(2, i).ToString());
                }
                if (i >= 24 && i <= 31)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetButtonAccess(3, i).ToString());
                }
                if (i >= 32 && i <= 39)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetButtonAccess(4, i).ToString());
                }
                if (i >= 40 && i <= 47)
                {
                    lines[i + 1] = string.Format("{0}路：{1} : {1}", i, serialPortInput.GetButtonAccess(5, i).ToString());
                }
            }
            richTextBox_Button.Lines = lines;

            //int axisCount = serialPortInput.AxisAccessCount;
            //lines = new string[axisCount + 1];
            //lines[0] = string.Format("{0} 路输入.", axisCount);
            //for (int i = 0; i < axisCount;i++ )
            //{
            //    lines[i + 1] = string.Format("{0}路:{1}", i, serialPortInput.GetAxisAccess(i));
            //}
            //richTextBox_Axis.Lines = lines;

            int commandCount = serialPortInput.CommanderAccessCount;
            lines = new string[commandCount + 1];
            lines[0] = string.Format("{0}路输出.", commandCount);
            for (int i = 0; i < commandCount;i++ )
            {
                lines[i + 1] = string.Format("{0}路：{1}", i, serialPortInput.GetCommanderAccess(i).ToString());
            }
            richTextBox_Command.Lines = lines;

            
            ////轴校正信息
            //float AxisV;
            //int AxisMinValue,AxisMaxValue,AxisValue;
            //AxisV=serialPortInput.GetAxisAccess(0);
            //serialPortInput.GetAxisAccessRange(0,out AxisMinValue,out AxisMaxValue);
            //AxisValue=serialPortInput.GetAxisAccessValue(0);
            //AxisAluSteeringWheelText.Text = string.Format("方向轴:{0},({1}[{2}-{3}])", AxisV, AxisValue, AxisMinValue, AxisMaxValue);

            //AxisV = serialPortInput.GetAxisAccess(1);
            //serialPortInput.GetAxisAccessRange(1, out AxisMinValue, out AxisMaxValue);
            //AxisValue = serialPortInput.GetAxisAccessValue(1);
            //AxisAluAcceleratorText.Text = string.Format("油门轴:{0},({1}[{2}-{3}])", AxisV, AxisValue, AxisMinValue, AxisMaxValue);

            //AxisV = serialPortInput.GetAxisAccess(2);
            //serialPortInput.GetAxisAccessRange(2, out AxisMinValue, out AxisMaxValue);
            //AxisValue = serialPortInput.GetAxisAccessValue(2);
            //AxisAluBrakeText.Text = string.Format("刹车轴:{0},({1}[{2}-{3}])", AxisV, AxisValue, AxisMinValue, AxisMaxValue);

            ////刷新震动力
            ////如果震动被激活了就刷新震动
            //if (shakeInterface != null)
            //{
            //    serialPortInput.UpdateShakeParameter(shakeInterface.type, ref shakeInterface.shakeParameter);
            //}
            //ShakeTitle.Text = string.Format("震动力(-1.0~1.0){0}", serialPortInput.shakeForce);

            //输出这次的信息
            List<string> traceList = serialPortInputTrack.LockTraceList();
            for (int i = 0; i < traceList.Count;i++ )
            {
                listBox1.Items.Add(traceList[i]);
            }
            traceList.Clear();
            serialPortInputTrack.UnlockTraceList();

            List<string> errList = serialPortInputTrack.LockErrList();
            for (int i = 0; i < errList.Count;i++ )
            {
                listBox1.Items.Add(errList[i]);
            }
            errList.Clear();
            serialPortInputTrack.UnlockErrList();
        }

        private void button_DeviceType_DDR_Click(object sender, EventArgs e)
        {
            if (serialPortInput != null)
                return;
            serialPortInputTrack = new SerialPortInputTrack();
            serialPortInput = new SerialPortInput();
            serialPortInput.traceObject = serialPortInputTrack;
            if (!serialPortInput.Initialization(SerialPortAccessType.Type_Treasure, comboBox1.SelectedIndex))
            {
                MessageBox.Show("设备连接失败!");
                serialPortInput.Release();
                serialPortInput = null;
            }


            //AcceleratorAlu = new SerialPortInputAxisAluAccelerator();
            //serialPortInput.GetAxisAccessAluAccelerator(AcceleratorAlu);
            //AxisAluAcceleratorUp.Text = AcceleratorAlu.UpValue.ToString();
            //AxisAluAcceleratorDown.Text = AcceleratorAlu.DownValue.ToString();

            //brakeAlu = new SerialPortInputAxisAluBrake();
            //serialPortInput.GetAxisAccessAluBrake(brakeAlu);
            //AxisAluBrakeUp.Text = brakeAlu.UpValue.ToString();
            //AxisAluBrakeDown.Text = brakeAlu.DownValue.ToString();


            //steerAlu = new SerialPortInputAxisAluSteeringWheel();
            //serialPortInput.GetAxisAccessAluSteeringWheel(steerAlu);
            //AxisAluSteeringWheelLeft.Text = steerAlu.LeftValue.ToString();
            //AxisAluSteeringWheelRight.Text = steerAlu.RightValue.ToString();

            //BaseForce.Text = serialPortInput.BaseForce.ToString();
            //ShakeTitle.Text = string.Format("震动力(-1.0~1.0){0}", serialPortInput.shakeForce);

            comboBoxShakeType.SelectedIndex = 0;
            //shakeInterface = new ShakeInterface(ShakeType.ShakeType_Sin, 0.0f, 0.0f, 1.0f, 0.0f);
            //给定当前设备的刷新时间
            updateTimer.Interval = serialPortInput.DeviceUpdatetimes;
            updateTimer.Start();
        }

        private void button_Command_Click(object sender, EventArgs e)
        {
            if (serialPortInput == null)
                return;
            string[] command=textBox_Command.Text.Split(',');
            serialPortInput.SetCommanderAccess(Convert.ToInt32(command[0]), Convert.ToInt32(command[1]) == 1 ? true : false);
            //bool b = serialPortInput.GetCommanderAccess(Convert.ToInt32(command[0]));
        }
        private void button_DeviceType_DDR_DIS_Click(object sender, EventArgs e)
        {
            if (serialPortInput != null)
            {
                updateTimer.Stop();

                richTextBox_Counter.Lines = null;
                richTextBox_Button.Lines = null;
                //richTextBox_Axis.Lines = null;
                richTextBox_Command.Lines = null;
        

                serialPortInput.Release();
                serialPortInput = null;
                serialPortInputTrack = null;

                //AcceleratorAlu = null;
                //brakeAlu = null;
                //steerAlu = null;
                //shakeInterface = null;
            }
        }

        private void button_ClearListBox_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        //private void AxisAluModify_Click(object sender, EventArgs e)
        //{
        //    AcceleratorAlu.UpValue = Convert.ToInt32(AxisAluAcceleratorUp.Text);
        //    AcceleratorAlu.DownValue = Convert.ToInt32(AxisAluAcceleratorDown.Text);
        //    serialPortInput.SetAxisAccessAluAccelerator(AcceleratorAlu);

        //    brakeAlu.UpValue=Convert.ToInt32(AxisAluBrakeUp.Text);
        //    brakeAlu.DownValue=Convert.ToInt32(AxisAluBrakeDown.Text);
        //    serialPortInput.SetAxisAccessAluBrake(brakeAlu);

        //    steerAlu.LeftValue=Convert.ToInt32(AxisAluSteeringWheelLeft.Text);
        //    steerAlu.RightValue=Convert.ToInt32(AxisAluSteeringWheelRight.Text);
        //    serialPortInput.SetAxisAccessAluSteeringWheel(steerAlu);

        //}

        //private void ShakeModify1_Click(object sender, EventArgs e)
        //{

        //    serialPortInput.BaseForce = Convert.ToSingle(BaseForce.Text);
        //    serialPortInput.shakeForce = Convert.ToSingle(OffsetForce.Text);
        //}

        //private void ShakeModify2_Click(object sender, EventArgs e)
        //{
        //    if (comboBoxShakeType.SelectedIndex == 0)
        //    {
        //        shakeInterface = null;
        //        serialPortInput.StopShake();
        //    }
        //    else
        //    {
        //        if (shakeInterface == null)
        //        {
        //            shakeInterface = new ShakeInterface(ShakeType.ShakeType_Sin, 0.0f, 0.0f, 0.0f,1.0f, 0.0f);
        //        }
        //        if (comboBoxShakeType.SelectedIndex == 1)
        //        {
        //            shakeInterface.type = ShakeType.ShakeType_Sin;
        //        }
        //        else if (comboBoxShakeType.SelectedIndex == 2)
        //        {
        //            shakeInterface.type = ShakeType.ShakeType_Cos;
        //        }
        //        shakeInterface.shakeForce = Convert.ToSingle(shakeForce.Text);
        //        shakeInterface.shakeForceDecay = Convert.ToSingle(shakeForceDecay.Text);
        //        shakeInterface.shakeOffsetForce = Convert.ToSingle(sakeOffsetForce.Text);
        //        shakeInterface.shakePeriod = Convert.ToSingle(shakePeriod.Text);
        //        shakeInterface.shakeStartRadian = Convert.ToSingle(shakeStartRadian.Text);
        //    }

        //}
    }
}