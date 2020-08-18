using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPosition : MonoBehaviour
{
	public Vector3 Position => transform.position;

	void OnDrawGizmosSelected()
	{
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = Color.green;

		Gizmos.DrawWireCube(transform.position, Vector3.one * .125f);
	}
}
