using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collide");
        GameObject collider = collision.gameObject;
        if (collider.CompareTag("Player") == true)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
