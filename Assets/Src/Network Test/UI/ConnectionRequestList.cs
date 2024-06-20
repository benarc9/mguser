// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Netcode.Transports.MultipeerConnectivity;

public class ConnectionRequestList : MonoBehaviour
{
    private static readonly string TAG = "ConnectionRequestList";

    [SerializeField] private ConnectionRequestSlot _connectionRequestSlotPrefab;

    [SerializeField] private RectTransform _root;

    private MultipeerConnectivityTransport _mpcTransport;

    public List<ConnectionRequestSlot> ConnectionRequestSlotList => _connectionRequestSlotList;

    private readonly List<ConnectionRequestSlot> _connectionRequestSlotList = new();

    private void Start()
    {
        // Get the reference of the MPC transport
        _mpcTransport = MultipeerConnectivityTransport.Instance;

        _mpcTransport.OnAdvertiserReceivedConnectionRequest += OnAdvertiserReceivedConnectionRequest;
        _mpcTransport.OnAdvertiserApprovedConnectionRequest += OnAdvertiserApprovedConnectionRequest;
        UpdateConnectionRequestList();
    }

    private void OnAdvertiserReceivedConnectionRequest(int _, string senderName)
    {
        Debug.Log($"!!! {TAG} -- Advertiser received connection request");
        UpdateConnectionRequestList();
    }

    private void OnAdvertiserApprovedConnectionRequest(int _)
    {
        Debug.Log($"!!! {TAG} -- Advertiser approved connection request");
        UpdateConnectionRequestList();
    }

    private void UpdateConnectionRequestList()
    {
        Debug.Log($"!!! {TAG} -- Updating connection request list");
        foreach (var slot in _connectionRequestSlotList)
        {
            Debug.Log($"!!! {TAG} -- Destroying connection request item");
            Destroy(slot.gameObject);
        }
        _connectionRequestSlotList.Clear();

        foreach (var connectionRequestKey in _mpcTransport.PendingConnectionRequestDict.Keys)
        {
            Debug.Log($"!!! {TAG} -- Creating connection request item...");
            var senderName = _mpcTransport.PendingConnectionRequestDict[connectionRequestKey];
            Debug.Log($"!!! {TAG}\t-- Sender Name: {senderName}");
            Debug.Log($"!!! {TAG}\t-- Connection Request Key: {connectionRequestKey}");
            var connectionRequestSlotInstance = Instantiate(_connectionRequestSlotPrefab);
            connectionRequestSlotInstance.Init(connectionRequestKey, senderName);
            connectionRequestSlotInstance.transform.localScale = Vector3.one;
            connectionRequestSlotInstance.transform.SetParent(_root, false);
            _connectionRequestSlotList.Add(connectionRequestSlotInstance);
            Debug.Log($"!!! {TAG} -- Connection request should be visible on host panel in connection request list");
        }
    }
}
