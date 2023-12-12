using UnityEngine;

public class InputControlScript : MonoBehaviour
{
    private Vector3 pos;
    
    // Start is called before the first frame update
    void Start()
    {
        pos = this.GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W was press.");
            pos.z += 1;
            var component = this.GetComponent<Transform>();
            component.SetPositionAndRotation(pos, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S was press.");
            pos.z -= 1;
            var component = this.GetComponent<Transform>();
            component.SetPositionAndRotation(pos, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A was press.");
            pos.x -= 1;
            var component = this.GetComponent<Transform>();
            component.SetPositionAndRotation(pos, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("D was press.");
            pos.x += 1;
            var component = this.GetComponent<Transform>();
            component.SetPositionAndRotation(pos, Quaternion.identity);
        }

        // GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
    }
}
