using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Demo : MonoBehaviour
{
	[SerializeField]
	[Range(.1f, 5)]
	float MoveTime = 3;

	[SerializeField]
	GameObject PropBench;

	[SerializeField]
	PropPosition BenchStart;

	[SerializeField]
	PropPosition BenchEnd;

	[SerializeField]
	GameObject PropLight;

	[SerializeField]
	PropPosition LightStart;

	[SerializeField]
	PropPosition LightEnd;

	Coroutine BenchCoroutine;

	Coroutine LightCoroutine;

	Camera MainCam
	{
		get
		{
			if(Application.isPlaying) return Camera.main;
			else return SceneView.lastActiveSceneView.camera;
		}
	}

	void SetFog(bool fog)
	{
		RenderSettings.fog = fog;
	}

	void SetOrthographic(bool orthographic)
	{
		if(Application.isPlaying) MainCam.orthographic = orthographic;
		else SceneView.lastActiveSceneView.orthographic = orthographic;

		MainCam.orthographicSize = 2;
		Vector3 movement = new Vector3(0, 1, 0) * (orthographic ? 1 : -1);
		MainCam.transform.position = MainCam.transform.position + movement;
	}

	void MoveBench()
	{
		StopBench();
		BenchCoroutine = MoveProp(PropBench.transform, BenchStart.Position, BenchEnd.Position);
	}

	void StopBench()
	{
		if(BenchCoroutine != null) StopCoroutine(BenchCoroutine);
	}

	void MoveLight()
	{
		StopLight();
		LightCoroutine = MoveProp(PropLight.transform, LightStart.Position, LightEnd.Position);
	}

	void StopLight()
	{
		if(LightCoroutine != null) StopCoroutine(LightCoroutine);
	}

	Coroutine MoveProp(Transform prop, Vector3 start, Vector3 end)
	{
		//figure out where start and end are
		Vector3 moveStart = prop.position;
		float d_start = Vector3.Distance(moveStart, start);
		float d_end = Vector3.Distance(moveStart, end);
		Vector3 moveEnd = d_start < d_end ? end : start;

		//scale movement time based on starting position
		float maxDistance = Vector3.Distance(start, end);
		float moveDistance = Vector3.Distance(moveStart, moveEnd);
		float timeScale = moveDistance / maxDistance;
		float moveTime = MoveTime * timeScale;

		//move the prop
		return StartCoroutine(MoveProp(prop, moveStart, moveEnd, moveTime));
	}

	IEnumerator MoveProp(Transform prop, Vector3 start, Vector3 end, float time)
	{
		float startTime = Time.time;
		float endTime = Time.time + time;

		while(Time.time < endTime)
		{
			float t = Mathf.Clamp01((Time.time - startTime) / time);
			prop.position = Vector3.Lerp(start, end, t);
			yield return new WaitForEndOfFrame();
		}

		prop.position = end;
	}

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Demo))]
    class Editor : UnityEditor.Editor
	{
		int SpaceSize = 3;

		void Space()
		{
			for(int i = 0; i < SpaceSize; i++) EditorGUILayout.Space();
		}

		public override void OnInspectorGUI()
		{
			EditorGUILayout.TextField("VR Demo Showcase", EditorStyles.boldLabel);

			base.OnInspectorGUI();

			Space();

			EditorGUILayout.TextField("Fog", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();

			if(GUILayout.Button("Fog"))
			{
				foreach(Demo d in targets) d.SetFog(true);
			}

			if(GUILayout.Button("No Fog"))
			{
				foreach(Demo d in targets) d.SetFog(false);
			}

			GUILayout.EndHorizontal();

			Space();

			EditorGUILayout.TextField("Perspective vs. Orthographic Rendering", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();

			if(GUILayout.Button("Perspective"))
			{
				foreach(Demo d in targets) d.SetOrthographic(false);
			}

			if(GUILayout.Button("Orthographic"))
			{
				foreach(Demo d in targets) d.SetOrthographic(true);
			}

			GUILayout.EndHorizontal();

			Space();

			EditorGUILayout.TextField("Occlusion and Parallax", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();

			if(GUILayout.Button("Move Bench"))
			{
				foreach(Demo d in targets) d.MoveBench();
			}

			if(GUILayout.Button("Stop Bench"))
			{
				foreach(Demo d in targets) d.StopBench();
			}

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

			if(GUILayout.Button("Move Light"))
			{
				foreach(Demo d in targets) d.MoveLight();
			}

			if(GUILayout.Button("Stop Light"))
			{
				foreach(Demo d in targets) d.StopLight();
			}

			GUILayout.EndHorizontal();
		}
	}
}
