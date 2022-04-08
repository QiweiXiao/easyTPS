using UnityEngine;

public static class PlayerInput
{
    //private static float lookAngle = 0f;
    //private static float tiltAngle = 0f;

    public static Vector3 GetMovementInput(Transform transform, Animator anim)
    {
        Vector3 moveVector;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveVector = h * transform.right + v * transform.forward;
        anim.SetFloat("speed", v);
        anim.SetFloat("direction", h);
        return moveVector.normalized;
    }

    public static Quaternion GetRotationInput(float mouseSensitivity = 1f, float minTiltAngle = -75f, float maxTiltAngle = 60f)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        float lookAngle = mouseX * mouseSensitivity;
        float tiltAngle = mouseY * mouseSensitivity;

        if (tiltAngle < minTiltAngle) tiltAngle = minTiltAngle;
        if (tiltAngle > maxTiltAngle) tiltAngle = maxTiltAngle;

        var roleRotation = Quaternion.Euler(tiltAngle, lookAngle, 0f);
        return roleRotation;
    }

    public static bool GetJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    public static bool GetFire1()
    {
        return Input.GetButtonDown("Fire1");
    }

    public static bool KeepFire1()
    {
        return Input.GetButton("Fire1");
    }

    public static bool UpFire1()
    {
        return Input.GetButtonUp("Fire1");
    }

    public static bool GetAlt()
    {
        return Input.GetKey(KeyCode.LeftAlt);
    }

    public static bool UpAlt()
    {
        return Input.GetKeyUp(KeyCode.LeftAlt);
    }

    public static bool GetQ()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    public static bool GetE()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public static bool GetShirt()
    {
        return Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
    }

    public static bool GetOne()
    {
        return Input.GetKeyDown(KeyCode.Alpha1);
    }

    public static bool GetTwo()
    {
        return Input.GetKeyDown(KeyCode.Alpha2);
    }

    public static bool GetThree()
    {
        return Input.GetKeyDown(KeyCode.Alpha3);
    }

    public static bool GetR()
    {
        return Input.GetKeyDown(KeyCode.R);
    }
}
