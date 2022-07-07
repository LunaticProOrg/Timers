using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Manager _manager;
    [SerializeField] private Button buttonPrefab;

    [SerializeField] private RectTransform _holder;
    [SerializeField] private Button _makerButton;

    [SerializeField] private int maxButtons;
    [SerializeField] private float DelayBeforeAnimation;
    [SerializeField] private float DelayBetweenAnimations;
    [SerializeField] private Vector2 instanceOffset;

    private ButtonsMaker _maker;

    private List<Button> _buttons;
    private List<RectTransform> _buttonsRects;
    private Dictionary<RectTransform, Vector3> _defaultPositions;

    private void Awake()
    {
        _manager.Load();
        _maker = new ButtonsMaker(buttonPrefab, maxButtons, instanceOffset, _makerButton, _holder, null);
        var buttonsCount = _holder.GetComponentsInChildren<Button>().Where(a => a != _makerButton).Count();

        if(_manager.Data.buttonsCount > buttonsCount)
        {
            for(int i = 0; i < _manager.Data.buttonsCount; i++)
            {
                _maker.CreateNewButton(buttonsCount + i);
            }
        }

        _maker = new ButtonsMaker(buttonPrefab, maxButtons, instanceOffset, _makerButton, _holder, () => Setup(false, true));

        Setup();
    }

    private void Setup(bool animate = true, bool incrementButtonsCount = false)
    {
        _buttons = new List<Button>();
        _buttonsRects = new List<RectTransform>();
        _defaultPositions = new Dictionary<RectTransform, Vector3>();

        _buttons.AddRange(_holder.GetComponentsInChildren<Button>().Where(a => a != _makerButton));

        for(int i = 0; i < _buttons.Count; i++)
        {
            _buttonsRects.Add(_buttons[i].targetGraphic.rectTransform);

            var index = i;
            _buttons[i].onClick.RemoveAllListeners();
            _buttons[i].onClick.AddListener(() => ShowBanner(index));
        }

        _buttonsRects.Add(_makerButton.targetGraphic.rectTransform);

        _defaultPositions = _buttonsRects.ToDictionary(a => a, a => a.anchoredPosition3D);

        if(incrementButtonsCount) _manager.Data.buttonsCount++;

        if(animate)
        {
            foreach(var buttonRectTransform in _buttonsRects)
            {
                buttonRectTransform.DOAnchorPosX(-Screen.width, 0f);
            }
        }

    }

    private void Start()
    {
        Show();
    }

    public void Show()
    {
        StartCoroutine(Animation(true));
    }

    private void ShowBanner(int buttonIndex)
    {
        Hide();

        GetComponent<TimerBanner>().ShowBanner(buttonIndex);
    }

    private void Hide()
    {
        StartCoroutine(Animation(false));
    }

    private IEnumerator Animation(bool show)
    {
        yield return new WaitForSeconds(DelayBeforeAnimation);

        foreach(var rect in _defaultPositions)
        {
            rect.Key.DOAnchorPosX(show ? rect.Value.x : -Screen.width, 1f);
            yield return new WaitForSeconds(DelayBetweenAnimations);
        }

    }

    private void OnDestroy()
    {
        _manager.Save();
    }
}
