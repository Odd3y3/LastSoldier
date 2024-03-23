using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArm : MonoBehaviour
{
    public Transform cam;
    public Vector3 zoomDisablePos = new Vector3(0, 0, -3.0f);
    public Vector3 zoomEnablePos = new Vector3(0.5f, -0.1f, -1.3f);

    Coroutine camMoveCoroutine = null;

    private void Awake()
    {
        if (cam == null)
            cam = transform.GetChild(0);

        Zoom(false);
    }

    public void Zoom(bool isZoom)
    {
        if (isZoom)
        {
            //cam.transform.localPosition = zoomEnablePos;
            CamMoveTo(zoomEnablePos);
        }
        else
        {
            //cam.transform.localPosition = zoomDisablePos;
            CamMoveTo(zoomDisablePos);
        }
    }

    private void CamMoveTo(Vector3 pos)
    {
        if(camMoveCoroutine != null) StopCoroutine(camMoveCoroutine);

        camMoveCoroutine = StartCoroutine(CamMoveToCo(pos));
    }
    IEnumerator CamMoveToCo(Vector3 pos)
    {
        var returnValue = new WaitForFixedUpdate();
        while (!Vector3.Equals(cam.transform.localPosition, pos))
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, pos, 0.2f);
            yield return returnValue;
        }
    }
}
