using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    GameController m_gameController;
    [SerializeField]
    TouchManager m_touchManager;

    public Slider m_dragDistanceSlider;
    public Slider m_swipeDistanceSlider;
    public Toggle m_toggleDiagnostic;

    // Start is called before the first frame update
    void Start()
    {
        if(m_dragDistanceSlider != null)
        {
            m_dragDistanceSlider.value = m_touchManager.m_minDragDistance;
            m_dragDistanceSlider.minValue = 50;
            m_dragDistanceSlider.maxValue = 150;

        }

        if (m_swipeDistanceSlider != null)
        {
            m_swipeDistanceSlider.value = m_touchManager.m_minSwipeDistance;
            m_swipeDistanceSlider.minValue = 20;
            m_swipeDistanceSlider.maxValue = 150;
        }

        if(m_toggleDiagnostic != null)
        {
            m_touchManager.m_useDiagnostic = m_toggleDiagnostic.isOn;
        }
    }

    public void UpdatePanel()
    {
        if (m_dragDistanceSlider != null && m_touchManager != null)
        {
            m_touchManager.m_minDragDistance = (int) m_dragDistanceSlider.value;
        }

        if(m_swipeDistanceSlider != null && m_touchManager != null)
        {
            m_touchManager.m_minSwipeDistance = (int) m_swipeDistanceSlider.value;
        }

        if (m_toggleDiagnostic != null)
        {
            m_touchManager.m_useDiagnostic = m_toggleDiagnostic.isOn;
        }
    }

}
