// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Netcode.Transports.MultipeerConnectivity;

public class NearbyHostSlot : MonoBehaviour
{
    private static readonly string TAG = "NearbyHostSlot";

    [SerializeField] private TMP_Text _nickname;

    [SerializeField] private Button _joinButton;

    public void Init(int nearbyHostKey, string nickname)
    {
        Debug.Log($"!!! {TAG} -- Initializing new nearby host item in nearby host list...");
        _nickname.text = nickname;
        Debug.Log($"!!! {TAG}\t-- Set nickname to {nickname}");
        Debug.Log($"!!! {TAG}\t-- Nearby Host Key: {nearbyHostKey}");
        _joinButton.onClick.AddListener(() =>
        {
            MultipeerConnectivityTransport.Instance.SendConnectionRequest(nearbyHostKey);
        });
    }
}
