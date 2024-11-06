using UnityEngine;

public class InputControlScript : MonoBehaviour
{
    private Vector3 pos;
    
    // Start is called before the first frame update
    void Start()
    {
        pos = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            pos.z += 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            pos.z -= 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            pos.x -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            pos.x += 1;
        }
        GetComponent<Transform>().SetPositionAndRotation(pos, Quaternion.identity);
    }
}
