using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class URGTcpClient : MonoBehaviour {

    private TcpClient tcpClient;
    private Thread receiveThread;

    public Action<string> OnReceiveCallback;
    //public Action<int> OnSendCallback;
    string ip;
    int port;
    public void Open(string ip, int port) {
        try {
            this.ip = ip;
            this.port = port;
            tcpClient = new TcpClient();
            tcpClient.ReceiveTimeout = 3000;
            tcpClient.SendTimeout = 3000;
            
            receiveThread = new Thread(new ThreadStart(ReceiveListener));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        
        } catch (Exception e) {
            Debug.Log("Exception: " + e.Message);
        }
    }

    void ReceiveListener() {
        try {
            tcpClient.Connect(ip, port);
            using (NetworkStream stream = tcpClient.GetStream()) {
                while (true) {
                    string receive_data = read_line(stream);
                    //Debug.Log("receive: " + receive_data);
                    if (OnReceiveCallback != null) {
                        OnReceiveCallback(receive_data);
                    }
                
                }
            }
        } catch (SocketException socketException) {
            Debug.Log("Socket exception: " + socketException);
        }
    }

 /*!
 * \file
 * \brief Get distance data from Ethernet type URG
 * \author Jun Fujimoto
 * $Id: get_distance_ethernet.cs 403 2013-07-11 05:24:12Z fujimoto $
 */
    /// <summary>
    /// Read to "\n\n" from NetworkStream
    /// </summary>
    /// <returns>receive data</returns>
    string read_line(NetworkStream stream) {
        if (stream.CanRead) {
            StringBuilder sb = new StringBuilder();
            bool is_NL2 = false;
            bool is_NL = false;
            do {
                char buf = (char)stream.ReadByte();
                if (buf == '\n') {
                    if (is_NL) {
                        is_NL2 = true;
                    } else {
                        is_NL = true;
                    }
                } else {
                    is_NL = false;
                }
                sb.Append(buf);
            } while (!is_NL2);

            return sb.ToString();
        } else {
            return null;
        }
    }
    public bool GetIsConnected() {
        if (tcpClient == null) {
            return false;
        }

        return tcpClient.Connected;
    }
        public void Send(string msg) {
        if (tcpClient == null || !tcpClient.Connected) {
            return;
        }
        try {
            // Get a stream object for writing. 			
            NetworkStream stream = tcpClient.GetStream();
            if (stream.CanWrite) {
                //string clientMessage = "This is a message from one of your clients.";
                // Convert string message to byte array.                 
                byte[] bytes = Encoding.ASCII.GetBytes(msg);
                // Write byte array to socketConnection stream.   
                stream.Write(bytes, 0, bytes.Length);
                Debug.Log("send: " + msg);
            }
        } catch (SocketException socketException) {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    void OnDestroy() {

        if (this.receiveThread != null) {
            this.receiveThread.Abort();
        }

        if (tcpClient != null) {
            if (tcpClient.Connected) {
                NetworkStream stream = tcpClient.GetStream();
                if (stream != null) {
                    stream.Close();
                }
            }
            tcpClient.Close();
        }


    }
}