using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public class ButtonsMaker
{
    private readonly Button Maker;
    private readonly RectTransform buttonsHolder;
    private readonly Button buttonPrefab;
    private readonly int maxButtons;
    private readonly Vector2 instanceOffset;
    public ButtonsMaker(Button buttonPrefab, int maxButtons, Vector2 instanceOffset, Button maker, RectTransform buttonsHolder, Action OnCreateCallback)
    {
        this.buttonPrefab = buttonPrefab;
        this.maxButtons = maxButtons;
        this.instanceOffset = instanceOffset;
        Maker = maker;
        this.buttonsHolder = buttonsHolder;

        Maker.onClick.RemoveAllListeners();
        Maker.onClick.AddListener(() =>
        {
            var contains = buttonsHolder.GetComponentsInChildren<Button>().Length - 1;

            if(contains == maxButtons) return;

            CreateNewButton(contains);
            OnCreateCallback?.Invoke();
        });
    }

    public void CreateNewButton(int buttonsCount)
    {
        var button = CreateButton();
        var mackerPos = Maker.targetGraphic.rectTransform.anchoredPosition;
        button.targetGraphic.rectTransform.anchoredPosition = mackerPos;
        Maker.targetGraphic.rectTransform.anchoredPosition = mackerPos + instanceOffset;
        button.GetComponentInChildren<TextMeshProUGUI>().text = "Timer " + (buttonsCount + 1);

        if(buttonsCount + 1 == maxButtons)
        {
            Maker.onClick.RemoveAllListeners();
            Maker.interactable = false;
        }
    }

    private Button CreateButton()
    {
        return UnityEngine.Object.Instantiate(buttonPrefab, buttonsHolder);
    }
}
