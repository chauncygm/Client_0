using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMove : MonoBehaviour
{
    // 目标位置
    public Transform target;

    private Transform self;
    private Vector3 start;
    private Vector3 end;


    public int second = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        self = this.GetComponent<Transform>();
        start = self.position;
        end = target.GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= second)
        {
            var pos = Vector3.Lerp(start, end, Time.time / second);
            self.position = pos;
        }
    }
}
