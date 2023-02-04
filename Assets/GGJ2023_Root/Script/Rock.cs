using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rock : MonoBehaviour
{
    [SerializeField] SpriteRenderer _rockSprite;
    [SerializeField] float _tweenDuration;

    List<Guid> _tokenList = new List<Guid>();

    Sequence _failSequence;

    private void Start()
    {
        _tokenList.Add(MessageHubSingleton.Instance.Subscribe<FailedGrowthRequirementEvent>((e) =>
        {
            if (e.blockingGameObject == this.gameObject)
            {
                OnFailedGrowthRequirement();
            }
        }));
    }

    private void OnDestroy()
    {
        MessageHubSingleton.Instance.Unsubscribe(_tokenList);
    }

    private void OnFailedGrowthRequirement()
    {
        if (_failSequence.IsActive()) return;

        _failSequence = DOTween.Sequence();

        _failSequence
        .Append(
            _rockSprite.DOColor(Color.red, _tweenDuration)
        )
        .AppendInterval(
            _tweenDuration
        )
        .Append(
            _rockSprite.DOColor(Color.white, _tweenDuration)
        );
    }
}