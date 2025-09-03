using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -20f;
    public float rotateSpeed = 720f; // deg/sec

    private CharacterController controller;
    private Vector3 velocity;
    private Transform cam;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    public void Move(Vector2 input)
    {
        if (cam == null) { controller.Move(Vector3.zero); return; }
        Vector3 f = cam.forward; f.y = 0; f.Normalize();
        Vector3 r = cam.right;   r.y = 0; r.Normalize();
        Vector3 dir = f * input.y + r * input.x;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, rotateSpeed * Time.deltaTime);
        }
        controller.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

    public void TickGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
