using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{

    public Transform m_holderXform;
    public Shape m_heldShape = null;
    float m_scale = .5f;
    public bool m_canRelease = false;

    public void Catch(Shape shape)
    {
        if (m_heldShape != null)
        {
            Debug.LogWarning("HOLDER Release a shape before trying to hold");
            return;
        }
        Debug.Log("teste " + m_heldShape);
        if (!shape)
        {
            Debug.LogWarning("HOLDER Invalid Shape");
        }

        if (m_holderXform)
        {
            shape.transform.position = m_holderXform.transform.position + shape.m_QueueOffSet;
            shape.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
            m_heldShape = shape;
            m_canRelease = true;
        }
        else
        {
            Debug.LogWarning("HOLDER Invalid holderXform");
        }

    }

    public Shape Release()
    {
        m_heldShape.transform.localScale = Vector3.one;

        m_canRelease = false;

        Shape newShape = m_heldShape;

        m_heldShape = null;

        return newShape;
    }
}
