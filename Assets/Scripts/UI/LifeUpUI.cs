using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeUpUI : MonoBehaviour
{
    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }
}
