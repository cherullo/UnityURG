/*!
 * \file
 * \brief Get distance data from Ethernet type URG
 * \author Jun Fujimoto
 * $Id: get_distance_ethernet.cs 403 2013-07-11 05:24:12Z fujimoto $
 */
using UnityEngine;
using System.Threading;

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

public class UrgDeviceEthernet : UrgDevice
{
    [SerializeField]
    private string ip_address = "192.168.0.10";
    [SerializeField]
    private int port_number = 10940;

    TcpClient _tcpClient;

    protected override Stream OpenDeviceStream()
    {
        try
        {
            Debug.Log("Connect setting = IP Address : " + ip_address + " Port number : " + port_number.ToString());

            _tcpClient = new TcpClient();
            _tcpClient.Connect(ip_address, port_number);

            return _tcpClient.GetStream();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return null;
    }

	public override void StopDevice()
	{
		base.StopDevice();

		if (_tcpClient != null)
		{
			_tcpClient.Close();
			_tcpClient = null;
		}
	}

    // bool CheckCommand(string get_command, string cmd)
    // {
    //     string[] split_command = get_command.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
    //     return split_command[0].StartsWith(cmd);
    // }

    //	void Update()
    //	{
    //		lock(messageQueue.SyncRoot){
    //			if(messageQueue.Count > 0){
    //				string receive_data = messageQueue.Dequeue().ToString();
    //				long time_stamp;
    //				if(CheckCommand(receive_data, "MD")){
    //					distances.Clear();
    //					time_stamp = 0;
    //
    //					SCIP_Reader.MD(receive_data, ref time_stamp, ref distances);
    //					//Debug.Log("time stamp: " + time_stamp.ToString() + " / count: "+distances.Count);
    //				}else if(CheckCommand(receive_data, "GD")){
    //					distances.Clear();
    //					time_stamp = 0;
    //
    //					SCIP_Reader.GD(receive_data, ref time_stamp, ref distances);
    //				}else{
    //					Debug.Log(">>"+receive_data);
    //				}
    //			}
    //		}
    //		
    //	}

}