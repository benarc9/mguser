// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Netcode.Transports.MultipeerConnectivity;
using System;

public class AppController : MonoBehaviour
{
    private static readonly string TAG = "AppController";

    [SerializeField] private PlayerController _playerPrefab;

    public Dictionary<ulong, PlayerController> PlayerDict => _playerDict;

    private readonly Dictionary<ulong, PlayerController> _playerDict = new();

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        PlayerController.OnPlayerSpawned += OnPlayerSpawned;
        PlayerController.OnPlayerDespawned += OnPlayerDespawned;
        MultipeerConnectivityTransport.Instance.OnTransportEvent += OnTransportEvent;
    }

    private void OnDestroy()
    {
        // Static events must be unregistered, otherwise the app will crash
        PlayerController.OnPlayerSpawned -= OnPlayerSpawned;
        PlayerController.OnPlayerDespawned -= OnPlayerDespawned;
        MultipeerConnectivityTransport.Instance.DisconnectLocalClient();
        MultipeerConnectivityTransport.Instance.Shutdown();
        // NetworkManager.Singleton.Shutdown(true);
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"!!! {TAG} -- Client connected...");
        Debug.Log($"!!! {TAG}\t-- Client ID: {clientId}");
        Debug.Log($"!!! {TAG}\t-- Checking if client is server...");
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"!!! {TAG}\t\t-- Client IS server, spawning player with client ID: {clientId}");
            SpawnPlayer(clientId);
        }
        else
        {
            Debug.Log($"!!! {TAG}\t\t-- Client is NOT server, not spawning player");
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"!!! {TAG} -- Client disconnected...");
        Debug.Log($"!!! {TAG}\t-- Checking if client is server...");
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"!!! {TAG}\t\t-- Client IS server...");
            Debug.Log($"!!! {TAG}\t\t\t-- Checking if player dictionary contains client ID: {clientId}...");
            if (_playerDict.ContainsKey(clientId))
            {
                Debug.Log($"!!! {TAG}\t\t\t\t-- Player dictionary contains client ID: {clientId}");
                var player = _playerDict[clientId];
                if (player != null)
                {
                    Debug.Log($"!!! {TAG}\t\t\t\t-- Destroying disconneted player entry in player dictionary");
                    Destroy(_playerDict[clientId].gameObject);
                }
            }
            else {
                Debug.Log($"!!! {TAG}\t\t\t\t-- Player with client ID: {clientId} not found in player dictionary. Nothing to do.");
            }
        }
        else
        {
            Debug.Log($"!!! {TAG} -- Client is NOT server. Nothing to do.");
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        Debug.Log($"!!! {TAG} -- Attempting to spawn player object with client ID: {clientId}");
        var playerInstance = Instantiate(_playerPrefab);
        playerInstance.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        Debug.Log($"!!! {TAG} -- Spawned player network object via NetworkManager with client ID: {clientId}");
    }

    private void OnPlayerSpawned(PlayerController player)
    {
        Debug.Log($"!!! {TAG} -- Player spawned, adding to player dictionary");
        _playerDict.Add(player.OwnerClientId, player);
    }

    private void OnPlayerDespawned(PlayerController player)
    {
        Debug.Log($"!!! {TAG} -- Player despawned, removing from player dictionary");
        if (_playerDict.ContainsKey(player.OwnerClientId))
        {
            _playerDict.Remove(player.OwnerClientId);
        }
    }

    public void StartHost()
    {
        Debug.Log($"!!! {TAG} -- Starting host...");
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        Debug.Log($"!!! {TAG} -- Starting client...");
        NetworkManager.Singleton.StartClient();
    }

    public void StartAdvertising()
    {
        Debug.Log($"!!! {TAG} -- Started advertising...");
        MultipeerConnectivityTransport.Instance.StartAdvertising();
    }

    public void StopAdvertising()
    {
        Debug.Log($"!!! {TAG} -- Stopped advertising...");
        MultipeerConnectivityTransport.Instance.StopAdvertising();
    }

    public void StartBrowsing()
    {
        Debug.Log($"!!! {TAG} -- Started browsing...");
        MultipeerConnectivityTransport.Instance.StartBrowsing();
    }

    public void StopBrowsing()
    {
        Debug.Log($"!!! {TAG} -- Stopped browsing...");
        MultipeerConnectivityTransport.Instance.StopBrowsing();
    }

    public void Shutdown()
    {
        Debug.Log($"!!! {TAG} -- Shutting down networking...");
        NetworkManager.Singleton.Shutdown();
    }
    
    public void OnTransportEvent(NetworkEvent _event, ulong clientId, ArraySegment<byte> payload, float receiveTime)
    {
        Debug.Log($"!!! {TAG} -- Transport Event --- {receiveTime}");
        Debug.Log($"!!! {TAG}\t-- Network Event: {_event.ToString()}");
        Debug.Log($"!!! {TAG}\t-- Client ID: {clientId}");
        Debug.Log($"!!! {TAG}\t-- Payload: {payload.ToString()}");
    }
}
