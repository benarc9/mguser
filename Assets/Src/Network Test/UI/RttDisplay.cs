// SPDX-FileCopyrightText: Copyright 2023 Holo Interactive <dev@holoi.com>
//
// SPDX-FileContributor: Yuchen Zhang <yuchen@holoi.com>
//
// SPDX-License-Identifier: MIT

using UnityEngine;
using TMPro;

public class RttDisplay : MonoBehaviour
{
    private static readonly string TAG = "RttDisplay";
    
    [SerializeField] private RttCounter _rttCounter;

    public TMP_Text _rttText;

    private void Start()
    {
        _rttText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        _rttText.text = $"{_rttCounter.Rtt:F4} ms";
    }
}
