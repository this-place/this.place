using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingScript : MonoBehaviour
{

    private void Awake()
    {
        StartCoroutine(SpawnLight());
    }

    IEnumerator SpawnLight()
    {
        if (gameObject.scene.name == "Tutorial" || gameObject.scene.name == "Main")
        {
            yield break;
        }

        Light lightObject = GetComponent<Light>();
        float originalIntensity = lightObject.intensity;
		lightObject.intensity = 0.0f;

        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("CheckPoint");
        GameObject checkpoint = gameObject;

        foreach (GameObject cp in checkpoints)
        {
            if (cp.gameObject.scene == gameObject.scene)
            {
                checkpoint = cp;
                break;
            }
        }

        Vector3 checkPointTransform = checkpoint.transform.position;
        Vector3 lightObjectTransform = transform.position;
        checkPointTransform.y = 0;
        lightObjectTransform.y = 0;
        float timeToWait = Mathf.FloorToInt(Vector3.Distance(checkPointTransform, lightObjectTransform));
        yield return new WaitForSeconds(timeToWait * 0.2f);
        float t = 0;
        while (t < 1)
        {
            t += 5 * Time.deltaTime;

            yield return null;
        }
        lightObject.intensity = originalIntensity;
    }

    public void DeSpawn(Vector3 position)
    {
        StartCoroutine(DeSpawnLight(position));
    }

    IEnumerator DeSpawnLight(Vector3 position)
    {
        float timeToWait = Mathf.FloorToInt(Vector3.Distance(position, transform.position));
        yield return new WaitForSeconds(timeToWait * 0.2f);

        float t = 1;
        while (t > 0)
        {
            t -= 5 * Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
