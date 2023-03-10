using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Test : MonoBehaviour
{
    [SerializeField, Tooltip("KeyPointのタグ")] string _keyPointTag;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(_keyPointTag))
        {
            Debug.Log("当たった");
        }
    }
}
