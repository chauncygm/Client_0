using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMove : MonoBehaviour
{
    private Transform start;

    public Transform end;
    // Start is called before the first frame update
    void Start()
    {
        start = GameObject.Find("OneCube").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(start.position, end.position, Time.deltaTime);
    }
}
