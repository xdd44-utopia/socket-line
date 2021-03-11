using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelController : MonoBehaviour
{
	public GameObject renderCam;
	public GameObject faceTracker;
	public GameObject touchProcessor;
	public Texture texture;

	private MeshRenderer meshRenderer;

	private Vector3 pos;

	void Start() {
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find("Custom/ClipShader"));
		meshRenderer.material.SetTexture("_MainTex", texture);
	}

	void Update() {
		renderCam.transform.position = faceTracker.GetComponent<FaceTracker>().currentObserve;
		pos = touchProcessor.GetComponent<TouchProcessor>().pos;
		meshRenderer.material.SetFloat("_OffsetX", pos.x);
		meshRenderer.material.SetFloat("_OffsetY", pos.y);
		meshRenderer.material.SetFloat("_OffsetZ", pos.z);
	}

}
