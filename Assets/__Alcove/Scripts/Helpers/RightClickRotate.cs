using UnityEngine;

public class RightClickRotate : SingletonMonoBehaviour<RightClickRotate>
{
    public float _TurnSpeed = 4.0f;
    private bool _IsRotating;
    public Vector3 _LastPosition;

    private void Awake()
    {
        if (!Application.isEditor)
            Destroy(this);

        gInstance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _LastPosition = Input.mousePosition;
            _IsRotating = true;
        }
        if (!Input.GetMouseButton(1)) _IsRotating = false;
        if (_IsRotating)
        {
            Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            DoRotation(pos - _LastPosition, _TurnSpeed);
            _LastPosition = pos;
        }
    }

    public void DoRotation(Vector3 pos, float speed)
    {
        transform.RotateAround(transform.position, transform.right, pos.y * speed);
        transform.RotateAround(transform.position, Vector3.up, -pos.x * speed);

        // after rotation
        Vector3 objRotation = transform.rotation.eulerAngles;
        if (objRotation.x > 180)
            objRotation.x = objRotation.x - 360;
        float clampedX = Mathf.Clamp(objRotation.x, -85f, 85f);
        float clampedZ = Mathf.Clamp(objRotation.x, 0f, 0.1f);
        transform.rotation = Quaternion.Euler(clampedX, objRotation.y, 0f);
    }
}
