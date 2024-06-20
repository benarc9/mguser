// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using UnityEngine;
using Unity.Netcode;
using Constants;

public class CanvasManager : MonoBehaviour
{
    private static readonly string TAG = "CanvasManager";

    [SerializeField] private GameObject _startPanel;

    [SerializeField] private GameObject _hostPanel;

    [SerializeField] private GameObject _clientPanel;

    public PanelStatus panelStatus = PanelStatus.Start;

    private void Start()
    {
        UpdateCurrentPanel();
    }

    private void Update()
    {
        UpdateCurrentPanel();
    }

    private void UpdateCurrentPanel()
    {
        PanelStatus newStatus = PanelStatus.New;
        if (NetworkManager.Singleton.IsServer)
        {
            _startPanel.SetActive(false);
            _hostPanel.SetActive(true);
            _clientPanel.SetActive(false);
            newStatus = PanelStatus.Host;
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            _startPanel.SetActive(false);
            _hostPanel.SetActive(false);
            _clientPanel.SetActive(true);
            newStatus = PanelStatus.Client;
        }
        else
        {
            _startPanel.SetActive(true);
            _hostPanel.SetActive(false);
            _clientPanel.SetActive(false);
            newStatus = PanelStatus.Start;
        }

        if (panelStatus != newStatus && newStatus != PanelStatus.New)
        {
            if (newStatus == PanelStatus.Client)
            {
                Debug.Log($"!!! {TAG} -- Peer IS client, setting client panel active");
            }
            else if (newStatus == PanelStatus.Start)
            {
                Debug.Log($"!!! {TAG} -- Peer is NOT server OR client, setting start panel active");
            }
            else
            {
                Debug.Log($"!!! {TAG} -- Peer IS server, setting host panel active");
            }

            panelStatus = newStatus;
        }
    }

    public enum PanelStatus {Start, Host, Client, New}
}
