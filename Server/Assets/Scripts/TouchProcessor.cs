﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchProcessor : MonoBehaviour
{
	public GameObject sender;

	[HideInInspector]
	public Vector3 pos = new Vector3(0, 0, 0);
	private Vector3 defaultPos = new Vector3(0, 0, 0.25f);
	private Vector3 prevTouch;
	private float moveSensitive = 0.001f;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
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
						sender.GetComponent<ServerController>().sendMessage();
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
			sender.GetComponent<ServerController>().sendMessage();
		}
	}

	public void resetAll() {
		pos = defaultPos;
	}
}
