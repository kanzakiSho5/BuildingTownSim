using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CreateType
{
    Selector,
    Move,
    CreateLoad,
    CreateBuilding
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public CreateType CullentCreateType;

    private void Awake()
    {
        if (!Instance) Instance = this;
    }

    public void ChangeCreateType(CreateType createType)
    {
        CullentCreateType = createType;
    }
}
