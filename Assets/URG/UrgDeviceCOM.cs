
using System.IO;
using System.IO.Ports;

using UnityEngine;

public class UrgDeviceCOM : UrgDevice
{
    [SerializeField]
    private string _COMPort;

    [SerializeField]
    private int _baudRate;

    private SerialPort _serialPort;

    protected override Stream OpenDeviceStream()
    {
        _serialPort =
            new SerialPort(_COMPort, _baudRate, Parity.None, 8, StopBits.One);

        // serialPort.ReadTimeout = 25;

        _serialPort.Open();

        return _serialPort.BaseStream;
    }

    public override void StopDevice()
    {
        base.StopDevice();

        if (_serialPort != null)
        {
            _serialPort.Close();
            _serialPort = null;
        }
    }
}