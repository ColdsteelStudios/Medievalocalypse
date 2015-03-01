// ---------------------------------------------------------------------------
// SetHeartDisplay.cs
// 
// Quick way to change the texture on a heart
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetHeartDisplay : MonoBehaviour
{
    public Sprite m_fullHeartSprite;
    public Sprite m_halfHeartSprite;
    public Sprite m_emptyHeartSprite;
    private Image m_heartImageDisplay;

    void Start()
    {
        m_heartImageDisplay = GetComponent<Image>();
    }

    public void SetFullHeart()
    {
        m_heartImageDisplay.sprite = m_fullHeartSprite;
    }

    public void SetHalfHeart()
    {
        m_heartImageDisplay.sprite = m_halfHeartSprite;
    }

    public void SetEmptyHeart()
    {
        m_heartImageDisplay.sprite = m_emptyHeartSprite;
    }
}