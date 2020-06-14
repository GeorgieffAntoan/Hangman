using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationPanel : SingletonMonoBehaviour<NotificationPanel> {
    public class NotificationData
    {
        public string MessageText;
        public string HeaderText;
        public string CancelButtonText = "Cancel";
        public string AcceptButtonText = "Accept";
        public bool ShowCancelButton = true;
        public bool ShowAcceptButton = true;
        public NotificationType NotificationType = NotificationType.Message;
        public Action AcceptCallback;
        public Action CancelCallback;
    }

    public enum NotificationType
    {
        Message,
        Error
    }

    public float m_NotificationDistance;
    public TextMeshProUGUI m_HeaderText;
    public TextMeshProUGUI m_MessageText;
    public TextMeshProUGUI m_CancelText;
    public TextMeshProUGUI m_AcceptText;

    private Action _AcceptCallback;
    private Action _CancelCallback;

    public GameObject m_CancelButton;

    public GameObject m_NotificationTarget;

    void Awake()
    {
        if (gInstance == null)
            gInstance = this;
        else
            Destroy(gameObject);

        gameObject.SetActive(false);
    }

    public void ShowNotification(NotificationData data, bool childOfCameraContainer = false)
    {
        gameObject.SetActive(true);

        //Reposition
        if (!childOfCameraContainer)
        {
            transform.SetParent(null);
            Vector3 targetPos = new Vector3(0f, 0f, 0f);
            targetPos.z = Mathf.Cos(CameraController.MainCameraObj.transform.eulerAngles.y * Mathf.Deg2Rad) *
                          m_NotificationDistance;
            targetPos.x = Mathf.Sin(CameraController.MainCameraObj.transform.eulerAngles.y * Mathf.Deg2Rad) *
                          m_NotificationDistance;
            transform.position = targetPos;
            transform.rotation =
                Quaternion.LookRotation(transform.position - CameraController.MainCameraObj.transform.position);
        }
        else
        {
            transform.SetParent(m_NotificationTarget.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            StartCoroutine(SetOverrideSorting());
;        }

        _AcceptCallback = data.AcceptCallback;
        _CancelCallback = data.CancelCallback;

        m_CancelButton.SetActive(!string.IsNullOrEmpty(data.CancelButtonText));

        m_MessageText.text = data.MessageText;
        m_HeaderText.text = data.HeaderText;
        m_CancelText.text = data.CancelButtonText;
        m_AcceptText.text = data.AcceptButtonText;
    }

    public void ClickCancel()
    {
        gameObject.SetActive(false);
        if (_CancelCallback != null)
            _CancelCallback();
    }

    public void ClickAccept()
    {
        gameObject.SetActive(false);
        if (_AcceptCallback != null)
            _AcceptCallback();
    }

    IEnumerator SetOverrideSorting()
    {
        yield return null;
        GetComponent<Canvas>().overrideSorting = true;
    }
}
