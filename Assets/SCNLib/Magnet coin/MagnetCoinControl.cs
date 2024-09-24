using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagnetCoinControl : MonoBehaviour
{
    [Header("Session 1")]
    [Tooltip("Giai doan cac hat bung ra")]
    [SerializeField] MoveSession explodeSession;

    [Header("Session 2")]
    [Tooltip("Giai doan cac hat bi hut den ")]
    [SerializeField] MoveSession magnetSession;

    [Header("Unit")]
    [Tooltip("Tong so hat")]
    [SerializeField] int totalEmission;

    [Tooltip("Toc do san sinh hat / giay")]
    [SerializeField] float emissionsPerSecond;

    [Tooltip("Mau vat hat")]
    [SerializeField] GameObject prefabs;
    [Tooltip("Object hut hat sau khi bung ra")]
    [SerializeField] Transform magnetTrans;

    [Tooltip("Khoang cach hat ket thuc khi bi hut den gan Magnet")]
    [SerializeField] float minDistance = 1;

    [Tooltip("Hat co xoay quanh chinh no khong")]
    [SerializeField] bool isRotate;

    [Tooltip("Huong san sinh ra cac hat")]
    [SerializeField] float[] rangeX = new float[2] { -1, 1 };
    [SerializeField] float[] rangeY = new float[2] { -1, 1 };

    [Header("Other")]
    [SerializeField] Canvas parentCanvas;

	readonly List<GameObject> particlePool = new List<GameObject>();
    float caledDistance;

    public MoveSession ExplodeSession => explodeSession;
    public MoveSession MagnetSession => magnetSession;

    public Transform MagnetTrans { get => magnetTrans; set => magnetTrans = value; }
    public Canvas ParentCanvas { get => parentCanvas; set => parentCanvas = value; }

    const int deltaTimeCoeff = 500;

    public void Play(Transform startTrans, System.Action<GameObject> onSpawnUnit = null, System.Action onUnitFinish = null
        , System.Action onComplete = null)
    {
        caledDistance = minDistance * parentCanvas.transform.localScale.x * 100;
        var count = 0;

        var parPos = new Vector3(startTrans.position.x, startTrans.position.y, transform.position.z);

        _ = StartCoroutine(DelayCallMaster.RepeatCall(1f / emissionsPerSecond
            , totalEmission, time =>
            {
                var particleObj = GetParticleObj();
                particleObj.transform.position = parPos;
                onSpawnUnit?.Invoke(particleObj);

                _ = StartCoroutine(ParticleFlyExplode(particleObj, () =>
                {
                    _ = StartCoroutine(ParticleFlyMagnet(particleObj, () =>
                    {
                        count++;
                        if (count == totalEmission)
                        {
                            onComplete?.Invoke();
                        }
                        else
                        {
                            onUnitFinish?.Invoke();
                        }
                    }));
                }));
            }));
    }

    public void Play(System.Action<GameObject> onSpawnUnit = null, System.Action<GameObject> onUnitFinish = null
        , System.Action onComplete = null)
    {
        caledDistance = minDistance * parentCanvas.transform.localScale.x * 100;
        var count = 0;

        _ = StartCoroutine(DelayCallMaster.RepeatCall(1f / emissionsPerSecond
            , totalEmission, time =>
            {
                var particleObj = GetParticleObj();
                onSpawnUnit?.Invoke(particleObj);

                _ = StartCoroutine(ParticleFlyExplode(particleObj, () =>
                {
                    _ = StartCoroutine(ParticleFlyMagnet(particleObj, () => 
                    {
                        onUnitFinish?.Invoke(particleObj);

                        count++;
                        if (count == totalEmission)
                        {
                            onComplete?.Invoke();
                        }
                    }));
                }));
            }));
    }

    IEnumerator ParticleFlyExplode(GameObject particleObj, System.Action onDone)
    {
        Vector2 _emissionAngle = new Vector2(Random.Range(rangeX[0], rangeX[1])
            , Random.Range(rangeY[0], rangeY[1]));
        _emissionAngle.Normalize();

        // Session explode
        var s0_time = 0f;
        while (s0_time < explodeSession.Time)
        {
            s0_time += Time.deltaTime;

            var velocityVector = _emissionAngle * explodeSession.ValueGraph.Evaluate(
                Mathf.Clamp(s0_time / explodeSession.Time, 0, 1))
                * explodeSession.Speed * parentCanvas.transform.localScale.x * Time.deltaTime * deltaTimeCoeff;

            particleObj.transform.position += new Vector3(velocityVector.x, velocityVector.y, 0);

            yield return null;
        }

        onDone?.Invoke();
    }

    IEnumerator ParticleFlyMagnet(GameObject particleObj, System.Action onDone)
	{
		// Session magnet
		var s1_time = 0f;
		while (Vector2.Distance(particleObj.transform.position, magnetTrans.position)
			> caledDistance)
		{
			s1_time += Time.deltaTime;

            // Hut vao
            Vector2 gravityForce = (magnetTrans.position - particleObj.transform.position).normalized
                * magnetSession.ValueGraph.Evaluate(
                Mathf.Clamp(s1_time / magnetSession.Time, 0, 1)) * magnetSession.Speed
                * parentCanvas.transform.localScale.x * Time.deltaTime * deltaTimeCoeff;

            particleObj.transform.position += new Vector3(gravityForce.x, gravityForce.y, 0);

			if (isRotate)
			{
				particleObj.transform.Rotate(new Vector3(0, 0, Vector2.Angle(gravityForce, Vector2.up)));
			}
			else
			{
				particleObj.transform.localEulerAngles = Vector3.zero;
			}

			yield return null;
		}

		particleObj.SetActive(false);
		particlePool.Add(particleObj);

        onDone?.Invoke();
	}

    GameObject GetParticleObj()
    {
        if (particlePool.Count > 0)
        {
            var obj = particlePool[0];
            _ = particlePool.Remove(obj);

            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);

            return obj;
        }
        else
        {
            var instanObj = Instantiate(prefabs, transform);
            particlePool.Add(instanObj);

            return GetParticleObj();
        }
    }

    [System.Serializable]
	public class MoveSession
	{
		[SerializeField] float time;
		[SerializeField] float speed;
		[SerializeField] AnimationCurve valueGraph;

		public float Time { get => time; set => time = value; }
		public float Speed { get => speed; set => speed = value; }
		public AnimationCurve ValueGraph { get => valueGraph; set => valueGraph = value; }
	}
}
