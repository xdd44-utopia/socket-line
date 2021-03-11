using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelController : MonoBehaviour
{
	public GameObject renderCam;
	public GameObject touchProcessor;
	public Texture texture;

	private MeshRenderer meshRenderer;

	[HideInInspector]
	public float angle;
	
	private Vector3 pos = new Vector3(0, 0, 0f);

	[HideInInspector]
	public Vector3 observe;
	
	void Start() {
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find("Custom/ClipShader"));
		meshRenderer.material.SetTexture("_MainTex", texture);

		angle = 2 * Mathf.PI / 3;
		observe = new Vector3(0, 0, -5);
	}

	void Update() {
		renderCam.transform.position = observe;
		pos = touchProcessor.GetComponent<TouchProcessor>().pos;
		meshRenderer.material.SetFloat("_OffsetX", pos.x);
		meshRenderer.material.SetFloat("_OffsetY", pos.y);
		meshRenderer.material.SetFloat("_OffsetZ", pos.z);
		meshRenderer.material.SetFloat("_RotateY", angle - Mathf.PI);
	}

}
