using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Lunatic.Timer
{
    public class TimerMenuManager : MonoBehaviour
    {
        [SerializeField] private Button _btn_Creator;
        [SerializeField] private RectTransform _prefab;
        [SerializeField] private RectTransform _btns_Holder;
        [SerializeField] private int _max_Btns;
        [SerializeField] private Vector2 _instanceOffset;

        [SerializeField] private float DelayBeforeAnimation;
        [SerializeField] private float DelayBetweenAnimations;
        [SerializeField] private float ButtonAnimationLenght;

        private ButtonCreator _creator;
        private List<Button> _buttons;
        private Dictionary<RectTransform, Vector2> _defaultPositions;

        private Action<int> OnChooseTickable;

        public void Initialize(PlayerData data, TimerSettings settings, Action<int> buttonsCallback)
        {
            OnChooseTickable = buttonsCallback;

            _creator = new ButtonCreator(_btn_Creator, _prefab, _btns_Holder, _instanceOffset, _max_Btns, () =>
            {
                RefreshLists();
                Refresh();
                InitializeTimer(data, settings, data.timers.Count - 1);

            });

            for(int i = 0; i < data.timers.Count; i++)
            {
                _creator.CreateButton(i);
            }

            RefreshLists();
            Refresh();

            foreach(var kvp in _defaultPositions)
            {
                kvp.Key.DOAnchorPosX(-Screen.width, 0f);
            }

            ShowButtons();
        }

        public void ShowButtons()
        {
            StartCoroutine(Animate(true, DelayBeforeAnimation, DelayBetweenAnimations, ButtonAnimationLenght));
        }

        private void RefreshLists()
        {
            _buttons = new List<Button>();
            _defaultPositions = new Dictionary<RectTransform, Vector2>();

            _buttons.AddRange(_btns_Holder.GetComponentsInChildren<Button>().Where(a => a != _btn_Creator));
            _defaultPositions = _buttons.ToDictionary(a => a.targetGraphic.rectTransform, a => a.targetGraphic.rectTransform.anchoredPosition);
            _defaultPositions.Add(_btn_Creator.targetGraphic.rectTransform, _btn_Creator.targetGraphic.rectTransform.anchoredPosition);
        }
        private void Refresh()
        {
            for(int i = 0; i < _buttons.Count; i++)
            {
                var index = i;
                _buttons[i].onClick.RemoveAllListeners();
                _buttons[i].onClick.AddListener(() =>
                {
                    OnChooseTickable?.Invoke(index);
                    StartCoroutine(Animate(false, DelayBeforeAnimation, DelayBetweenAnimations, ButtonAnimationLenght));
                });
            }
        }

        private void InitializeTimer(PlayerData data, TimerSettings settings, int index)
        {
            if(data[index] == null)
                data.timers.Add(index, settings.DefaultTimerInSeconds);
        }

        private IEnumerator Animate(bool FadeIn, float before, float between, float fly)
        {
            yield return new WaitForSeconds(before);

            foreach(var kvp in _defaultPositions)
            {
                kvp.Key.DOAnchorPosX(FadeIn ? kvp.Value.x : -Screen.width, fly);
                yield return new WaitForSeconds(between);
            }
        }
    }
}
