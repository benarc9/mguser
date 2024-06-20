using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Constants
{
    public enum AnchorType
    {
        Plane,
        Point,
        Object,
        Image
    }
    
    public class UploadCare
    {
        public static readonly String PUB_KEY = "dc63b7727e3a912ad70f";
        public static readonly String PRIV_KEY = "1a3174db920e1e414d12";
            }

    public enum Direction
    {
        N, NE, E, SE, S, SW, W, NW
    }

    public enum LerpDirection
    {
        Forward, Return
    }

    public enum Tag
    {
        AnchorList,
        Settings,
        Debug,
        PlacementIndicator,
        SessionOrigin,
        Session,
        AnchorListContent,
        DebugContent,
        SaveWorldButton,
        LoadWorldButton,
        ResetWorldButton,
        TrackingModeSelector,
        Placer,
        Indicator,
        DataManager
    }
}
