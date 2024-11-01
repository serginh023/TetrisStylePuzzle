using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private TouchManager TouchManager;

        public Toggle ToggleDiagnostic;
        public Slider DragDistanceSlider;
        public Slider SwipeDistanceSlider;

        private void Start()
        {
            if(DragDistanceSlider != null)
            {
                DragDistanceSlider.value = TouchManager.m_minDragDistance;
                DragDistanceSlider.minValue = 50;
                DragDistanceSlider.maxValue = 150;
            }

            if (SwipeDistanceSlider != null)
            {
                SwipeDistanceSlider.value = TouchManager.m_minSwipeDistance;
                SwipeDistanceSlider.minValue = 20;
                SwipeDistanceSlider.maxValue = 150;
            }

            if(ToggleDiagnostic != null)
            {
                TouchManager.m_useDiagnostic = ToggleDiagnostic.isOn;
            }
        }

        public void UpdatePanel()
        {
            if (DragDistanceSlider != null && TouchManager != null)
            {
                TouchManager.m_minDragDistance = (int) DragDistanceSlider.value;
            }

            if(SwipeDistanceSlider != null && TouchManager != null)
            {
                TouchManager.m_minSwipeDistance = (int) SwipeDistanceSlider.value;
            }

            if (ToggleDiagnostic != null)
            {
                TouchManager.m_useDiagnostic = ToggleDiagnostic.isOn;
            }
        }

    }
}
