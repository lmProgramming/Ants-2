using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AnimationSpawner : MonoBehaviour
{
    public static AnimationSpawner Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private GameObject popAnimation;
    public static GameObject PopAnimation { get { return Instance.popAnimation; } }

    public static void AddPopAnimation(GameObject objectToAddAnimation)
    {
        Instantiate(PopAnimation, objectToAddAnimation.transform);
    }
}
