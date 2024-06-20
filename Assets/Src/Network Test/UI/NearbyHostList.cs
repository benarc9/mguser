// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.MultipeerConnectivity;

public class NearbyHostList : MonoBehaviour
{
    private static readonly string TAG = "NearbyHostList";

    [SerializeField] private NearbyHostSlot _nearbyHostSlotPrefab;

    [SerializeField] private RectTransform _root;

    private MultipeerConnectivityTransport _mpcTransport;

    public List<NearbyHostSlot> NearbyHostSlotList => _nearbyHostSlotList;

    private readonly List<NearbyHostSlot> _nearbyHostSlotList = new();

    private void Start()
    {
        // Get the reference of the MPC transport
        _mpcTransport = MultipeerConnectivityTransport.Instance;

        _mpcTransport.OnBrowserFoundPeer += OnBrowserFoundPeer;
        _mpcTransport.OnBrowserLostPeer += OnBrowserLostPeer;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        UpdateNearbyHostList();
    }

    private void OnBrowserFoundPeer(int _, string hostName)
    {
        Debug.Log($"!!! {TAG} -- Browser found peer with host name: {hostName}");
        UpdateNearbyHostList();
    }

    private void OnBrowserLostPeer(int _, string hostName)
    {
        Debug.Log($"!!! {TAG} -- Browser lost peer with host name: {hostName}");
        UpdateNearbyHostList();
    }

    private void OnClientConnected(ulong _)
    {
        Debug.Log($"!!! {TAG} -- Client connected to host");
        UpdateNearbyHostList();
    }

    private void UpdateNearbyHostList()
    {
        Debug.Log($"!!! {TAG} -- Updating nearby host list...");
        // We destroy and instantiate every connection request slot in every frame.
        // This is wasteful and unnecessary. But it is less error-prone.
        // You can register callbacks instead.
        foreach (var slot in _nearbyHostSlotList)
        {
            Debug.Log($"!!! {TAG}\t-- Destroying nearby host list item");
            Destroy(slot.gameObject);
        }
        _nearbyHostSlotList.Clear();

        foreach (var nearbyHostKey in _mpcTransport.NearbyHostDict.Keys)
        {
            Debug.Log($"!!! {TAG}\t-- Creating new nearby host list item...");
            var hostName = _mpcTransport.NearbyHostDict[nearbyHostKey];

            var nearbyHostSlotInstance = Instantiate(_nearbyHostSlotPrefab);
            nearbyHostSlotInstance.Init(nearbyHostKey, hostName);
            Debug.Log($"!!! {TAG}\t\t-- Nearby Host Key: {nearbyHostKey}");
            Debug.Log($"!!! {TAG}\t\t-- Nearby Host Name: {hostName}");
            nearbyHostSlotInstance.transform.localScale = Vector3.one;
            nearbyHostSlotInstance.transform.SetParent(_root, false);

            _nearbyHostSlotList.Add(nearbyHostSlotInstance);
        }
    }
}
