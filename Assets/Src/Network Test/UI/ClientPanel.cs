// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Netcode.Transports.MultipeerConnectivity;

public class ClientPanel : MonoBehaviour
{
    private static readonly string TAG = "ClientPanel";

    [SerializeField] private TMP_Text _browsingStatusText;

    [SerializeField] private Button _startBrowsingButton;

    [SerializeField] private Button _stopBrowsingButton;

    private MultipeerConnectivityTransport _mpcTransport;

    private const string BROWSING_STATUS_PFEFIX = "Browsing Status: ";

    public bool isBrowsing = false;

    private void Start()
    {
        _mpcTransport = MultipeerConnectivityTransport.Instance;
    }

    private void Update()
    {
        if (isBrowsing != MultipeerConnectivityTransport.Instance.IsBrowsing)
        {
            Debug.Log($"!!! {TAG} -- Browsing status changed.");
            Debug.Log($"!!! {TAG} -- Is Browsing?: {MultipeerConnectivityTransport.Instance.IsBrowsing}");
            isBrowsing = MultipeerConnectivityTransport.Instance.IsBrowsing;
        }
        
        // The client can no longer browse after connected to the network
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.Log($"!!! {TAG} -- Client is connected ton the network, can no longer browse after being connected. Stopping browsing.");
            _browsingStatusText.text = BROWSING_STATUS_PFEFIX + "Not Browsing";
            _startBrowsingButton.interactable = false;
            _stopBrowsingButton.interactable = false;
            return;
        }

        if (_mpcTransport.IsBrowsing)
        {
            _browsingStatusText.text = BROWSING_STATUS_PFEFIX + "Browsing";
            _startBrowsingButton.interactable = false;
            _stopBrowsingButton.interactable = true;
        }
        else
        {
            _browsingStatusText.text = BROWSING_STATUS_PFEFIX + "Not Browsing";
            _startBrowsingButton.interactable = true;
            _stopBrowsingButton.interactable = false;
        }
    }
}
