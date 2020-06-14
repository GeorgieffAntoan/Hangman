using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleVisibilityController : SingletonMonoBehaviour<ReticleVisibilityController>
{
    public static bool StayHidden = false;
    public static bool ShowLoadingObject = false;

    public static bool UseVisibilityController = true;
    public Renderer m_LaserRenderer;
    private MeshRenderer _Renderer;
    public GameObject m_LoadingObj;

    public static bool HideOneFrame = true;

    void Awake()
    {
        gInstance = this;
        _Renderer = GetComponent<MeshRenderer>();

        if (!m_LoadingObj)
        {
            Transform loadingTransform = transform.Find("Loading");
            if (loadingTransform)
                m_LoadingObj = loadingTransform.gameObject;
        }
    }

	// Update is called once per frame
	void Update ()
	{
	    if (!UseVisibilityController)
	    {
	        _Renderer.enabled = true;
	        m_LaserRenderer.enabled = true;
            return;
        }

        if (ShowLoadingObject && m_LoadingObj)
	    {
	        m_LoadingObj.transform.position = GvrPointerInputModule.Pointer.CurrentRaycastResult.worldPosition;
	        m_LoadingObj.SetActive(true);
	    }
	    else if (m_LoadingObj)
	        m_LoadingObj.SetActive(false);

	    bool overUi = RaycastHelper.IsPointerOverUIObject();
	    _Renderer.enabled = overUi;

        if (m_LaserRenderer)
	        m_LaserRenderer.enabled = overUi;

        if (StayHidden)
	        _Renderer.enabled = false;
        else if (HideOneFrame)
	    {
	        _Renderer.enabled = false;
	        HideOneFrame = false;
	    }
    }
}
