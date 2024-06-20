// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Netcode.Transports.MultipeerConnectivity;

public class HostPanel : MonoBehaviour
{
    private static readonly string TAG = "HostPanel";

    [SerializeField] private TMP_Text _advertisingStatusText;

    [SerializeField] private Button _startAdvertisingButton;

    [SerializeField] private Button _stopAdvertisingButton;

    private MultipeerConnectivityTransport _mpcTransport;

    private const string ADVERTISING_STATUS_PREFIX = "Advertising Status: ";

    public bool isAdvertising = false;

    private void Start()
    {
        _mpcTransport = MultipeerConnectivityTransport.Instance;
    }

    private void Update()
    {
        if (_mpcTransport.IsAdvertising != isAdvertising)
        {
            Debug.Log($"!!! {TAG} -- Advertising Status Changed. Is host advertising?: {_mpcTransport.IsAdvertising}");
            isAdvertising = _mpcTransport.IsAdvertising;
        }
        
        if (_mpcTransport.IsAdvertising)
        {
            _advertisingStatusText.text = ADVERTISING_STATUS_PREFIX + "Advertising";
            _startAdvertisingButton.interactable = false;
            _stopAdvertisingButton.interactable = true;
        }
        else
        {
            _advertisingStatusText.text = ADVERTISING_STATUS_PREFIX + "Not Advertising";
            _startAdvertisingButton.interactable = true;
            _stopAdvertisingButton.interactable = false;
        }
    }
}
