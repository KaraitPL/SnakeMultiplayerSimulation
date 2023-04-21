using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLength : NetworkBehaviour
{
    [SerializeField] private GameObject tailPrefab;

    public NetworkVariable<ushort> length = new(6, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private List<GameObject> _tails;
    private Transform _lastTail;
    private Collider2D _collider2D;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _tails = new List<GameObject>();
        _lastTail = transform;
        _collider2D = GetComponent<Collider2D>();
        for(ushort i = 1; i < length.Value; i++)
            InstantiateTail();
        if (!IsServer) length.OnValueChanged += LengthChanged;
    }

    [ContextMenu("Add Length")]
    private void AddLength()
    {
        length.Value += 1;
        InstantiateTail();
    }

    private void LengthChanged(ushort previousValue, ushort newValue)
    {
        Debug.Log("LengthChanged Callback");
        InstantiateTail();
    }

    private void InstantiateTail()
    {
        GameObject tailGameObject = Instantiate(tailPrefab, transform.position, Quaternion.identity);
        tailGameObject.GetComponent<SpriteRenderer>().sortingOrder = -length.Value;
        if(tailGameObject.TryGetComponent(out Tail tail))
        {
            tail.networkedOwner = transform;
            tail.followTransform = _lastTail;
            _lastTail = tailGameObject.transform;
            Physics2D.IgnoreCollision(tailGameObject.GetComponent<Collider2D>(), _collider2D);
        }

        _tails.Add(tailGameObject);
    }
}
