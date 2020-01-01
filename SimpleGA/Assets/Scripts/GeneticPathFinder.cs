using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPathFinder : MonoBehaviour {
    public float creatureSpeed;
    public float rotationSpeed;
    public float pathMultiplier;
    bool hasCrashed = false;
    int pathIndex = 0;
    public DNA dna;
    bool hasBeenIntialized = false;
    Vector2 target;
    Vector2 nextPoint;
    public bool hasFinished = false;
    Quaternion targetRotation;
    LineRenderer lr;
    List<Vector2> traveledPath = new List<Vector2>();

    public void InitCreature(DNA newDNA, Vector2 _target) {
        lr = GetComponent<LineRenderer>();
        traveledPath.Add(transform.position);
        dna = newDNA;
        target = _target;
        nextPoint = transform.position;
        traveledPath.Add(nextPoint);
        hasBeenIntialized = true;
    }

    private void Update() {
        if (hasBeenIntialized && !hasFinished) {
            if (pathIndex == dna.genes.Count || Vector2.Distance(transform.position, target) < 0.01f) {
                hasFinished = true;
            }
            if ((Vector2)transform.position == nextPoint) {
                nextPoint = (Vector2)transform.position + dna.genes[pathIndex];
                traveledPath.Add(nextPoint);
                targetRotation = LookAt2D(nextPoint);
                pathIndex++;
            } else {
                transform.position = Vector2.MoveTowards(transform.position, nextPoint, creatureSpeed * Time.deltaTime);
            }
            if(transform.rotation != targetRotation) {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            RenderLine();
        }
    }

    public float Fitness {
        get {
            float dist = Vector2.Distance(transform.position, target);
            if(dist == 0) {
                dist = 0.0001f;
            }
            return (60/dist) * (hasCrashed ? 0.7f : 1f);
        }
    }

    public void RenderLine() {
        List<Vector3> linePoints = new List<Vector3>();
        if(traveledPath.Count > 2) {
            for (int i = 0; i < traveledPath.Count - 1; i++) {
                linePoints.Add(traveledPath[i]);
            }
            linePoints.Add(transform.position);
        } else {
            linePoints.Add(traveledPath[0]);
            linePoints.Add(transform.position);
        }
        lr.positionCount = linePoints.Count;
        lr.SetPositions(linePoints.ToArray());
    }

    public Quaternion LookAt2D(Vector2 target, float angleOffset = -90) {
        Vector2 fromTo = (target - (Vector2)transform.position).normalized;
        float zRotation = Mathf.Atan2(fromTo.y, fromTo.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, zRotation + angleOffset);
    }

    private void OnTriggerEnter2D(Collider2D c) {
        if(c.gameObject.layer == 8) {
            hasFinished = true;
            hasCrashed = true;
        }
    }
}
