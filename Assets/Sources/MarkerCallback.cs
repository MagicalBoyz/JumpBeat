using System;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

public class MarkerCallback : MonoBehaviour
{
	public struct EventMarkerProperties
	{
		public string name;
		public int markerPosition;
		public int bar;
		public int beat;
		public int beatPosition;
		public float tempo;
		public int timeSignatureUpper;
		public int timeSignatureLower;
	}

	[SerializeField]
	private bool changeCameraColor = true;

	[FMODUnity.EventRef]
	[SerializeField]
	private string eventName;

	private FMOD.Studio.EventInstance eventInstance;

	private FMOD.Studio.EVENT_CALLBACK beatCallback;

	private int beatCount;
	private string currentMarker;
	private int currentPosition;

	FMOD.Studio.TIMELINE_MARKER_PROPERTIES currentMarkerProperties;
	FMOD.Studio.TIMELINE_BEAT_PROPERTIES beatProperties;

	private new Camera camera;

	private void Start()
	{
		camera = GetComponent<Camera>();

		beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

		eventInstance = FMODUnity.RuntimeManager.CreateInstance(eventName);

		eventInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
		eventInstance.start();
	}

	private void OnDestroy()
	{
		eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		eventInstance.release();
	}

	private FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr)
	{
		if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT)
		{
			beatProperties = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));

			if (changeCameraColor)
				camera.backgroundColor = UnityEngine.Random.ColorHSV();
			
			beatCount++;
		}
		else if (type == FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
		{
			currentMarkerProperties = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
			currentMarker = currentMarkerProperties.name;
		}

		return FMOD.RESULT.OK;
	}

	private void OnGUI()
	{
		GUIStyle style = new GUIStyle(GUI.skin.box);
		style.fontSize = 16;
		style.normal.textColor = Color.white;

		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Beat Count {beatCount} \n");
		stringBuilder.AppendLine($"Bar: {beatProperties.bar}");
		stringBuilder.AppendLine($"Beat: {beatProperties.beat}");
		stringBuilder.AppendLine($"Positiont: {beatProperties.position}");
		stringBuilder.AppendLine($"Tempo: {beatProperties.tempo}");
		stringBuilder.AppendLine($"Time Signature Upper: {beatProperties.timesignatureupper}");
		stringBuilder.AppendLine($"Time Signature Lower: {beatProperties.timesignaturelower}");

		GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, (Screen.height / 2), 500, 800));
	
		GUILayout.Box(stringBuilder.ToString(), style);
		GUILayout.Box($"Current Marker {(string)currentMarkerProperties.name} at position {currentMarkerProperties.position}", style);

		GUILayout.EndArea();
	}
}
