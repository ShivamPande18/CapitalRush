using UnityEngine;

public class CameraSrc : MonoBehaviour
{
    int side = 2;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,new Vector3(((-0.5f)+(side+.5f))/2f, ((-0.5f) + (side + .5f)) / 2f, transform.position.z),10f*Time.fixedDeltaTime);
    }

    public void OnSideChange(int _side)
    {
        side = _side;
    }
}
