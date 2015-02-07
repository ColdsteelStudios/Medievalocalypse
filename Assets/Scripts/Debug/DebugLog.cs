// ---------------------------------------------------------------------------
// DebugLog.cs
// 
// Class description goes here
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugLog : MonoBehaviour
{
	private string m_messageOne;
	private string m_messageTwo;
	private string m_messageThree;
	private string m_messageFour;
	private string m_messageFive;
	private Text m_debugTextbox;

	void Start()
	{
		m_debugTextbox = gameObject.GetComponent<Text>();
		m_messageOne = "";
		m_messageTwo = "";
		m_messageThree = "";
		m_messageFour = "";
		m_messageFive = "";
	}

	public void PrintMessage(string a_debugMessage)
	{
		//Move all the messages down one line and add the new message to the start
		m_messageFive = m_messageFour;
		m_messageFour = m_messageThree;
		m_messageThree = m_messageTwo;
		m_messageTwo = m_messageOne;
		m_messageOne = a_debugMessage;
		//Update the textbox to display the messages
		m_debugTextbox.text = m_messageOne + "\n" + m_messageTwo + "\n" + m_messageThree + "\n" + m_messageFour + "\n" + m_messageFive;
	}

	public void ClearLog()
	{
		m_messageOne = "";
		m_messageTwo = "";
		m_messageThree = "";
		m_messageFour = "";
		m_messageFive = "";
		m_debugTextbox.text = m_messageOne + "\n" + m_messageTwo + "\n" + m_messageThree + "\n" + m_messageFour + "\n" + m_messageFive;
	}
}