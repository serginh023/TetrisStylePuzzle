using UnityEngine;

namespace Core
{
    public class Holder : MonoBehaviour
    {
        [SerializeField] private Transform holderXform;
        [SerializeField] private Shape heldShape;
        
        public bool CanRelease;
        public Shape HeldShape => heldShape;
        
        private readonly float scale = .5f;

        public void Catch(Shape shape)
        {
            if (heldShape != default)
            {
                Debug.LogWarning("HOLDER Release a shape before trying to hold");
                return;
            }

            if (!shape)
            {
                Debug.LogWarning("HOLDER Invalid Shape");
                return;
            }

            if (holderXform)
            {
                var shapeTransform = shape.transform;
                shapeTransform.position = holderXform.transform.position + shape.m_QueueOffSet;
                shapeTransform.localScale = new Vector3(scale, scale, scale);
                heldShape = shape;
                CanRelease = true;
                shapeTransform.rotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("HOLDER Invalid holderXform");
            }

        }

        public Shape Release()
        {
            heldShape.transform.localScale = Vector3.one;
            CanRelease = false;
            var newShape = heldShape;
            heldShape = null;
            return newShape;
        }
    }
}
