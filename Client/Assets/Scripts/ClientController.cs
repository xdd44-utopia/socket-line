using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ClientController : MonoBehaviour {

	public SpriteRenderer sr;
	public GameObject touchPoint;

	private Color disconnectColor = new Color(0.8156f, 0.3529f, 0.4313f);
	private Color connectColor = new Color(0.5254f, 0.7568f, 0.4f);

	private Vector3 posOpposite = new Vector3(0, 0, 0);
	private bool isRefreshed = false;

	private TcpClient socketConnection;
	private Thread clientReceiveThread;
	
	void Start () {
		socketConnection = null;
		ConnectToTcpServer("127.0.0.1");
	}
	
	void Update () {
		sr.color = (socketConnection == null ? disconnectColor : connectColor);
		if (isRefreshed) {
			touchPoint.GetComponent<TouchController>().receivePos(posOpposite);
			isRefreshed = false;
		}
	}
	
	public void ConnectToTcpServer (string ipText) {
		try {
			clientReceiveThread = new Thread(() => ListenForData(ipText));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e) {
			Debug.Log("On client connect exception " + e);
		}
	}
	
	private void ListenForData(string ipText) {
		socketConnection = null;
		try {
			socketConnection = new TcpClient(ipText, 8052);
			Byte[] bytes = new Byte[1024];
			while (true) {
				using (NetworkStream stream = socketConnection.GetStream()) {
					int length;
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						string serverMessage = Encoding.ASCII.GetString(incommingData);
						posOpposite = getVector(serverMessage);
						isRefreshed = true;
					}
				}
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}
	
	public void sendMessage(Vector3 pos) {
		if (socketConnection == null) {
			return;
		}
		try {		
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite) {
				string clientMessage = pos.x + "," + pos.y + ",";
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}
	}

	private Vector3 getVector(string str) {
		string[] temp = str.Split(',');
		float x = System.Convert.ToSingle(temp[0]);
		float y = System.Convert.ToSingle(temp[1]);
		Vector3 v = new Vector3(x, y, 0);
		return v;
	}
}