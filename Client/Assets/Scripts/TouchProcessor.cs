using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchProcessor : MonoBehaviour
{
	public GameObject sender;

	private string address = "192.168.0.106";
	//Macbook local connecting to iPhone hotspot: 172.20.10.2
	//Samsung connecting to iPhone hotspot: 172.20.10.6
	//Samsung connecting to xdd44's wifi: 192.168.0.106
	//Macbook local connecting to xdd44's wifi: 192.168.0.101
	//iPhone connecting to iPhone hotspot: 10.150.153.190

	[HideInInspector]
	public Vector3 pos = new Vector3(0, 0, 0);
	private Vector3 defaultPos = new Vector3(0, 0, 0.25f);
	private Vector3 prevTouch;
	private bool valid;
	private float moveSensitive = 0.001f;
	private float angleSensitive = 0.002f;

	[HideInInspector]
	public float angle;
	private const float minAngle = Mathf.PI / 2;
	private const float maxAngle = Mathf.PI;
	// Start is called before the first frame update
	void Start()
	{
		angle = 2 * Mathf.PI / 3;
	}

	// Update is called once per frame
	void Update()
	{
		bool isStartingTouching = false;
		bool isTouching = false;
		Vector2 tp = new Vector2(2000, 2000);

		if (Input.touchCount > 0) {
			tp = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
			isTouching = true;
			isStartingTouching = (Input.GetTouch(0).phase == TouchPhase.Began);
		}
		if (Input.GetMouseButton(0)) {
			tp = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			isTouching = true;
			isStartingTouching = Input.GetMouseButtonDown(0);
		}

		if (!valid && tp.y < 1400f) {
			prevTouch = tp;
		}
		valid = (tp.y < 1400f);
		if (isTouching) {
			if (!valid) {
				prevTouch = tp;
			}
			else {
				if (tp.y > 150) {
					pos += moveSensitive * (new Vector3(tp.x, tp.y, 0) - new Vector3(prevTouch.x, prevTouch.y, 0));
				}
				else {
					angle -= angleSensitive * (tp.x - prevTouch.x);
					angle = angle > minAngle ? angle : minAngle;
					angle = angle < maxAngle ? angle : maxAngle;
				}
				prevTouch = tp;
				sender.GetComponent<ClientController>().sendMessage();
			}
		}

	}

	public void resetAll() {
		pos = defaultPos;
		sender.GetComponent<ClientController>().ConnectToTcpServer(address);
	}
}
