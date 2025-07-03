using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;
    [SerializeField] LayerMask triggersLayer;
    [SerializeField] LayerMask ledgeLayer;
    [SerializeField] LayerMask waterLayer;

    public static GameLayers i { get; set; }

    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidObjectsLayer {
        get => solidObjectsLayer; }
    public LayerMask GrassLayer { get => grassLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask FovLayer { get => fovLayer; }
    public LayerMask PortalLayer { get => portalLayer; }
    public LayerMask LedgeLayer { get => ledgeLayer; }
    public LayerMask WaterLayer { get => waterLayer; }
    public LayerMask TriggerableLayers { get => grassLayer | fovLayer | portalLayer | triggersLayer | waterLayer; }



}
