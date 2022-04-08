using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;
    public GameObject tpCamera;
    public Vector3 pos = new Vector3(-3.8f, -4.3f, 31.7f);
    private GameObject tpCameraClone;
    private GameObject playerClone;

    private void Awake()
    {
        playerClone = Instantiate(player, pos, Quaternion.identity);
        tpCameraClone = Instantiate(tpCamera, pos, Quaternion.identity);
        playerClone.GetComponent<PlayerRobot>().tpsCamera = tpCameraClone.GetComponent<Camera>();
        tpCameraClone.GetComponent<TPCamera>().target = playerClone.transform;
    }

    public GameObject GetCamera()
    {
        return tpCameraClone;
    }
}
