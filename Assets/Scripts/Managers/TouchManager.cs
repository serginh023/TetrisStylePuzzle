﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    public delegate void TouchEventHandler(Vector2 swipe);

    public static event TouchEventHandler DragEvent;
    public static event TouchEventHandler SwipeEvent;
    public static event TouchEventHandler TapEvent;

    Vector2 m_touchMovement;

    [Range(50, 150)][SerializeField]
    public int m_minDragDistance = 100;

    [Range(20, 150)][SerializeField]
    public int m_minSwipeDistance = 50;

    [Range(.01f, .5f)][SerializeField]
    int m_timeTapWindow = 200;
    float m_timeTapMax = 0.1f;

    void OnDrag()
    {
        if(DragEvent != null)
            DragEvent(m_touchMovement);
    }

    void OnSwipe()
    {
        if (SwipeEvent != null)
            SwipeEvent(m_touchMovement);
    }

    void OnTap()
    {
        if (TapEvent != null)
            TapEvent(m_touchMovement);
    }

    public Text mDiagnosticText1;
    public Text mDiagnosticText2;

    public bool m_useDiagnostic = false;

    void Diagnostic(string text1, string text2)
    {
        if (mDiagnosticText1 && mDiagnosticText2)
        {
            mDiagnosticText1.gameObject.SetActive(m_useDiagnostic);
            mDiagnosticText2.gameObject.SetActive(m_useDiagnostic);

            mDiagnosticText1.text = text1;
            mDiagnosticText2.text = text2;
        }
    }

    string SwipeDiagnostic(Vector2 swipeMovement)
    {
        string direction = "";

        if(Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
            direction = (swipeMovement.x >= 0) ? "right" : "left";
        else
            direction = (swipeMovement.y >= 0) ? "up" : "down";

        return direction;
    }

    private void Start()
    {
        Diagnostic("", "");
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if(touch.phase == TouchPhase.Began)
            {
                m_touchMovement = Vector2.zero;
                m_timeTapMax = Time.time + m_timeTapWindow;
                Diagnostic("", "");
            }

            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                m_touchMovement += touch.deltaPosition;

                if (m_touchMovement.magnitude > m_minDragDistance)
                {
                    OnDrag();
                    Diagnostic("Swipe detected", touch.position.x + ", " + touch.position.y + " " + SwipeDiagnostic(touch.position));
                }

            }
            else if(touch.phase == TouchPhase.Ended)
            {
                if(m_touchMovement.magnitude > m_minSwipeDistance)
                {
                    OnSwipe();
                    Diagnostic("Drag detected", touch.position.x + ", " + touch.position.y + " " + SwipeDiagnostic(touch.position));
                }
                else if(Time.time < m_timeTapMax)
                {
                    OnTap();

                    Diagnostic("Tap detected ", touch.position.x + ", " + touch.position.y + " " + SwipeDiagnostic(touch.position));
                }
                    
            }
                

        }
    }

}
