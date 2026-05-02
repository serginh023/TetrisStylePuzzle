using System.Collections;
using UnityEngine;
using Utility;

namespace Core
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Shape[] allShapes;
        [SerializeField] private Transform[] queueXforms;
        [SerializeField] private ParticlePlayer spawnFx;
        [SerializeField] private float queueScale = .1f;
        private Shape[] queuedShapes = new Shape[3];

        private void Awake()
        {
            InitQueue();
        }

        private Shape GetRandomShape()
        {
            var i = Random.Range(0, allShapes.Length);
            if (allShapes[i])
                return allShapes[i];
        
            Debug.LogWarning("WARNING! Invalid shape in spawner.");

            return null;
        }

        public Shape SpawnShape()
        {
            var shape = GetQueuedShape(); //shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
            shape.transform.position = transform.position;
            StartCoroutine(GrowShape(shape, .25f));

            if (spawnFx)
                spawnFx.Play();
            if (shape)
                return shape;
            
            Debug.LogWarning("WARNING! Invalid shape in spawner.");

            return null;
        }

        private void InitQueue()
        {
            for(var i = 0; i < queuedShapes.Length; i++)
            {
                queuedShapes[i] = null;
            }
            FillQueue();
        }

        private void FillQueue()
        {
            for(var i = 0; i < queuedShapes.Length; i++)
                if (!queuedShapes[i])
                {
                    queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity);
                    queuedShapes[i].transform.position = queueXforms[i].transform.position + queuedShapes[i].m_QueueOffSet;
                    queuedShapes[i].transform.localScale = new Vector3(queueScale, queueScale, queueScale);
                }
        }

        private Shape GetQueuedShape()
        {
            Shape firstShape = null;

            if (queuedShapes[0])
                firstShape = queuedShapes[0];

            for(var i = 1; i < queuedShapes.Length; i++)
            {
                queuedShapes[i - 1] = queuedShapes[i];
                queuedShapes[i - 1].transform.position = queueXforms[i - 1].transform.position + queuedShapes[i].m_QueueOffSet;
            }

            queuedShapes[queuedShapes.Length - 1] = null;

            FillQueue();

            return firstShape;
        }

        private IEnumerator GrowShape(Shape shape, float growTime = .5f)
        {
            var size = 0f;
            growTime = Mathf.Clamp(growTime, 0.1f, 1.5f);
            var sizeDelta = Time.deltaTime / growTime;

            while (size < 1f)
            {
                var shapeTransform = shape.transform;
                shapeTransform.localScale = new Vector3(size, size, size);
                size += sizeDelta;
                shapeTransform.position = transform.position;
                yield return null;
            }
            shape.transform.localScale = Vector3.one;
        }
    }
}
