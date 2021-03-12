using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderController : MonoBehaviour
{
	public GameObject touchProcessor;
	private const float leftMost = 18.75f;
	private const float rightMost = -18.75f;
	// Start is called before the first frame update
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		float angle =  touchProcessor.GetComponent<TouchProcessor>().angle;
		float pos = leftMost - (leftMost - rightMost) * (angle - Mathf.PI / 2) / (Mathf.PI / 2);
		transform.localPosition = new Vector3(pos, transform.localPosition.y, 0);
	}
}
