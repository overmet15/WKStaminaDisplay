using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WKStaminaSlider.MonoBehaviours
{
    public class StaminaSlider : MonoBehaviour
    {
        CL_Player.Hand hand;

        RectTransform rectTransform;
        Slider slider;
        CanvasGroup canvasGroup;
        float initalPosition;

        bool hidden;


        void Start()
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            hidden = true;
            canvasGroup.alpha = 0;
            rectTransform = GetComponent<RectTransform>();
            initalPosition = rectTransform.anchoredPosition.x;

            if (!WKStaminaSliderPlugin.staminaSliderTransitionEnabled.Value) Toggle();
        }

        public void Update()
        {
            if (hand == null || slider == null) return;

            // if they change it in future, idk how to get it manualy
            if (slider.maxValue < hand.gripStrength) slider.maxValue = hand.gripStrength;

            slider.value = hand.gripStrength;

            if (!WKStaminaSliderPlugin.staminaSliderTransitionEnabled.Value) return;

            if (hidden && hand.gripStrength < slider.maxValue) Toggle();
            else if (!hidden && hand.gripStrength >= slider.maxValue) Toggle();

        }
        
        public void Setup(CL_Player.Hand hand, Slider slider)
        {
            this.hand = hand;
            this.slider = slider;

            slider.maxValue = hand.gripStrength;
        }

        void Toggle()
        {
            hidden = !hidden;

            if (hidden)
            {
                canvasGroup.DOFade(0, 0.25f).SetEase(Ease.OutQuart);

                rectTransform.DOAnchorPosX(initalPosition + (initalPosition < 0 ? 75 : -75),
                    WKStaminaSliderPlugin.staminaTransitionSpeed.Value).SetEase(Ease.OutQuart);
            }
            else
            {
                canvasGroup.DOFade(1, 0.25f).SetEase(Ease.OutQuart);

                rectTransform.DOAnchorPosX(initalPosition,
                    WKStaminaSliderPlugin.staminaTransitionSpeed.Value).SetEase(Ease.OutQuart);
            }
        }
    }
}
