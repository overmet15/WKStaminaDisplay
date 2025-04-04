using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WKStaminaDisplay.MonoBehaviours
{
    public class StaminaSlider : MonoBehaviour
    {
        CL_Player.Hand hand;

        RectTransform rectTransform;
        Slider slider;
        CanvasGroup canvasGroup;
        float initalPosition;

        bool hidden;

        bool isLeft;

        void Start()
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            hidden = true;
            canvasGroup.alpha = 0;
            rectTransform = GetComponent<RectTransform>();

            initalPosition = rectTransform.anchoredPosition.x;

            Plugin.onConfigChange.AddListener(OnConfigChange);

            if (!Plugin.staminaSliderTransitionEnabled.Value) ForceInPlace();
            else rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Update()
        {
            if (hand == null || slider == null) return;

            // if they change it in future, idk how to get it manualy
            if (slider.maxValue < hand.gripStrength) slider.maxValue = hand.gripStrength;

            slider.value = hand.gripStrength;

            if (!Plugin.staminaSliderTransitionEnabled.Value)  return;
            
            if (hidden && hand.gripStrength < slider.maxValue) Toggle();
            else if (!hidden && hand.gripStrength >= slider.maxValue) Toggle();
        }
        
        public void Setup(CL_Player.Hand hand, Slider slider, bool isLeft)
        {
            this.hand = hand;
            this.slider = slider;
            this.isLeft = isLeft;

            slider.maxValue = hand.gripStrength;
        }

        void Toggle()
        {
            hidden = !hidden;

            int xPos = Plugin.staminaDistanceFromCenter.Value;
            if (isLeft) xPos *= -1;

            if (hidden)
            {
                canvasGroup.DOFade(0, 0.25f).SetEase(Ease.OutQuart);

                rectTransform.DOAnchorPosX(xPos + (xPos < 0 ? 75 : -75),
                    Plugin.staminaTransitionSpeed.Value).SetEase(Ease.OutQuart);
            }
            else
            {
                canvasGroup.DOFade(1, 0.25f).SetEase(Ease.OutQuart);

                rectTransform.DOAnchorPosX(xPos, Plugin.staminaTransitionSpeed.Value)
                    .SetEase(Ease.OutQuart);
            }
        }

        void OnConfigChange()
        {
            // so if its changed it will pull it back
            if (!Plugin.staminaSliderTransitionEnabled.Value) ForceInPlace();
            else if (!hidden)
            {
                hidden = true;
                Toggle();
            }

            GetComponent<Image>().color = Plugin.staminaBackgroundColor.Value;
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Plugin.staminaFillColor.Value;
        }

        void ForceInPlace()
        {
            int xPos = Plugin.staminaDistanceFromCenter.Value;
            if (isLeft) xPos *= -1;

            canvasGroup.alpha = 1;
            rectTransform.anchoredPosition = new Vector2(xPos, 0);
        }

        void OnDestroy()
        {
            Plugin.onConfigChange.RemoveListener(OnConfigChange);
        }
    }
}
