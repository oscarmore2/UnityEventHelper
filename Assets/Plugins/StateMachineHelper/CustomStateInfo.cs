/// <summary>
/// Create By Oscar Mok
/// </summary>

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[Serializable]
public class CustomStateInfo 
{
	[Serializable]
	public class CustomStateEvent : UnityEvent <UnityEngine.Object>
	{

	}

	public CustomStateEvent EnterEvent;
	public CustomStateEvent UpdateEvent;
	public CustomStateEvent ExitEvent;

	#if UNITY_4_6



	[SerializeField]
	public int index;

	public int nameHash;

	public string Name;

	private int _enterEventExcuted = 0;
	public int EnterEventExcuted
	{
		get{return _enterEventExcuted;}
	}



	private int _exitEventExcuted = 0;
	public int ExitEventExcute
	{
		get{return _exitEventExcuted;}
	}


	public string types;
	public UnityEngine.Object args;


	public int[] TransitionsHash;

	[SerializeField]
	private bool fold;

	public void SetEnterToken()
	{
		_enterEventExcuted = 1;
	}

	public void ResetEnterToken()
	{
		_enterEventExcuted = 0;
	}

	public void SetExitToken()
	{
		_exitEventExcuted = 1;
	}

	public void ResetExitToken()
	{
		_exitEventExcuted = 0;
	}
	#endif
}


