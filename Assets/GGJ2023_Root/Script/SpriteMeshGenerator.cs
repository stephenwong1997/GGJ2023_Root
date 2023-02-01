using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteMeshGenerator : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] MeshCollider _meshCollider;

    private void Awake()
    {
        Mesh mesh = SpriteToMesh(_spriteRenderer.sprite);
        _meshCollider.sharedMesh = mesh;
    }

    Mesh SpriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i);
        mesh.uv = sprite.uv;
        mesh.triangles = Array.ConvertAll(sprite.triangles, i => (int)i);

        return mesh;
    }
}