using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
	public GameObject BallPrefab;

	[Range(.1f, 100)]
	public float Force = 1;

	public Material BallMaterial;

	void Shoot(Vector3 targetPosition)
	{
		//get main camera
		Camera mainCam = Camera.main;

		//spawn a new ball at the camera's position
		GameObject ball = Instantiate(BallPrefab, transform);
		ball.transform.position = mainCam.transform.position;

		//give the ball a random color
		Renderer ballRenderer = ball.GetComponent<Renderer>();
		ballRenderer.sharedMaterial = BallMaterial;
		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		float randomH = Random.Range(0f, 1f);
		float randomS = Random.Range(.8f, 1f);
		float randomV = Random.Range(.5f, 1f);
		Color randomColor = Color.HSVToRGB(randomH, randomS, randomV);
		mpb.SetColor("_BaseColor", randomColor);
		ballRenderer.SetPropertyBlock(mpb);

		//shoot it at the target
		Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();
		Vector3 direction = (targetPosition - mainCam.transform.position).normalized;
		ballRigidbody.AddForce(direction * Force, ForceMode.Impulse);
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			//convert mouse location to world position
			Vector3 mouseScreenPosition = Input.mousePosition;
			Vector3 mouseWorldPosition = Vector3.zero;
			Camera mainCam = Camera.main;

			//raycast to find where the mouse is pointing in the world
			Ray ray = mainCam.ScreenPointToRay(mouseScreenPosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit))
			{
				//target the collision location
				mouseWorldPosition = hit.point;
			}
			else
			{
				//just use a point very far away
				mouseScreenPosition.z = mainCam.nearClipPlane;
				mouseWorldPosition = mainCam.ScreenToWorldPoint(mouseScreenPosition);
			}

			//shoot a ball at the location
			Shoot(mouseWorldPosition);
		}
	}
}
