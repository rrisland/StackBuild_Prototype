using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;
using Cinemachine;

namespace StackProto
{
    public class FinishSequence : MonoBehaviour
    {

        [Serializable]
        internal struct Player
        {

            [SerializeField] public BuildingSpace space;
            [SerializeField] public ResultsHeightDisplay heightDisplay;

        }

        [SerializeField] private List<Player> players;
        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private TMP_Text finishText;
        [SerializeField] private CanvasGroup heightDisplayContainer;

        public async UniTaskVoid DisplayAsync()
        {
            gameObject.SetActive(true);
            vcam.gameObject.SetActive(true);
            heightDisplayContainer.gameObject.SetActive(false);
            
            _ = finishText.transform.DOScale(1, 1.5f).From(2).SetEase(Ease.OutQuart);
            _ = finishText.DOFade(0, 1.5f).From(1).SetEase(Ease.InQuart);
            await UniTask.Delay(500);
            
            foreach (var player in players)
            {
                _ = player.space.transform.DOLocalMoveY(0, 2).SetEase(Ease.OutQuart);
            }
            await UniTask.Delay(2500);

            var winner = players.Aggregate((a, b) => a.space.Height > b.space.Height ? a : b);
            heightDisplayContainer.gameObject.SetActive(true);
            _ = heightDisplayContainer.DOFade(1, 0.15f).From(0);
            
            _ = vcam.transform.DOLocalMoveY(winner.space.Height, 4).SetEase(Ease.InQuad);
            await DOVirtual.Float(0, winner.space.Height, 4, y =>
            {
                foreach (var player in players)
                {
                    player.heightDisplay.SetHeight(Mathf.Min(y, player.space.Height));
                }
            }).SetEase(Ease.InQuad);
            
            await winner.heightDisplay.DisplayWinAsync();

            /* 下から1つずつみるやつ
            foreach (var space in buildingSpaces.OrderBy(space => space.Height))
            {
                await vcam.transform.DOLocalMoveY(space.transform.position.y + space.Height, 2).SetEase(Ease.OutCubic).AsyncWaitForCompletion();
            }
            */
        }

    }
}
