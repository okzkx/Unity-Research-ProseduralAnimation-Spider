using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Spider : MonoBehaviour {
    public Transform[] Legs;
    public Vector2[] offset;
    public float FeetMinDistance = 0.01f;
    public float FeetMaxDistance;
    public float Up = 0.2f;
    public float Offset = 0.5f;

    //debug
    public Vector3[] posFeets;
    public Vector3[] poss;
    public bool[] IsMovingLeg;
    public float[] Timer;
    private void Awake() {
        poss = new Vector3[Legs.Length];
        for (int i = 0; i < Legs.Length; i++) {
            poss[i] = Legs[i].position;
        }

        posFeets = (Vector3[])poss.Clone();
        for (int i = 0; i < posFeets.Length; i++) {
            posFeets[i].x += Random.Range(-0.1f, 0.1f);
            posFeets[i].z += Random.Range(-0.1f, 0.1f);
        }
        IsMovingLeg = new bool[4];
        Timer = new float[4];
    }
    void Update() {

        float feetHigh = 0;
        foreach (var feet in posFeets) {
            feetHigh += feet.y;
        }
        feetHigh /= 4;
        Vector3 bodyPos = transform.position;
        bodyPos.y = feetHigh + Offset;

        transform.position = Vector3.Lerp(transform.position, bodyPos, 0.2f);

        Vector3[] localFeets = new Vector3[4];
        Vector3 avgLocalFeet = Vector3.zero;
        for (int i = 0; i < Legs.Length; i++) {
            localFeets[i] = transform.InverseTransformPoint(posFeets[i]);
            avgLocalFeet += localFeets[i];
        }
        avgLocalFeet /= 4;
        if (localFeets[0].y > avgLocalFeet.y && (localFeets[0].y - avgLocalFeet.y) > 0.1f) {
            transform.Rotate(Vector3.forward, Space.Self);
            transform.Rotate(Vector3.left, Space.Self);
        }
        if (localFeets[1].y > avgLocalFeet.y && (localFeets[1].y - avgLocalFeet.y) > 0.1f) {
            transform.Rotate(Vector3.back, Space.Self);
            transform.Rotate(Vector3.left, Space.Self);
        }
        if (localFeets[2].y > avgLocalFeet.y && (localFeets[2].y - avgLocalFeet.y) > 0.1f) {
            transform.Rotate(Vector3.forward, Space.Self);
            transform.Rotate(Vector3.right, Space.Self);
        }
        if (localFeets[3].y > avgLocalFeet.y && (localFeets[3].y - avgLocalFeet.y) > 0.1f) {
            transform.Rotate(Vector3.back, Space.Self);
            transform.Rotate(Vector3.right, Space.Self);
        }

        for (int i = 0; i < Legs.Length; i++) {
            Vector3 point = new Vector3(offset[i].x, 1, offset[i].y);
            var hitInfos = Physics.RaycastAll(transform.TransformPoint(point), transform.TransformDirection(Vector3.down));

            Vector3 highestPoint = Vector3.down;
            foreach (var hitInfo in hitInfos) {
                Spider spider = hitInfo.transform.GetComponentInParent<Spider>();
                if (spider == null || spider != this) {
                    if (hitInfo.point.y > highestPoint.y) {
                        highestPoint = hitInfo.point;
                    }
                }
            }
            poss[i] = highestPoint;
        }

        for (int i = 0; i < Legs.Length; i++) {
            if (IsMovingLeg[i]) {
                Timer[i] += Time.deltaTime * 2;
                Vector3 pos = Vector3.Lerp(posFeets[i], poss[i], Timer[i]);
                pos.y += Mathf.Sin(Mathf.PI * Timer[i]) * Up;
                Legs[i].position = pos;
                if (Timer[i] > 1) {
                    IsMovingLeg[i] = false;
                    posFeets[i] = poss[i];
                }
            } else {
                Legs[i].position = posFeets[i];

                if (Vector3.Distance(posFeets[i], poss[i]) > FeetMaxDistance) {
                    if (!IsMovingLeg[(i + 2) % Legs.Length] && !IsMovingLeg[(i + 3) % Legs.Length]) {
                        IsMovingLeg[i] = true;
                        Timer[i] = 0;
                        continue;
                    }
                }
            }
        }
        //Legs[i].position = hitInfo.point;

        //if (Physics.Raycast(transform.position + new Vector3(offset[i].x, 0, offset[i].y), Vector3.down, out var hitInfo)) {
        //    Debug.DrawLine(transform.position + new Vector3(offset[i].x, 0, offset[i].y), hitInfo.point);
        //}
        //for (int i = 0; i < Legs.Length; i++) {
        //    Legs[i].position = poss[i];
        //}
    }
}
