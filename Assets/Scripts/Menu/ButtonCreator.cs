using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lunatic.Timer
{
    public class ButtonCreator
    {
        private readonly Button _btn_Create;
        private readonly RectTransform _prefab;
        private readonly RectTransform _btns_holder;
        private readonly Vector2 _instanceOffset;
        private readonly int _max_Btns;

        public ButtonCreator(Button btn_Create, RectTransform prefab, RectTransform btns_holder, Vector2 instanceOffset, int max_Btns, Action OnCreatedCallback)
        {
            _btn_Create = btn_Create;
            _prefab = prefab;
            _btns_holder = btns_holder;
            _instanceOffset = instanceOffset;
            _max_Btns = max_Btns;

            _btn_Create.onClick.RemoveAllListeners();
            _btn_Create.onClick.AddListener(() =>
            {
                var btns_count = _btns_holder.GetComponentsInChildren<Button>().Length - 1;

                if(btns_count == _max_Btns) return;

                CreateButton(btns_count);
                OnCreatedCallback?.Invoke();
            });
        }

        public void CreateButton(int btnsCount)
        {
            var button = UnityEngine.Object.Instantiate(_prefab, _btns_holder);
            var btnPos = _btn_Create.targetGraphic.rectTransform.anchoredPosition;
            button.anchoredPosition = btnPos;
            _btn_Create.targetGraphic.rectTransform.anchoredPosition = btnPos + _instanceOffset;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Timer " + (btnsCount + 1);

            if(btnsCount + 1 == _max_Btns)
            {
                _btn_Create.onClick.RemoveAllListeners();
                _btn_Create.interactable = false;
            }
        }
    }
}
