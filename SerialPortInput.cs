using System;
using System.Collections.Generic;
using System.Text;

namespace HelperInterface
{
    public enum SerialPortAccessType
    {
        Type_Initial_D_5,       //ͷ����D5
        Type_DDR,              //���쭳�
        Type_Treasure          //����ı���
    }
    //�����ͷ���
    public enum AxisFunctionType
    {
        FunctionType_Accelerator        =0,         //����
        FunctionType_Brake              =1,         //ɲ��
        FunctionType_SteeringWheel      =2          //������
    }
    //�������ͷ���
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

        //�趨���豸ˢ��ʱ�䣬��λ����
        public int DeviceUpdatetimes { get { return serialPortAccess.DeviceUpdatetimes; } }
        //�������߿���,�������ߵ�ֵ���ۼ��͵ģ�������������һֱ�ۼ�
        public int CounterAccessCount { get { return serialPortAccess.CounterAccessCount; } }
        public int GetCounterAccess(int index1, int index2)
        {
            return serialPortAccess.GetCounterAccess(index1, index2);
        }
        public int GetCounterAccessReset(int index1, int index2)
        {
            return serialPortAccess.GetCounterAccessReset(index1, index2);
        }
        //��ť����
        public int ButtonAccessCount { get { return serialPortAccess.ButtonAccessCount; } }
        public bool GetButtonAccess(int index1, int index2)
        {
            return serialPortAccess.GetButtonAccess(index1, index2);
        }
        ////��ֵ����
        //public int AxisAccessCount { get { return serialPortAccess.AxisAccessCount; } }
        //public float GetAxisAccess(int index)
        //{
        //    return serialPortAccess.GetAxisAccess(index);
        //}
        ////��ȡ��ֵ��
        //public void GetAxisAccessRange(int index, out int min, out int max)
        //{
        //    serialPortAccess.GetAxisAccessRange(index, out min, out max);
        //}
        ////��ȡ��ĵ�ǰ��λֵ
        //public int GetAxisAccessValue(int index)
        //{
        //    return serialPortAccess.GetAxisAccessValue(index);
        //}
        ////����������
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
        //���ָ�����
        public int CommanderAccessCount { get { return serialPortAccess.CounterAccessCount; } }
        public bool GetCommanderAccess(int index)
        {
            return serialPortAccess.GetCommanderAccess(index);
        }
        public void SetCommanderAccess(int index, bool v)
        {
            serialPortAccess.SetCommanderAccess(index, v);
        }
        //����������
        //��������С0.0~1.0
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
        ////��������С-1.0~1.0,����Ϊ����
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
        //ˢ���𶯺��������������Ҫ��ÿ�������ڳ������ã�����������
        //private HelperInterfaceInternal.ShakeBase currentShakeInfo = null;
        //public void UpdateShakeParameter(HelperInterface.ShakeType type, ref HelperInterface.ShakeParameter shakeParameter)
        //{
        //    try
        //    {
        //        if (type == ShakeType.ShakeType_None)//ֹͣ��
        //        {
        //            currentShakeInfo = null;
        //            shakeForce = 0.0f;
        //        }
        //        else
        //        {
        //            //����û�иı���ֱ��ˢ��һ�β���
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
        //            //��������
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
