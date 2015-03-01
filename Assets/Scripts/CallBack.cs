// ---------------------------------------------------------------------------
// CallBack.cs
// 
// Pass an object, function name and an amount of time
// When that amount of time has passed, the named function will be called
// on the object that was passed in
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CallBack : MonoBehaviour 
{
	//name of the functions to be called
	private List<string> m_functionNames;
	private List<float> m_functionTimes;
	private List<GameObject> m_functionTargets;
	private bool m_setup = false;

	void Update()
	{
		//Count down the timers
		if((m_functionTimes != null) && (m_functionTimes.Count > 0))
		{
            for ( int i = 0; i < m_functionTimes.ToArray().Length; i++ )
            {
                //Update time remaining
                float l_timeRemaining = m_functionTimes[i] - Time.deltaTime;
                m_functionTimes[i] = l_timeRemaining;

                //If time has run out, it's time to call the target
                //function and remove it from the list
                if (l_timeRemaining <= 0.0f)
                {
                    //Call the function
                    m_functionTargets[i].SendMessage(m_functionNames[i]);
                    //Remove the objects from the lists, they no longer need to be tracked
                    m_functionNames.RemoveAt(i);
                    m_functionTimes.RemoveAt(i);
                    m_functionTargets.RemoveAt(i);
                }
            }
		}
	}

	public void CreateCallback( GameObject a_targetObject, string a_functionName, float a_functionTime )
	{
		if(!m_setup)
		{
			m_functionNames = new List<string> ();
			m_functionTimes = new List<float> ();
			m_functionTargets = new List<GameObject> ();
			m_setup = true;
		}

		m_functionNames.Add (a_functionName);
		m_functionTimes.Add (a_functionTime);
		m_functionTargets.Add (a_targetObject);
	}
}