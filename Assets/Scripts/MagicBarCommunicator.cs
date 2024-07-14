using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBarCommunicator : MonoBehaviour
{
    [SerializeField] private ActionCommandController magicActionCommunicator;
    // Start is called before the first frame update
    public void MagicBarCloser()
    {
        magicActionCommunicator.magicAction.MagicBarCloser();
    }

    public void MagicBarFinisher()
    {
        magicActionCommunicator.magicAction.MagicBarFinisher();
    }
}
