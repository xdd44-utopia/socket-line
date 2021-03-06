﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class ServerController : MonoBehaviour {

	public Text ipText;
	public Text rcvText;
	public GameObject touchProcessor;
	public GameObject frame;
	public GameObject faceTracker;
	public Camera renderCamera;

	private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
	private Color connectColor = new Color(0f, 0f, 0f);

	private Vector3 pos = new Vector3(0, 0, 0);
	private float angle = Mathf.PI / 2;
	private bool isRefreshed = false;

	private TcpListener tcpListener;
	private Thread tcpListenerThread;
	private TcpClient connectedTcpClient;
	private string rcvMsg = "";

	private bool noConnection = true;
	
	void Start () {
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncommingRequests));
		tcpListenerThread.IsBackground = true;
		tcpListenerThread.Start();
	}
	
	void Update () {
		ipText.text = getIPAddress();
		rcvText.text = rcvMsg;
		renderCamera.backgroundColor = (connectedTcpClient == null ? disconnectColor : connectColor);
		if (connectedTcpClient != null && noConnection) {
			sendMessage();
			noConnection = false;
		}
		if (isRefreshed) {
			touchProcessor.GetComponent<TouchProcessor>().pos = pos;
			frame.GetComponent<FrameController>().angle = angle;
			isRefreshed = false;
		}
	}
	
	private void ListenForIncommingRequests() {
		try {
			tcpListener = new TcpListener(IPAddress.Any, 8052);
			tcpListener.Start();
			Debug.Log("Server is listening");
			Byte[] bytes = new Byte[1024];
			while (true) {
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) {
					using (NetworkStream stream = connectedTcpClient.GetStream()) {
						int length;
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
							var incommingData = new byte[length];
							Array.Copy(bytes, 0, incommingData, 0, length);
							string clientMessage = Encoding.ASCII.GetString(incommingData);
							rcvMsg = clientMessage;
							getInfo(clientMessage);
						}
					}
				}
			}
		}
		catch (SocketException socketException) {
			Debug.Log("SocketException " + socketException.ToString());
		}
	}
	
	public void sendMessage() {
		if (connectedTcpClient == null) {
			return;
		}
		
		try {			
			NetworkStream stream = connectedTcpClient.GetStream();
			if (stream.CanWrite) {
				string serverMessage =
					touchProcessor.GetComponent<TouchProcessor>().pos.x + "," +
					touchProcessor.GetComponent<TouchProcessor>().pos.y + "," +
					touchProcessor.GetComponent<TouchProcessor>().pos.z + "," +
					faceTracker.GetComponent<FaceTracker>().currentObserve.x + "," +
					faceTracker.GetComponent<FaceTracker>().currentObserve.y + "," +
					faceTracker.GetComponent<FaceTracker>().currentObserve.z + ",";
				byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
				stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
				Debug.Log("Server sent his message - should be received by client");
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
			connectedTcpClient = null;
		}
	}
	
	private string getIPAddress() {
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				return ip.ToString();
			}
		}
		throw new System.Exception("No network adapters with an IPv4 address in the system!");
	}

	private void getInfo(string str) {
		string[] temp = str.Split(',');
		float x = System.Convert.ToSingle(temp[0]);
		float y = System.Convert.ToSingle(temp[1]);
		float z = System.Convert.ToSingle(temp[2]);
		pos = new Vector3(x, y, z);
		angle = System.Convert.ToSingle(temp[3]);
		isRefreshed = true;
	}

}