using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI obj")] [SerializeField] private Button Move;

    private void ResetClick()
    {
        Move.enabled = false;
    }

    public void OnMoveButtom(bool active)
    {
        if(active)
            GameManager.Instance.CullentCreateType = CreateType.Move;
    }

    public void OnLoadButton(bool active)
    {
        if(active)
            GameManager.Instance.CullentCreateType = CreateType.CreateLoad;
    }
}
