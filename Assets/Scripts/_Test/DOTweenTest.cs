using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DOTweenTest : MonoBehaviour
{
	public Transform tweenTrans;
	public Transform[] pathTrans;
	public float duration = 2f;
	Tweener tweener;
	Sequence sequence;

	int curIndex = 0;

	Vector3 initPos;
	Vector3 endPos;
	Vector3 toScale = new Vector3 (1f, 2f, 1f);

	void Start ()
	{
		Vector3[] path = new Vector3[pathTrans.Length];
		for (int i = 0; i < pathTrans.Length; i++)
			path [i] = pathTrans [i].position;
		// path test
//		tweener = tweenTrans.DOPath (path, duration, PathType.CatmullRom).SetLoops (-1).SetOptions (true);

		// patrol test
//		tweener = tweenTrans.DOMove (GetRandomPos (), duration).SetAutoKill (false).OnComplete (
//			() => {
//				tweener.ChangeStartValue (tweenTrans.position);
//				tweener.ChangeEndValue (GetRandomPos ());
//				tweener.Play ();
//			}
//		);

		initPos = tweenTrans.position;
		endPos = initPos + tweenTrans.TransformDirection (Vector3.up);

//		Tweener patrolTween = tweenTrans.DOMove (GetRandomPos (), 1f).SetAutoKill (false);
//		patrolTween.OnComplete (() => {
//			patrolTween.ChangeStartValue (tweenTrans.position);
//			patrolTween.ChangeEndValue (GetRandomPos ());
//			patrolTween.Play ();
//		});
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Space))
		{
			tweener.Pause ();
			tweenTrans.DOLocalMoveY (2f, 0.2f).SetRelative ();
			tweenTrans.DOLocalMoveY (-2f, 0.2f).SetRelative ().SetDelay (0.2f).OnComplete (() => {
				tweener.Play ();
			});
		}

		if (Input.GetKeyDown (KeyCode.Q))
		{
			if (sequence != null)
				sequence.Kill ();
			
			sequence = DOTween.Sequence ();
//			sequence.Append (tweenTrans.DOScale (endPos, 0.2f));
//			sequence.Insert (0.2f, tweenTrans.DOMove (initPos, 1f));
			sequence.Append (tweenTrans.DOScale (toScale, 0.2f));
			sequence.Append (tweenTrans.DOScale (Vector3.one, 1f));
		}
	}

	Vector3 GetRandomPos ()
	{
		curIndex += Random.Range (1, pathTrans.Length);
		curIndex %= pathTrans.Length;
		return pathTrans [curIndex].position;
	}
}
