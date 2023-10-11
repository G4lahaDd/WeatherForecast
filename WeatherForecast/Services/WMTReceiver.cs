using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Windows.Data;

namespace WeatherForecast.Services
{ 
    public class WMTReceiver : SerialPort
    {
        private readonly Logger logger;

        private Thread readThread;
        private List<byte> buffer = new List<byte>();

        public bool opened = false;
        public event ReceiveData ReceiveForecastData;

        public WMTReceiver(string port) : base(port)
        {
            logger = Logger.Instance;
            logger.LogInfo<WMTReceiver>($"Create WMT700 Data receiver, port: {port}");

            base.BaudRate = 2400;
            base.DataBits = 8;
            base.StopBits = StopBits.One;

            readThread = new Thread(Read);
        }

        public new void Open()
        {
            logger.LogInfo<WMTReceiver>($"Open serial port connection");
            try
            {
                if (base.IsOpen)
                {
                    base.Close();
                }

                base.Open();

                opened = true;
                readThread?.Start();
            }
            catch(Exception ex)
            {
                logger.LogError<WMTReceiver>(ex);
                throw ex;
            }
        }

        public new void Close()
        {
            logger.LogInfo<WMTReceiver>($"Close WMT700 Data receiver");
            opened = false;
            if(readThread.IsAlive)
                readThread?.Join();

            base.Close();
        }

        private void Read()
        {
            while(opened)
            {
                if(BytesToRead > 0)
                {
                   int value = ReadByte();
                    switch (value)
                    {
                        case 10:
                            if(buffer.Count > 0 && buffer[0] == 36)
                            {
                                ReceiveForecastData?.Invoke(Encoding.UTF8.GetString(buffer.ToArray()));
                            }
                            buffer.Clear();
                            break;
                        case 36:
                            buffer.Clear();
                            buffer.Add((byte)value);
                            break;
                        default:
                            buffer.Add((byte)value);
                            break;
                    }
                }
            }
        }

        public delegate void ReceiveData(string data);
    }
}
