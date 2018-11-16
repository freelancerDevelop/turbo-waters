using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public struct SphereCollisionResults : IComparer<Collider>
{
    public int count;
    public Collider[] colliders;
    private Vector3 position;

    public int Compare(Collider lhs, Collider rhs)
    {
        float lhsDistance = (this.position - lhs.transform.position).sqrMagnitude;
        float rhsDistance = (this.position - rhs.transform.position).sqrMagnitude;

        return (int) ((lhsDistance - rhsDistance) * 1000.0f);
    }

    public void SortByDistanceToOriginsAscending(Vector3 position)
    {
        this.position = position;

        Array.Sort(this.colliders, 0, this.count, this);
    }
}

public class SphereCollisionTest
{
    private static Collider[] sphereCollisionResults = new Collider[1000];

    public static SphereCollisionResults OverlapSphereNonAlloc(Vector3 position, float radius, int layerMask)
    {
        SphereCollisionResults results = new SphereCollisionResults();

        results.count = Physics.OverlapSphereNonAlloc(position, radius, sphereCollisionResults, layerMask);
        results.colliders = sphereCollisionResults;

        return results;
    }
}
