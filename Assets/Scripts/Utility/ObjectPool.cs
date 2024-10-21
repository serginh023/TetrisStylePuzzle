using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject gameObject;
        [SerializeField] private int total = 20;

        private List<GameObject> _list;
        private int _enlargeQuant = 5;

        private void Start()
        {
            InitiateList();
        }

        public GameObject Withdraw()
        {
            if (_list == null)
            {
                return null;
            }

            if (_list.Count == 0)
            {
                EnlargeList(_enlargeQuant);
            }

            var obj = _list[0];
            _list.RemoveAt(0);
            return obj;
        }

        public void Retrieve(GameObject item)
        {
            _list.Add(item);
        }

        public List<GameObject> Withdraw(int quant)
        {
            var smallList = new List<GameObject>();
            for (int i = 0; i < quant; i++)
            {
                smallList.Add(Withdraw());
            }

            return smallList;
        }

        private void InitiateList()
        {
            _list = new List<GameObject>();
            EnlargeList(_enlargeQuant);
        }

        private void EnlargeList(int quant)
        {
            //TODO Enlarge list with more items
            for (var i = 0; i < quant; i++)
            {
                var go = Instantiate(gameObject, Vector3.zero, Quaternion.identity);
                _list.Add(go);
            }
        }
    }
}
