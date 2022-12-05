using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{

    [SerializeField] private float introDisplayDuration;
    [SerializeField] private float hudDelay;
    [SerializeField] private float startDelay;
    [SerializeField] private IntroDisplay introDisplay;
    [SerializeField] private HUDSlide[] huds;
    [SerializeField] private StartDisplay startDisplay;
    [SerializeField] private CanvasGroup fade;

    private void Start()
    {
        // 一応仮のつもり
        // マッチ管理クラス的なの作るならそこから呼ぶ or そっちにこのクラス統合するのがよさげ
        // (特にstartDisplay、実際の開始と合わせたいし)
        // 作らないならこのままでいいや
        RunAsync().Forget();
    }

    public async UniTask RunAsync()
    {
        introDisplay.gameObject.SetActive(true);
        introDisplay.Display();
        await UniTask.Delay(TimeSpan.FromSeconds(introDisplayDuration));
        await fade.DOFade(1, 0.5f).From(0).SetEase(Ease.InQuad);
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        
        introDisplay.gameObject.SetActive(false); // もう表示しないのでオブジェクトごとOFF!w
        // introDisplayでカメラ動かすならここで戻す
        
        await fade.DOFade(0, 0.3f);
        await UniTask.Delay(TimeSpan.FromSeconds(hudDelay));
        foreach (var hud in huds)
        {
            hud.SlideInAsync().Forget();
        }
        
        await UniTask.Delay(TimeSpan.FromSeconds(startDelay));
        startDisplay.gameObject.SetActive(true);
        startDisplay.Display();
    }
    
}
