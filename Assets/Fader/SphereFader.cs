using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SphereFader : Fader
{
    private Material _FaderMaterial;

    // Use this for initialization
    void Awake()
    {
        _FaderMaterial = GetComponent<Renderer>().sharedMaterial;
        if (m_LoadingObject)
            m_LoadingObject.SetActive(m_LoadingAtStart);

        m_FadedIn = !m_BlackAtStart;

        if (m_BlackAtStart || m_FadeInOnStart)
        {
            _FaderMaterial.color = Color.black;
            if (m_LoadingAtStart && m_LoadingObject)
            {
                CanvasGroup loadingGroup = m_LoadingObject.GetComponentInChildren<CanvasGroup>();
                DOTween.To(() => loadingGroup.alpha, newAlpha => loadingGroup.alpha = newAlpha, 1f, 0.5f);
            }
        }

        Faders.Add(this);
    }

    void Start()
    {
        if (m_FadeInOnStart)
            FadeIn();
    }

    void OnDestroy()
    {
        Faders.Remove(this);
    }

    void OnEnable()
    {
        OVRManager.HMDMounted += OnHmdMount; ;
        OVRManager.HMDUnmounted += SetBlackForHmdDismount;
    }

    private void OnHmdMount()
    {
        if (m_BlackFromDismount)
            FadeIn();
    }

    private void OnDisable()
    {
        OVRManager.HMDMounted -= OnHmdMount;
        OVRManager.HMDUnmounted -= SetBlackForHmdDismount;
        _FaderMaterial.color = Color.clear;
    }

    private void OnApplicationQuit()
    {
        _FaderMaterial.color = Color.black;
    }

    [ContextMenu("FadeIn")]
    public void FadeInSimple()
    {
        FadeIn(null,true);
    }

    [ContextMenu("FadeOut")]
    public void FadeOutSimple()
    {
        FadeOut(null, true, true);
    }

    public new void FadeIn(Action onFadeIn = null, bool forceBlack = false)
    {
        if (forceBlack)
            _FaderMaterial.color = Color.black;
        else if (Math.Abs(_FaderMaterial.color.a) < 0.01f)
        {
            m_FadedIn = true;
            if (onFadeIn != null)
                onFadeIn();
            return;
        }
        if (m_LoadingObject)
        {
            CanvasGroup loadingGroup = m_LoadingObject.GetComponentInChildren<CanvasGroup>();
            DOTween.To(() => loadingGroup.alpha, newAlpha => loadingGroup.alpha = newAlpha, 0f, 0.25f)
                .OnComplete(delegate
                {
                    m_FadedIn = true;
                    m_LoadingObject.SetActive(false);
                    FadeObjectIn(onFadeIn);
                });
        }
        else
        {
            FadeObjectIn(onFadeIn);
        }
    }

    public new void FadeObjectIn(Action onFadeIn = null)
    {
        _FaderMaterial.DOFade(0f,m_Duration).OnComplete(delegate
        {
            m_FadedIn = true;

            if (onFadeIn != null) onFadeIn();
        });
    }

    public new void FadeOut(Action onFadeOut = null, bool showLoading = false, bool forceBlack = false)
    {
        if (forceBlack)
            _FaderMaterial.color = Color.clear;

        _FaderMaterial.DOFade(1f, m_Duration).OnComplete(delegate
        {
            m_FadedIn = false;

            if (onFadeOut != null) onFadeOut();

            if (m_LoadingObject && showLoading)
            {
                //CameraController.getInstance().PositionObjectInFrontOfCamera(m_LoadingObject);
                m_LoadingObject.SetActive(true);
                CanvasGroup loadingGroup = m_LoadingObject.GetComponentInChildren<CanvasGroup>();
                loadingGroup.alpha = 0f;
                DOTween.To(() => loadingGroup.alpha, newAlpha => loadingGroup.alpha = newAlpha, 1f, 0.25f);
            }
        });
    }

    public new void SetBlackForHmdDismount()
    {
        m_BlackFromDismount = true;
        _FaderMaterial.color = Color.black;
    }
}
