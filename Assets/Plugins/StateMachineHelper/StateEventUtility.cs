/// <summary>
/// Create By Oscar Mok
/// </summary>

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[AddComponentMenu("State Event Utility")]
[System.Serializable]
#if UNITY_4_6
public class StateEventUtility : MonoBehaviour, IEventSystemHandler {
#elif UNITY_5
public class StateEventUtility :  StateMachineBehaviour, IEventSystemHandler {
#endif

	#if UNITY_4_6
	[SerializeField]
	Animator PerviousAnimator;

	[SerializeField]
	Animator TargetStateMachine;
	#endif

	[SerializeField]
	#if UNITY_5
	public CustomStateInfo CSState;
	#elif UNITY_4_6
	public CustomStateInfo[] CSState;
	#endif

	#if UNITY_5
	StateMachineBehaviour stateBehaviour;
	#endif


	[SerializeField]
	int CurrentStateNameHash;

	int transitionHash;

	void Start()
	{
		#if UNITY_5
		stateBehaviour = this as StateMachineBehaviour;
		#endif

	}

	bool isEnter;
	void Update()
	{
#if UNITY_4_6
		for (int i = 0; i < CSState.Length; i++)
		{
			if (CSState [i].nameHash == TargetStateMachine.GetCurrentAnimatorStateInfo (0).nameHash)
			{
				if (CSState[i].EnterEventExcuted == 0)
				{
					if (!isEnter)
					{
						//Debug.Log(CSState[i].Name+" is Enter, current state hash is "+ CSState [i].nameHash);
						isEnter = true;
						CSState[i].SetEnterToken();
						CSState[i].EnterEvent.Invoke(new Object());

					}
				}

				if (CSState[i].ExitEventExcute == 0)
				{
					foreach (int TranHash in CSState[i].TransitionsHash)
					{
						if (isEnter)
						{
							if (TargetStateMachine.GetAnimatorTransitionInfo (0).nameHash == TranHash)
							{
								//CSState [i].EnterEvent.Invoke (CSState [i].args);
								//Debug.Log(CSState[i].Name+" is exiting,"+CSState.Length+" current tran hash is "+ TranHash);
								isEnter = false;
								CSState[i].SetExitToken();
								CSState[i].ExitEvent.Invoke(new Object());
							}
						}
					}
				}
			} else {
				CSState[i].ResetExitToken();
				CSState[i].ResetEnterToken();
			}

			if (isEnter)
			{
				CSState[i].UpdateEvent.Invoke(new Object());
			}
		}
#endif
	}

#if UNITY_5
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		CSState.EnterEvent.Invoke (new Object ());
	}
	
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		CSState.ExitEvent.Invoke (new Object ());
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		CSState.UpdateEvent.Invoke (new Object ());
	}
#endif
}
