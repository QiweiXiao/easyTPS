using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCamera : MonoBehaviour
{
    private const float MIN_ROTATIONDAMP_SPEED = 0f;
    private const float MAX_ROTATIONDAMP_SPEED = 1f;
    private const float MIN_CATCHDAMP_SPEED = 0f;
    private const float MAX_CATCHDAMP_SPEED = 1f;

    public Transform target = null;
    public Vector3 posOffset = new Vector3(0.5f, 3f, -2f);

    [SerializeField]
    [Range(MIN_CATCHDAMP_SPEED, MAX_CATCHDAMP_SPEED)]
    private float catchDampSpeed = MIN_CATCHDAMP_SPEED;

    [SerializeField]
    [Range(MIN_ROTATIONDAMP_SPEED, MAX_ROTATIONDAMP_SPEED)]
    private float rotationDampSpeed = 0.3f;

    public float ySpeed = 3f;                   //绕y轴旋转速度
    public float xSpeed = 3f;                   //绕x轴旋转速度
    public float yMaxLimit = 50.0f;             //相机仰角上限
    public float yMinLimit = -60.0f;            //相机仰角下限

    private float x;
    private float y;
    private float targetX;
    private float targetY;
    private Vector3 position;
    private float xVelocity;
    private float yVelocity;
    private Vector3 cameraVelocity;

    private void Start()
    {
        var angles = transform.eulerAngles;
        targetX = x = angles.x;
        targetY = y = ClampAngle(angles.y, yMinLimit, yMaxLimit);
        //transform.rotation = target.rotation;
        //curXAngle = transform.eulerAngles.x;
        //curYAngle = transform.eulerAngles.y;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;
        targetX += Input.GetAxis("Mouse X") * xSpeed;
        targetY -= Input.GetAxis("Mouse Y") * ySpeed;
        targetY = ClampAngle(targetY, yMinLimit, yMaxLimit);
        x = Mathf.SmoothDampAngle(x, targetX, ref xVelocity, rotationDampSpeed);
        y = Mathf.SmoothDampAngle(y, targetY, ref yVelocity, rotationDampSpeed);
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 followpos = transform.rotation * Vector3.forward * posOffset.z + target.right * posOffset.x + target.up * posOffset.y + target.forward * posOffset.z + target.position;
        position = Vector3.SmoothDamp(position, followpos, ref cameraVelocity, catchDampSpeed);
        transform.rotation = rotation;
        transform.position = position;
        target.rotation = Quaternion.Euler(0, x, 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
