using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    public float m_startAlpha = 1f;
    public float m_targetAlpha = 0f;
    public float m_startDelay = 0f;
    public float m_transitionTime = .5f;

    float m_increment;
    float m_currentAlpha;
    MaskableGraphic m_graphic;
    Color m_originalColor;

    // Start is called before the first frame update
    void Start()
    {
        m_graphic = GetComponent<MaskableGraphic>();
        m_originalColor = m_graphic.color;
        m_currentAlpha = m_startAlpha;

        Color tempColor = new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b, m_currentAlpha);

        m_graphic.color = tempColor;
        m_increment = (m_targetAlpha - m_startAlpha) / m_transitionTime * Time.deltaTime;

        StartCoroutine("FadeRoutine");
    }


    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(m_startDelay);

        while ( Mathf.Abs(m_targetAlpha - m_startAlpha) > 0.01f )
        {
            yield return new WaitForEndOfFrame();

            m_currentAlpha += m_increment;

            Color tempColor = new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b, m_currentAlpha);

            m_graphic.color = tempColor;
        }
    }
}
