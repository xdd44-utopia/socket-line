using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
	float timer = 0;
	float speed = 1f;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime * speed;
		transform.position = new Vector3(0, 0, 1.5f * (Mathf.Sin(timer) / 2 - 0.5f));
	}
}
