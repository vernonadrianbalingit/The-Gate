using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{   
    
    private GameObject grabbed = null;
    private GameObject lockedStand = null;


    private Camera GetCurrentCamera()
    {
        return Camera.current != null ? Camera.current : Camera.main;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = CastRay();

            if (hit.collider != null && grabbed == null && hit.collider.gameObject.CompareTag("Turret"))
            {
                Debug.Log("Grabbed Turret");
                grabbed = hit.collider.gameObject;
                if (GetGrandestParent(GetClosestStand(grabbed)) == GetGrandestParent(GetCurrentCamera().gameObject))
                {
                    Cursor.visible = false;
                    StandInUse standInUse = GetClosestStand(grabbed).GetComponent<StandInUse>();

                    if (standInUse != null)
                    {
                        standInUse.SetInUse(false);
                    }
                }
                else
                {
                    grabbed = null;
                }
            }
            else if (grabbed != null)
            {
                // Place on the closest stand
                GameObject closestStand = GetClosestStand(grabbed);

                if (CheckValidPlacement(closestStand))
                {
                    grabbed.transform.position = closestStand.transform.position;
                    grabbed.transform.position = new Vector3(grabbed.transform.position.x, 2.88f, grabbed.transform.position.z);
                    grabbed = null;
                    Cursor.visible = true;
                }
            }
        }
        if (grabbed != null)
        {   
            GameObject closestStand = GetClosestStand(grabbed);
            Vector3 ScreenMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, GetCurrentCamera().WorldToScreenPoint(grabbed.transform.position).z);   
            Vector3 worldMousePos = GetCurrentCamera().ScreenToWorldPoint(ScreenMousePos);
            grabbed.transform.position = new Vector3(worldMousePos.x, 3.88f, worldMousePos.z);
        }
    }

    private RaycastHit CastRay()
    {
        Camera cam = GetCurrentCamera();
        Vector3 ScreenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.farClipPlane);
        Vector3 ScreenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane);
        Vector3 worldMousePosFar = cam.ScreenToWorldPoint(ScreenMousePosFar);
        Vector3 worldMousePosNear = cam.ScreenToWorldPoint(ScreenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);
        return hit;
    }

    private GameObject GetClosestStand(GameObject turret)
    {
        GameObject[] stands = GameObject.FindGameObjectsWithTag("Stand");
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 turretPos = turret.transform.position;

        foreach (GameObject stand in stands)
        {
            float distance = Vector3.Distance(stand.transform.position, turretPos);
            if (distance < minDistance)
            {
                closest = stand;
                minDistance = distance;
            }
        }

        return closest;
    }

    private bool CheckValidPlacement(GameObject stand)
    {
        GameObject StandSection = GetGrandestParent(stand);
        GameObject CameraSection = GetGrandestParent(GetCurrentCamera().gameObject);

        StandInUse standInUse = stand.GetComponent<StandInUse>();
        
        if (standInUse != null && standInUse.IsInUse())
        {
            return false;
        }
        else
        {
            if (standInUse != null)
            {
                standInUse.SetInUse(true);
            }
        }

        return StandSection == CameraSection;

    }

    private GameObject GetGrandestParent(GameObject obj)
    {
        Transform current = obj.transform;
        while (current.parent != null)
        {
            current = current.parent;
        }
        return current.gameObject;
    }

    /// <summary>
    /// Spawns a prefab at the mouse cursor position and allows it to be grabbed
    /// </summary>
    /// <param name="prefab">The prefab to spawn</param>
    public void SpawnPrefabAtCursor(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Grabber: Cannot spawn null prefab");
            return;
        }

        // Get the mouse position in world space
        Camera cam = GetCurrentCamera();
        Vector3 ScreenMousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.nearClipPlane + 10f);
        Vector3 worldMousePos = cam.ScreenToWorldPoint(ScreenMousePos);

        // Spawn the prefab at the cursor position
        GameObject spawnedObject = Instantiate(prefab, new Vector3(worldMousePos.x, 3.88f, worldMousePos.z), Quaternion.identity);

        // Automatically grab the spawned object
        grabbed = spawnedObject;
        Cursor.visible = false;

        Debug.Log($"Spawned {prefab.name} at cursor position");
    }
}
