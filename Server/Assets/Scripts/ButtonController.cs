using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public int buttonNum;
	public GameObject obj;
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		
	}
	public void OnPointerDown(PointerEventData eventData){
		switch (buttonNum) {
			case 0: obj.GetComponent<FaceTracker>().increaseX = true; break;
			case 1: obj.GetComponent<FaceTracker>().decreaseX = true; break;
			case 2: obj.GetComponent<FaceTracker>().increaseY = true; break;
			case 3: obj.GetComponent<FaceTracker>().decreaseY = true; break;
			case 4: obj.GetComponent<FaceTracker>().increaseZ = true; break;
			case 5: obj.GetComponent<FaceTracker>().decreaseZ = true; break;
		}
	}
	
	public void OnPointerUp(PointerEventData eventData){
		switch (buttonNum) {
			case 0: obj.GetComponent<FaceTracker>().increaseX = false; break;
			case 1: obj.GetComponent<FaceTracker>().decreaseX = false; break;
			case 2: obj.GetComponent<FaceTracker>().increaseY = false; break;
			case 3: obj.GetComponent<FaceTracker>().decreaseY = false; break;
			case 4: obj.GetComponent<FaceTracker>().increaseZ = false; break;
			case 5: obj.GetComponent<FaceTracker>().decreaseZ = false; break;
		}
	}
}
