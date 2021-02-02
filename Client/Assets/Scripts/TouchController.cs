using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
	public GameObject sender;
	private Vector3 posOpposite = new Vector3(0, 0, 0);
	private Vector3 posSelf = new Vector3(0, 0, 0);
	private float delayTimer = 0;
	private float delayTolerance = 2f;
	private SpriteRenderer spriteRenderer;
	private LineRenderer lineRenderer;
	// Start is called before the first frame update
	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.enabled = false;
		lineRenderer = this.gameObject.AddComponent<LineRenderer>();
		lineRenderer.alignment = LineAlignment.TransformZ;
		lineRenderer.SetWidth(0.1f, 0.1f);
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.positionCount = 2;
		lineRenderer.SetColors(new Color(1, 1, 1, 1), new Color(1, 1, 1, 1));
		lineRenderer.enabled = false;
	}

	// Update is called once per frame
	void Update() {
		delayTimer -= delayTolerance * Time.deltaTime;
		updatePoint();
		updateLine();
	}

	void updatePoint() {
		if (Input.touchCount > 0) {
			spriteRenderer.enabled = true;

			Touch touch = Input.GetTouch(0);
			posSelf = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
			transform.position = posSelf;

			sender.GetComponent<ClientController>().sendMessage(posSelf);

		}
		else if (Input.GetMouseButton(0)){
			spriteRenderer.enabled = true;

			posSelf = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
			transform.position = posSelf;

			sender.GetComponent<ClientController>().sendMessage(posSelf);

		}
		else {
			spriteRenderer.enabled = false;
		}
	}

	void updateLine() {
		if (spriteRenderer.enabled && delayTimer > 0) {
			lineRenderer.enabled = true;
			lineRenderer.SetPosition(0, posSelf);
			lineRenderer.SetPosition(1, posOpposite);
		}
		else {
			lineRenderer.enabled = false;
		}
	}

	public void receivePos(Vector3 p) {
		posOpposite = p + new Vector3(-Camera.main.aspect * Camera.main.orthographicSize * 2f, 0f, 0f);
		delayTimer = 1f;
		Debug.Log("receive opposite pos: " + p);
	}
}
