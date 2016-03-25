/// <summary>
/// Create By Oscar Mok
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(StateEventUtility))]
public class StateEventUtilityInspector : Editor {

	private SerializedObject StateEnvUtility;
	private SerializedProperty

	#if UNITY_4_6
		TargetStateMachine,
		PervoisAnimator,
		currentHash,
		CurrentStateInfo,
	#endif

	#if UNITY_5
		
	#endif
		CustomStateInfo;
		

	#if UNITY_4_6
	Animator TargetAnimator;
	CustomStateInfo _csState;

	GUIContent InsertItem = new GUIContent("+", "Add New Item For State Event");
	GUIContent DeleteItem = new GUIContent ("-", "Add New Item For State Event");

	AnimatorController anim;
	Dictionary<string, State> States = new Dictionary<string, State>();
	List<string> StatesName = new List<string>();
	#endif


	void OnEnable ()
	{
		StateEnvUtility = new SerializedObject (target);

		CustomStateInfo = StateEnvUtility.FindProperty ("CSState");

		#if UNITY_4_6
		TargetStateMachine = StateEnvUtility.FindProperty ("TargetStateMachine");
		#endif


		#if UNITY_4_6
		PervoisAnimator = StateEnvUtility.FindProperty ("PerviousAnimator");
		#endif

		//_csState = CSState.objectReferenceValue as CustomStateClass;

		#if UNITY_4_6
		if (TargetAnimator != null && States.Count < 1) 
		{
			GetAllAnimatorStates ();
		}
		#endif

		#if UNITY_5


		#endif
	}

	int TransitionHash;
	void GetAllAnimatorStates()
	{
		#if UNITY_4_6
		anim = TargetAnimator.runtimeAnimatorController as AnimatorController;

		if (anim != null) 
		{
			//Fetch All states
			for (int j = 0; j < anim.layerCount; j++) 
			{
				for (int k = 0; k < anim.GetLayer (j).stateMachine.stateMachineCount; k++)
				{
					for (int i = 0; i < anim.GetLayer (j).stateMachine.GetStateMachine(k).stateCount; i++) 
					{
						States.Add (anim.GetLayer (j).stateMachine.GetStateMachine(k).GetState (i).name, anim.GetLayer (j).stateMachine.GetStateMachine(k).GetState (i));
						StatesName.Add (anim.GetLayer (j).stateMachine.GetStateMachine(k).GetState (i).name);
					}
				}

				for (int i = 0; i < anim.GetLayer (j).stateMachine.stateCount; i++) 
				{
					States.Add (anim.GetLayer (j).stateMachine.GetState (i).name, anim.GetLayer (j).stateMachine.GetState (i));
					StatesName.Add (anim.GetLayer (j).stateMachine.GetState (i).name);
				}
			}
		}
		#endif
	}

	// Update is called once per frame
	public override void OnInspectorGUI  ()
	{
		StateEnvUtility.Update (); ///Make update

		#if UNITY_4_6
		EditorGUILayout.PropertyField(TargetStateMachine);
		TargetAnimator = TargetStateMachine.objectReferenceValue as Animator;
		//currentHash.intValue = TargetAnimator.GetCurrentAnimatorStateInfo (0).nameHash;

		//While the Animator Chnage, reset everything
		if (PervoisAnimator.objectReferenceValue != TargetStateMachine.objectReferenceValue) 
		{
			CustomStateInfo = StateEnvUtility.FindProperty ("CSState");
			if (States != null)
				States.Clear ();

			if (StatesName != null)
				StatesName.Clear ();
			
			CustomStateInfo.GetArrayElementAtIndex (0).FindPropertyRelative ("index").intValue = 0;

			if (CustomStateInfo.arraySize > 0)
			{
				//Remove from the end
				for (int j = CustomStateInfo.arraySize - 1; j > 0; j--)
				{
					CustomStateInfo.DeleteArrayElementAtIndex (j);
				}
			}
		}

		if (TargetAnimator != null && States.Count < 1) 
		{
			GetAllAnimatorStates ();
		}

		PervoisAnimator.objectReferenceValue = TargetStateMachine.objectReferenceValue; //mark down the change

		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();
		EditorGUILayout.Separator ();

		if (TargetStateMachine != null) {
			
			if (CustomStateInfo.arraySize < 1)
			{
				CustomStateInfo.arraySize = 1;
			}

			GUILayoutOption ButtonWidth = GUILayout.MaxWidth (100f);
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (new GUIContent ("Add/Remove Items"));
			EditorGUILayout.Space ();
			if (GUILayout.Button (InsertItem, EditorStyles.miniButtonLeft, ButtonWidth)) {
				CustomStateInfo.InsertArrayElementAtIndex (CustomStateInfo.arraySize - 1);
			}

			if (GUILayout.Button (DeleteItem, EditorStyles.miniButtonRight, ButtonWidth)) {
				if (CustomStateInfo.arraySize > 1)
				{
					CustomStateInfo.DeleteArrayElementAtIndex (CustomStateInfo.arraySize - 1);
				}
			}

			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();

			//EditorGUILayout.PropertyField (CSState);

			StateEventUtility btnEvnTrigger = Selection.activeGameObject.GetComponent<StateEventUtility> ();

			for (int i = 0; i < CustomStateInfo.arraySize && States.Count > 0 && StatesName.Count > 0; i++) {
				SerializedProperty list = CustomStateInfo.GetArrayElementAtIndex (i);
				SerializedProperty index =  list.FindPropertyRelative ("index");

				//Retrive the transition

				StateMachine sm = States [StatesName [index.intValue]].stateMachine;
				SerializedProperty tranHash = list.FindPropertyRelative ("TransitionsHash");
				Transition[] tran = sm.GetTransitionsFromState (States [StatesName [index.intValue]]);
				tranHash.arraySize = tran.Length;

				list.FindPropertyRelative ("nameHash").intValue = States [StatesName [index.intValue]].uniqueNameHash;
				list.FindPropertyRelative ("Name").stringValue = States [StatesName [index.intValue]].uniqueName;
				for (int t = 0; t < tran.Length; t++)
				{
					tranHash.GetArrayElementAtIndex (t).intValue = tran[t].uniqueNameHash;
				}



				list.FindPropertyRelative("fold").boolValue = EditorGUILayout.Foldout (list.FindPropertyRelative("fold").boolValue, new GUIContent (list.FindPropertyRelative ("Name").stringValue.Replace ("Base Layer.", "") + " state Event"));

				if (list.FindPropertyRelative("fold").boolValue)
				{
					EditorGUILayout.HelpBox (StatesName [index.intValue].Replace ("Base Layer.", "") + " Field, index is "+index.intValue, MessageType.Info);
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.LabelField (new GUIContent ("State To Trigger Event"), GUILayout.MaxWidth (200f));
					index.intValue = EditorGUILayout.Popup (index.intValue, StatesName.ToArray ());
					EditorGUILayout.EndHorizontal ();

					EditorGUILayout.Separator ();
					EditorGUILayout.LabelField (new GUIContent ("State Enter Event"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("EnterEvent"));
					EditorGUILayout.LabelField (new GUIContent ("State Update Event"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("UpdateEvent"));
					EditorGUILayout.LabelField (new GUIContent ("State Exit Event"));
					EditorGUILayout.PropertyField (list.FindPropertyRelative ("ExitEvent"));


					EditorGUILayout.Separator ();

					EditorGUILayout.LabelField (new GUIContent ("Selected State name is: " + list.FindPropertyRelative ("Name").stringValue));
					//EditorGUILayout.LabelField (new GUIContent ("Selected State hash is:"+list.FindPropertyRelative ("nameHash").intValue));

					EditorGUILayout.Separator ();

				}

				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (new GUIContent ("Remove This Field"));
				EditorGUILayout.Space ();
				if (GUILayout.Button (DeleteItem, EditorStyles.miniButton, ButtonWidth))
				{
					CustomStateInfo.DeleteArrayElementAtIndex (i);
				}
				EditorGUILayout.EndHorizontal ();



				//EditorGUILayout.EndHorizontal ();
				EditorGUILayout.Separator ();
				EditorGUILayout.Separator ();
			}

		}

		#elif UNITY_5

		EditorGUILayout.Separator ();
		EditorGUILayout.LabelField (new GUIContent ("State Enter Event"));
		EditorGUILayout.PropertyField (CustomStateInfo.FindPropertyRelative("EnterEvent"));
		EditorGUILayout.LabelField (new GUIContent ("State Update Event"));
		EditorGUILayout.PropertyField (CustomStateInfo.FindPropertyRelative("UpdateEvent"));
		EditorGUILayout.LabelField (new GUIContent ("State Exit Event"));
		EditorGUILayout.PropertyField (CustomStateInfo.FindPropertyRelative("ExitEvent"));

		EditorGUILayout.Separator ();


		#endif

		StateEnvUtility.ApplyModifiedProperties (); ///Apply the changes
	}
}
