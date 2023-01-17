using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Test : MonoBehaviour
{
    [SerializeField, Tooltip("KeyPoint�̃^�O")] string _keyPointTag;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(_keyPointTag))
        {
            Debug.Log("��������");
        }
    }
}
