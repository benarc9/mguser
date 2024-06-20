// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using Netcode.Transports.MultipeerConnectivity;

public class PlayerController : NetworkBehaviour
{
    private static readonly string TAG = "Player";

    public NetworkVariable<FixedString64Bytes> Nickname = new("yuchen", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public static event Action<PlayerController> OnPlayerSpawned;

    public static event Action<PlayerController> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"!!! {TAG} -- Player spawned via NetworkManager...");
        Debug.Log($"!!! {TAG}\t-- Checking if this instance is owner of network object...");
        if (IsOwner)
        {
            Debug.Log($"!!! {TAG}\t\t-- Instance IS owner of network object");
            Nickname.Value = MultipeerConnectivityTransport.Instance.Nickname;
            Debug.Log($"!!! {TAG}\t\t-- Set network object nickname to: {Nickname.Value}");
        }

        OnPlayerSpawned?.Invoke(this);
    }

    public override void OnNetworkDespawn()
    {
        Debug.Log($"!!! {TAG} -- Network player object despawning...");
        OnPlayerDespawned?.Invoke(this);
    }
}