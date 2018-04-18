using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject EyesPivot;
    private BlockBehaviour _inner;
    private Vector3 _newPosition;
	private bool _isSpawning = true;
    private bool _isMoving;
    // Use this for initialization
    void Awake()
    {
        _inner = GetComponentInChildren<BlockBehaviour>();
		StartCoroutine (SpawnBlock ());
    }


	IEnumerator SpawnBlock()
	{
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
		Vector3 _originalScale = transform.localScale;
		transform.localScale = Vector3.zero;
		Vector3 checkPointTransform = checkpoint.transform.position;
		Vector3 blockTransform = transform.position;
		checkPointTransform.y = 0;
		blockTransform.y = 0;
		float timeToWait = Mathf.FloorToInt(Vector3.Distance(checkPointTransform, blockTransform));
		yield return new WaitForSeconds(timeToWait * 0.2f);
		float t = 0;
		while (t < 1) {
			transform.localScale = _originalScale * t;
			t += Time.deltaTime * 5f;
			yield return null;
		}
		transform.localScale = _originalScale;
		_isSpawning = false;
	}
    // Update is called once per frame
    void Update()
    {
		if (_isSpawning)
			return;
        if (_isMoving)
        {
            if (Vector3.Distance(transform.position, _newPosition) <= 0.01f)
            {
                transform.position = _newPosition;
                _isMoving = false;
            }
            else
            {
                transform.Translate((_newPosition - transform.position).normalized * Time.deltaTime);

            }
        }
    }

    public void MoveEnemy(BlockFace face)
    {
        if (!_inner.FireRaycastFromFace(face))
        {
            _newPosition = transform.position + face.GetNormal();
            _isMoving = true;
        }
    }

    public void LookAtFaceDir(BlockFace face)
    {
        EyesPivot.transform.forward = face.GetNormal();
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    // we handle the box collision here.  we should have a separate component
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponentInChildren<EnemyAI>().EnemyDeath();
        }
    }
}
