using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public float distance = 10.0f;
    public float height = 5.0f;
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
    public float distanceDamping = 5.0f;
    public Transform target;
    public bool Look { get; set; }
    public float Distance { get; set; }
    private float tempDistance { get; set; }

    private void LateUpdate()
    {
        // Early out if we don't have a target
        if (!target)
        {
            return;
        }
        if (Distance == 0)
            Distance = tempDistance = distance;

        // Calculate the current rotation angles
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
        tempDistance = Mathf.MoveTowards(tempDistance, Distance, distanceDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        var pos = transform.position;
        pos = target.position - currentRotation * Vector3.forward * tempDistance;
        pos.y = currentHeight;
        transform.position = pos;
    }
}