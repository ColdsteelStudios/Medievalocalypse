// ---------------------------------------------------------------------------
// BlockButtonSetup.cs
// 
// Called by player controller when they are added to the scene.
// The event triggers for this button need to be set up at run time
// because the player isnt added to the scene until the dungeon has
// been already been generated.
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BlockButtonSetup : MonoBehaviour
{
    //Create an event delegate that will be used for creating methods that respond to events
    public delegate void EventDelegate(UnityEngine.EventSystems.BaseEventData baseEvent);
    //Player character we send messages to when the run button is interacted with
    private GameObject m_playerCharacter;

    public void PointerDownEvent(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        if (m_playerCharacter != null)
            m_playerCharacter.SendMessage("MobileShieldOn");
    }

    public void PointerUpEvent(UnityEngine.EventSystems.BaseEventData baseEvent)
    {
        if (m_playerCharacter != null)
            m_playerCharacter.SendMessage("MobileShieldOff");
    }

    public void SetUp(GameObject a_player)
    {
        //Store reference to the player character
        m_playerCharacter = a_player;

        //Get the event trigger attached to this object
        EventTrigger l_eventTrigger = GetComponent<EventTrigger>();
        //Create two new entry's to the event trigger
        EventTrigger.Entry l_pointerDownEntry = new EventTrigger.Entry();
        EventTrigger.Entry l_pointerUpEntry = new EventTrigger.Entry();
        //This event will respond to pointer down and pointer up
        l_pointerDownEntry.eventID = EventTriggerType.PointerDown;
        l_pointerUpEntry.eventID = EventTriggerType.PointerUp;
        //Create a new trigger to hold our callback method
        l_pointerDownEntry.callback = new EventTrigger.TriggerEvent();
        l_pointerUpEntry.callback = new EventTrigger.TriggerEvent();
        //Create a new UnityAction, it contains our PointerUpMethod and PointerDownMethod
        //delegate to respond to events
        UnityEngine.Events.UnityAction<BaseEventData> l_pointerDownCallback = new UnityEngine.Events.UnityAction<BaseEventData>(PointerDownEvent);
        UnityEngine.Events.UnityAction<BaseEventData> l_pointerUpCallback = new UnityEngine.Events.UnityAction<BaseEventData>(PointerUpEvent);
        //Add our callback to the listeners
        l_pointerDownEntry.callback.AddListener(l_pointerDownCallback);
        l_pointerUpEntry.callback.AddListener(l_pointerUpCallback);
        //Add the EventTrigger event to the event trigger component
        l_eventTrigger.delegates.Add(l_pointerDownEntry);
        l_eventTrigger.delegates.Add(l_pointerUpEntry);
    }
}