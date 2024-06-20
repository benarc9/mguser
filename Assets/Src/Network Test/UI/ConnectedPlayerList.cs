// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ConnectedPlayerList : MonoBehaviour
{
    private readonly static string TAG = "ConnectedPlayerList";

    [SerializeField] private GameObject _playerSlotPrefab;

    [SerializeField] private RectTransform _root;

    private AppController _app;

    private readonly List<GameObject> _playerSlotList = new();

    private void Start()
    {
        _app = FindObjectOfType<AppController>();

        Player.OnPlayerSpawned += OnPlayerSpawned;
        Player.OnPlayerDespawned += OnPlayerDespawned;
        UpdateConnectedPlayerList();
    }

    private void OnDestroy()
    {
        Player.OnPlayerSpawned -= OnPlayerSpawned;
        Player.OnPlayerDespawned -= OnPlayerDespawned;
    }

    private void OnPlayerSpawned(Player _)
    {
        UpdateConnectedPlayerList();
    }

    private void OnPlayerDespawned(Player _)
    {
        UpdateConnectedPlayerList();
    }

    private void UpdateConnectedPlayerList()
    {
        Debug.Log($"!!! {TAG} -- Updating Connected Player List...");
        // We destroy and instantiate every connection request slot in every frame.
        // This is wasteful and unnecessary. But it is less error-prone.
        // You can register callbacks instead.
        foreach (var playerSlot in _playerSlotList)
        {
            Debug.Log($"!!! {TAG} -- Destroying player slot gameobject");
            Destroy(playerSlot);
        }
        _playerSlotList.Clear();

        foreach (var player in _app.PlayerDict.Values)
        {
            Debug.Log($"!!! {TAG} -- Instantiating new connected player slot object");
            // Instantiate the player slot prefab
            var playerSlotInstance = Instantiate(_playerSlotPrefab);
            // Set the nickname of the slot
            string nickname = player.Nickname.Value.ToString();
            Debug.Log($"!!! {TAG} -- Set nickname to {player.Nickname.Value.ToString()}");
            // If this is the local player
            if (player.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                nickname += " (You)";
                Debug.Log($"!!! {TAG} -- Peer IS local client, updating nickname in connected player list");
            }
            else
            {
                Debug.Log($"!!! {TAG} -- Peer is NOT local client. Not adding suffix to item in player list");
            }

            playerSlotInstance.GetComponentInChildren<TMP_Text>().text = nickname;
            // Attach the slot instance to the list root
            playerSlotInstance.transform.SetParent(_root, false);

            _playerSlotList.Add(playerSlotInstance);
        }
    }
}
