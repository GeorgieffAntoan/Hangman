using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Fader : MonoBehaviour
{
    protected static List<Fader> Faders = new List<Fader>();

    [NonSerialized]
    public CanvasGroup m_CanvasGroup;

    [NonSerialized]
    public GameObject m_LoadingObject;

    public bool m_FadeInOnStart;
    public bool m_BlackAtStart;
    public bool m_LoadingAtStart;
    public float m_Duration;

    public bool m_FadedIn;
    public bool m_Tweening;

    [NonSerialized]
    public bool m_BlackFromDismount;

    // Use this for initialization
    void Awake()
    {

        m_FadedIn = !m_BlackAtStart;

        m_CanvasGroup = GetComponent<CanvasGroup>();
        if (m_BlackAtStart || m_FadeInOnStart)
        {
            m_CanvasGroup.alpha = 1f;
            if (m_LoadingAtStart)
            {
                m_LoadingObject.SetActive(true);
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
    }

    void OnDestroy()
    {
        Faders.Remove(this);
    }

    public void SetBlackForHmdDismount()
    {
        m_BlackFromDismount = true;
        m_CanvasGroup.alpha = 1f;
    }

    public static void SetBlackForHmdDismountAll()
    {
        foreach (Fader fader in Faders)
        {
            if (!fader.m_FadedIn)
                continue;

            SphereFader sphereFader = fader as SphereFader;
            if (sphereFader != null)
            {
                sphereFader.SetBlackForHmdDismount();
                continue;
            }

            if (fader)
            {
                fader.SetBlackForHmdDismount();
            }
        }
    }

    public static void FadeInAll(Action onFadeIn = null, bool forceBlack = false)
    {
        foreach (Fader fader in Faders)
        {
            SphereFader sphereFader = fader as SphereFader;
            if (sphereFader != null)
            {
                sphereFader.FadeIn(onFadeIn, forceBlack);
                continue;
            }

            if (fader)
                fader.FadeIn(onFadeIn,forceBlack);
        }
    }


    public void FadeIn(Action onFadeIn = null, bool forceBlack = false)
    {
        if (forceBlack)
            m_CanvasGroup.alpha = 1f;
        else if (Math.Abs(m_CanvasGroup.alpha) < 0.01f)
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
                    m_LoadingObject.SetActive(false);
                    FadeObjectIn(onFadeIn);
                });
        }
        else
        {
            FadeObjectIn(onFadeIn);
        }
    }

    public void FadeObjectIn(Action onFadeIn = null)
    {
        DOTween.To(() => m_CanvasGroup.alpha, newAlpha => m_CanvasGroup.alpha = newAlpha, 0f, m_Duration).OnComplete(delegate
        {
            m_FadedIn = true;
            if (onFadeIn != null) onFadeIn();
        });
    }

    public static void FadeOutAll(Action onFadeOut = null, bool showLoading = false)
    {
        foreach (Fader fader in Faders)
        {
            SphereFader sphereFader = fader as SphereFader;
            if (sphereFader != null)
            {
                sphereFader.FadeOut(onFadeOut, showLoading);
                continue;
            }

            fader.FadeOut(onFadeOut,showLoading);
        }
    }

    public void FadeOut(Action onFadeOut = null, bool showLoading = false, bool forceBlack = false)
    {
        if (forceBlack)
            m_CanvasGroup.alpha = 0f;

        DOTween.To(() => m_CanvasGroup.alpha, newAlpha => m_CanvasGroup.alpha = newAlpha, 1f, m_Duration).OnComplete(delegate
        {
            m_FadedIn = false;

            if (onFadeOut != null) onFadeOut();

            if (m_LoadingObject && showLoading)
            {
                CameraController.getInstance().PositionObjectInFrontOfCamera(m_LoadingObject);
                m_LoadingObject.SetActive(true);
                CanvasGroup loadingGroup = m_LoadingObject.GetComponentInChildren<CanvasGroup>();
                loadingGroup.alpha = 0f;
                DOTween.To(() => loadingGroup.alpha, newAlpha => loadingGroup.alpha = newAlpha, 1f, 0.25f);
            }
        });
    }
}
