using System;
using UnityEngine;

namespace Core
{
    public class Ghost : MonoBehaviour
    {
        private Shape ghostShape;
        private Boolean hitBottom = false;
        public Color Color = new Color(1f, 1f, 1f, 0.25f);

        public void DrawGhost(Shape originalShape, Board gameBoard)
        {
            if (!ghostShape)
            {
                ghostShape = Instantiate(originalShape, originalShape.transform.position, originalShape.transform.rotation) as Shape;
                ghostShape.gameObject.name = "GhostShape";
                var allRenders = ghostShape.GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in allRenders)
                    r.color = Color;
            }
            else
            {
                ghostShape.transform.rotation = originalShape.transform.rotation;
                ghostShape.transform.position = originalShape.transform.position;
                ghostShape.transform.localScale = Vector3.one;
            }

            hitBottom = false;

            while (!hitBottom)
            {
                ghostShape.MoveDown();
                if (!gameBoard.IsValidPosition(ghostShape))
                {
                    ghostShape.MoveUp();
                    hitBottom = true;
                }
                
            }
        }

        public void Reset()
        {
            Destroy(ghostShape.gameObject);
        }
    }
}
