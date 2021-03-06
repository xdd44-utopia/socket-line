using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelController : MonoBehaviour
{
	public GameObject sender;
	public GameObject renderCam;
	public Texture texture;

	private MeshRenderer meshRenderer;

	private string address = "172.20.10.6";
	//Macbook local connecting to iPhone hotspot: 172.20.10.2
	//Samsung connecting to iPhone hotspot: 172.20.10.6
	//Macbook local connecting to xdd44's wifi: 192.168.0.101
	//iPhone connecting to iPhone hotspot: 10.150.153.190

	private float camWidth;
	private float camHeight;
	private float angle = 2 * Mathf.PI / 3;
	
	[HideInInspector]
	public Vector3 pos = new Vector3(0, 0, 0f);
	private Vector3 defaultPos = new Vector3(0, 0, 0.25f);
	private Vector3 prevTouch;
	private float moveSensitive = 0.001f;

	private Vector3 observe = new Vector3(0, 0, -5f);
	private Vector3 defaultObserve = new Vector3(0, 0, -5f);
	private float smoothSpeed = 25f;
	private float smoothTolerance = 0.1f;
	
	void Start() {
		Camera cam = Camera.main;
		camHeight = 2f * cam.orthographicSize;
		camWidth = camHeight * cam.aspect;

		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find("Custom/ClipShader"));
		meshRenderer.material.SetTexture("_MainTex", texture);
	}

	void Update() {
		updateObservation();
		updateFov();
		updatePos();
	}

	void updateObservation() {
		if (Vector3.Distance(renderCam.transform.position, observe) > smoothTolerance) {
			renderCam.transform.position = Vector3.Lerp(renderCam.transform.position, observe, smoothSpeed * Time.deltaTime);
		}
	}

	void updateFov() {
		Camera cam = renderCam.GetComponent<Camera>();
		float fovHorizontal = Mathf.Atan(-(Mathf.Abs(renderCam.transform.position.x) + camWidth / 2) / renderCam.transform.position.z) * 2;
		fovHorizontal = fovHorizontal * 180 / Mathf.PI;
		fovHorizontal = Camera.HorizontalToVerticalFieldOfView(fovHorizontal, cam.aspect);
		float fovVertical = Mathf.Atan(-(Mathf.Abs(renderCam.transform.position.y) + camHeight / 2) / renderCam.transform.position.z) * 2;
		fovVertical = fovVertical * 180 / Mathf.PI;
		cam.fieldOfView = (fovVertical > fovHorizontal ? fovVertical : fovHorizontal);
	}

	void updatePos() {
		if (Input.touchCount > 0) {
			Touch touch = Input.GetTouch(0);
			if (touch.position.y < 850f && touch.position.y > 300f) {
				Vector2 tp = new Vector2(touch.position.x, touch.position.y);
				switch (touch.phase) {
					case TouchPhase.Began:
						prevTouch = tp;
						break;
					case TouchPhase.Moved:
						pos += moveSensitive * (new Vector3(tp.x, tp.y, 0) - new Vector3(prevTouch.x, prevTouch.y, 0));
						prevTouch = tp;
						sender.GetComponent<ClientController>().sendMessage(convertToServer(pos));
						break;
				}
			}
		}
		if (Input.GetMouseButtonDown(0) && Input.mousePosition.y < 2500f && Input.mousePosition.y > 300f) {
			Vector2 tp = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			prevTouch = tp;
		}
		else if (Input.GetMouseButton(0) && Input.mousePosition.y < 2500f && Input.mousePosition.y > 300f) {
			Vector2 tp = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			pos += moveSensitive * (new Vector3(tp.x, tp.y, 0) - new Vector3(prevTouch.x, prevTouch.y, 0));
			prevTouch = tp;
			sender.GetComponent<ClientController>().sendMessage(convertToServer(pos));
		}
		meshRenderer.material.SetFloat("_OffsetX", pos.x);
		meshRenderer.material.SetFloat("_OffsetY", pos.y);
		meshRenderer.material.SetFloat("_OffsetZ", pos.z);
		meshRenderer.material.SetFloat("_RotateY", angle - Mathf.PI);
	}

	private Vector3 convertFromServer(Vector3 v) {
		Vector3 origin = new Vector3(camWidth / 2 + camWidth * Mathf.Cos(Mathf.PI - angle) / 2, 0, - camWidth * Mathf.Sin(Mathf.PI - angle) / 2);
		Vector3 x = new Vector3(Mathf.Cos(Mathf.PI - angle), 0, - Mathf.Sin(Mathf.PI - angle));
		Vector3 z = new Vector3(Mathf.Cos(angle - Mathf.PI / 2), 0, Mathf.Sin(angle - Mathf.PI / 2));
		v -= origin;
		return new Vector3(multXZ(v, x), v.y, multXZ(v, z));
	}

	private Vector3 convertToServer(Vector3 v) {
		Vector3 origin = new Vector3(- camWidth / 2 - camWidth * Mathf.Cos(Mathf.PI - angle) / 2, 0, - camWidth * Mathf.Sin(Mathf.PI - angle) / 2);
		Vector3 x = new Vector3(Mathf.Cos(Mathf.PI - angle), 0, Mathf.Sin(Mathf.PI - angle));
		Vector3 z = new Vector3(-Mathf.Cos(angle - Mathf.PI / 2), 0, Mathf.Sin(angle - Mathf.PI / 2));
		v -= origin;
		return new Vector3(multXZ(v, x), v.y, multXZ(v, z));
	}

	private float multXZ(Vector3 from, Vector3 to) {
		return from.x * to.x + from.z * to.z;
	}

	public void receivePos(Vector3 p) {
		pos = convertFromServer(p);
	}

	public void receiveView(Vector3 p) {
		observe = convertFromServer(p);
	}

	public void resetAll() {
		pos = defaultPos;
		observe = defaultObserve;
		sender.GetComponent<ClientController>().ConnectToTcpServer(address);
	}

}
