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

        bool isLeft;

        void Start()
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            hidden = true;
            canvasGroup.alpha = 0;
            rectTransform = GetComponent<RectTransform>();

            initalPosition = rectTransform.anchoredPosition.x;

            WKStaminaSliderPlugin.onColorConfigChange.AddListener(OnColorValuesChanged);

            if (!WKStaminaSliderPlugin.staminaSliderTransitionEnabled.Value) Toggle();
            else rectTransform.anchoredPosition = Vector2.zero; // Making it zero and calling toggle broke the transition off option
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

            int xPos = WKStaminaSliderPlugin.staminaDistanceFromCenter.Value;
            if (isLeft) xPos *= -1;

            if (hidden)
            {
                canvasGroup.DOFade(0, 0.25f).SetEase(Ease.OutQuart);

                rectTransform.DOAnchorPosX(xPos + (xPos < 0 ? 75 : -75),
                    WKStaminaSliderPlugin.staminaTransitionSpeed.Value).SetEase(Ease.OutQuart);
            }
            else
            {
                canvasGroup.DOFade(1, 0.25f).SetEase(Ease.OutQuart);

                rectTransform.DOAnchorPosX(xPos, WKStaminaSliderPlugin.staminaTransitionSpeed.Value)
                    .SetEase(Ease.OutQuart);
            }
        }

        void OnColorValuesChanged()
        {
            GetComponent<Image>().color = WKStaminaSliderPlugin.staminaBackgroundColor.Value;

            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = WKStaminaSliderPlugin.staminaFillColor.Value;
        }

        void OnDestroy()
        {
            WKStaminaSliderPlugin.onColorConfigChange.RemoveListener(OnColorValuesChanged);
        }
    }
}
