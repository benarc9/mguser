// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using UnityEngine;
using Netcode.Transports.MultipeerConnectivity;

public class StartPanel : MonoBehaviour
{
    private static readonly string TAG = "StartPanel";

    private AppController app;

    void Start(){
        app = GameObject.FindGameObjectWithTag("App").GetComponent<AppController>();
    }

    public void OnNicknameChanged(string nickname)
    {
        Debug.Log($"!!! {TAG} -- Nickname changed to: {nickname}");
        MultipeerConnectivityTransport.Instance.Nickname = nickname;
    }

    public void OnAutoAdvertiseToggled(bool value)
    {
        MultipeerConnectivityTransport.Instance.AutoAdvertise = value;

        Debug.Log($"!!! {TAG} -- AutoAdvertise changed to {value}");
    }

    public void OnAutoApproveConnectionRequestToggled(bool value)
    {
        MultipeerConnectivityTransport.Instance.AutoApproveConnectionRequest = value;

        Debug.Log($"!!! {TAG} -- AutoApproveConnectionRequest changed to {value}");
    }

    public void OnAutoBrowseToggled(bool value)
    {
        MultipeerConnectivityTransport.Instance.AutoBrowse = value;

        Debug.Log($"!!! {TAG} -- AutoBrowse changed to {value}");
    }

    public void OnAutoSendConnectionRequestToggled(bool value)
    {
        MultipeerConnectivityTransport.Instance.AutoSendConnectionRequest = value;

        Debug.Log($"!!! {TAG} -- AutoSendConnectionRequest changed to {value}");
    }
}
