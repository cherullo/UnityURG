using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using SCIP_library;
using UnityEngine;

public enum CMD
{
    // https://www.hokuyo-aut.jp/02sensor/07scanner/download/pdf/URG_SCIP20.pdf
    VV, PP, II, // センサ情報要求コマンド(3 種類)  
    BM, QT, //計測開始・終了コマンド
    MD, GD, // 距離要求コマンド(2 種類) 
    ME //距離・受光強度要求コマンド 
}

public abstract class UrgDevice : MonoBehaviour
{
    public List<long> distances;
    public List<long> strengths;

    private Stream _stream;
    private Thread _clientThread;

    public void StartDevice()
    {
        distances = new List<long>();
        strengths = new List<long>();

        _stream = OpenDeviceStream();

        ListenForClients();
    }

    protected abstract Stream OpenDeviceStream();

    private void ListenForClients()
    {
        _clientThread = new Thread(new ThreadStart(HandleClientComm));
        _clientThread.Start();
    }

    private void HandleClientComm()
    {
        while (true)
        {
            try
            {
                long time_stamp = 0;
                string receive_data = read_line(_stream);

                string cmd = GetCommand(receive_data);
                if (cmd == GetCMDString(CMD.MD))
                {
                    distances.Clear();
                    SCIP_Reader.MD(receive_data, ref time_stamp, ref distances);
                }
                else if (cmd == GetCMDString(CMD.ME))
                {
                    distances.Clear();
                    strengths.Clear();
                    SCIP_Reader.ME(receive_data, ref time_stamp, ref distances, ref strengths);
                }
                else
                {
                    Debug.Log(">>" + receive_data);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("error: " + ex);
            }
        }
    }

    private string GetCommand(string get_command)
    {
        string[] split_command = get_command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        return split_command[0].Substring(0, 2);
    }

    public virtual void Write(string line)
	{
		write_line(_stream, line);
	}

    void OnDisable()
    {
        StopDevice();
    }
    void OnApplicationQuit()
    {
        StopDevice();
    }

    public virtual void StopDevice()
    {
		if (_clientThread != null)
        {
            _clientThread.Abort();
			_clientThread = null;
        }

        if (_stream != null)
        {
            _stream.Close();
            _stream = null;
        }
    }

    /// <summary>
    /// Read to "\n\n" from NetworkStream
    /// </summary>
    /// <returns>receive data</returns>
    private string read_line(Stream stream)
    {
        if (stream.CanRead)
        {
            StringBuilder sb = new StringBuilder();
            bool is_NL2 = false;
            bool is_NL = false;
            do
            {
                char buf = (char)stream.ReadByte();
                if (buf == '\n')
                {
                    if (is_NL)
                    {
                        is_NL2 = true;
                    }
                    else
                    {
                        is_NL = true;
                    }
                }
                else
                {
                    is_NL = false;
                }
                sb.Append(buf);
            } while (!is_NL2);

            return sb.ToString();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// write data
    /// </summary>
    private bool write_line(Stream stream, string data)
    {
        if (stream.CanWrite)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            stream.Write(buffer, 0, buffer.Length);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string GetCMDString(CMD cmd)
    {
        return cmd.ToString();
    }
}
