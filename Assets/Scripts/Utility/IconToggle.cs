using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour
{
    public Sprite m_iconTrue;
    public Sprite m_iconFalse;

    public bool m_myDefaultIconState = true;

    Image m_image;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        m_image.sprite = (m_myDefaultIconState) ? m_iconTrue : m_iconFalse;
    }

    public void ToogleIcon(bool state)
    {
        if (!m_image || !m_iconFalse || !m_iconTrue)
        {
            Debug.LogWarning("WARNING!  ICONTOOGLE missing iconTrue or iconFalse");
            return;
        }
            
        m_image.sprite = (state) ? m_iconTrue : m_iconFalse;
    }
}
