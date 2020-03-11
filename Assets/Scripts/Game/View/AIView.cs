using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIView : MonoBehaviour
{
    public Transform TransSelf;

    public float Speed = 5;
    public Transform[] Waypoints;

    private int _index = 0;


    private float _falloff = 0.5f;

    private void Start()
    {
        TransSelf.LookAt(Waypoints[_index + 1].position);
        _index++;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Vector3.Distance(TransSelf.position, Waypoints[_index].position) <= Speed * Time.deltaTime) 
        //{
        //    GoToNextNode();
        //}

        if (Vector3.Distance(TransSelf.position, Waypoints[_index].position) <= _falloff)
        {
            GoToNextNode();
        }


        Vector3 v = Waypoints[_index].position - TransSelf.position;
        TransSelf.rotation = Quaternion.Lerp(TransSelf.rotation, Quaternion.LookRotation(v, Vector3.up), Time.deltaTime * 3.5f);

        TransSelf.Translate(Vector3.forward * Speed * Time.deltaTime, Space.Self);
    }
    void GoToNextNode()
    {
        _index++;
        if (_index < Waypoints.Length)
        {
            //TransSelf.LookAt(Waypoints[_index].position);
        }
        else
        {
            _index = 0;
        }

    }

    private void OnDrawGizmos()
    {
        if (Waypoints != null)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(Waypoints[i].position, Vector3.one * 0.5f);

                if (i < Waypoints.Length - 1)
                {
                    Gizmos.DrawLine(Waypoints[i].position, Waypoints[i + 1].position);

                }
            }
        }
    }
}
