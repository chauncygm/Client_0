using UnityEngine;

namespace GameMain.Scripts
{
    public class SlowMove : MonoBehaviour
    {
        // 目标位置
        public Transform target;

        private Transform _self;
        private Vector3 _start;
        private Vector3 _end;


        public int second = 1;
    
        private void Start()
        {
            _self = this.GetComponent<Transform>();
            _start = _self.position;
            _end = target.GetComponent<Transform>().position;
        }

        private void Update()
        {
            if (!(Time.time <= second)) return;
            var pos = Vector3.Lerp(_start, _end, Time.time / second);
            _self.position = pos;
        }
    }
}
