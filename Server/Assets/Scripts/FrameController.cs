using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour
{
	public GameObject faceTracker;
	private float lineWidth = 0.025f;


	private float camWidth;
	private float camHeight;

	[HideInInspector]
	public float angle;
	private LineRenderer lineRenderer;
	private Vector3 observe;
	// Start is called before the first frame update
	void Start()
	{
		lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		lineRenderer.alignment = LineAlignment.TransformZ;
		lineRenderer.startWidth = lineWidth;
		lineRenderer.useWorldSpace = false;
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.positionCount = 4;
		lineRenderer.startColor = new Color(1, 1, 1, 1);
		lineRenderer.endColor = new Color(1, 1, 1, 1);
		angle = 2 * Mathf.PI / 3;
	}

	// Update is called once per frame
	void Update()
	{
		Camera cam = Camera.main;
		camHeight = cam.orthographicSize;
		camWidth = camHeight * cam.aspect;
		observe = faceTracker.GetComponent<FaceTracker>().currentObserve;

		transform.localPosition = new Vector3(0, 0, -observe.z);

		Vector3 camPos = new Vector3(observe.x, observe.y, 0);
		lineRenderer.SetPosition(0, new Vector3(
			camWidth,
			- camHeight,
			0
		));
		lineRenderer.SetPosition(1, getProjection(new Vector3(
			camWidth - 2 * camWidth * Mathf.Cos(Mathf.PI - angle),
			- camHeight,
			2 * camWidth * Mathf.Sin(Mathf.PI - angle)
		)));
		lineRenderer.SetPosition(2, getProjection(new Vector3(
			camWidth - 2 * camWidth * Mathf.Cos(Mathf.PI - angle),
			camHeight,
			2 * camWidth * Mathf.Sin(Mathf.PI - angle)
		)));
		lineRenderer.SetPosition(3, new Vector3(
			camWidth,
			camHeight,
			0
		));

	}
	Vector3 getProjection(Vector3 v) {
		v -= new Vector3(observe.x, observe.y, 0);
		v.x = v.x / (observe.z - v.z) * observe.z;
		v.y = v.y / (observe.z - v.z) * observe.z;
		v += new Vector3(observe.x, observe.y, 0);
		return v;
	}
}
