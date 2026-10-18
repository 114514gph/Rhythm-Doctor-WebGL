using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletShooter : RDBase
{
	public GameObject bulletPrefab;

	public Text timer;

	public GameObject heart;

	public static float bulletSpeed = 300f;

	private const int MaxBullets = 10;

	private const float Rate = 1f;

	private const float offsetLimit = 10f;

	private List<GameObject> mBullets = new List<GameObject>();

	private float mInitialHeartPosX;

	private float mOffsetHeartPosX;

	private double mNextBeat;

	private float mHeartExtentsX;

	private double mAudioPos;

	private double mPreviousAudioPos;

	private double mLastReportedPlayheadPosition;

	private void Start()
	{
		QualitySettings.vSyncCount = 0;
		bulletPrefab.SetActive(value: false);
		mInitialHeartPosX = heart.transform.position.x;
		mHeartExtentsX = heart.GetComponent<tk2dSprite>().GetBounds().extents.x;
		base.transform.position = new Vector3(mInitialHeartPosX - mHeartExtentsX - 200f, heart.transform.position.y, 0f);
		mPreviousAudioPos = Time.realtimeSinceStartup;
		mLastReportedPlayheadPosition = 0.0;
		base.conductor.SetBPM(88f);
		base.conductor.PlaySong("sndTutorial", LevelBase.musicGroup);
		for (int i = 5; i < 50; i++)
		{
			base.conductor.PlayBeat("sndKick", base.conductor.crotchet * (float)i);
		}
		mNextBeat = base.conductor.startOfSong + (double)base.conductor.crotchet;
	}

	private void LateUpdate()
	{
		mAudioPos += (double)Time.realtimeSinceStartup - mPreviousAudioPos;
		mPreviousAudioPos = Time.realtimeSinceStartup;
		timer.text = "Time: " + mAudioPos.ToString("0.00");
		if (base.conductor.audioPos != mLastReportedPlayheadPosition)
		{
			mAudioPos = (mAudioPos + base.conductor.audioPos) / 2.0;
			mLastReportedPlayheadPosition = base.conductor.audioPos;
		}
		if (mAudioPos > mNextBeat)
		{
			mNextBeat += base.conductor.crotchet;
			ShootBullet(mNextBeat);
		}
		for (int num = mBullets.Count - 1; num >= 0; num--)
		{
			Transform transform = mBullets[num].transform;
			if (transform.position.x + transform.localScale.x * 0.5f > mInitialHeartPosX + mOffsetHeartPosX - mHeartExtentsX)
			{
				SpriteFX.Flash(heart.GetComponent<tk2dSprite>());
				mBullets[num].GetComponent<Bullet>().Remove();
				mBullets.RemoveAt(num);
			}
		}
	}

	private void ShootBullet(double beatTime)
	{
		GameObject gameObject = Object.Instantiate(bulletPrefab);
		gameObject.transform.position = base.transform.position;
		gameObject.SetActive(value: true);
		Bullet component = gameObject.GetComponent<Bullet>();
		component.birthTime = beatTime;
		component.speed = bulletSpeed;
		component.InitialHeartLeftBorder = new Vector3(mInitialHeartPosX - mHeartExtentsX, heart.transform.position.y, 0f);
		mBullets.Add(gameObject);
	}

	private void MoveHeart()
	{
		mOffsetHeartPosX = Mathf.Clamp(mOffsetHeartPosX, -10f, 10f);
		Vector3 position = heart.transform.position;
		position.x = mInitialHeartPosX + mOffsetHeartPosX;
		heart.transform.position = position;
		MonoBehaviour.print(GetTimeOffset());
	}

	public void LeftButtonPressed()
	{
		mOffsetHeartPosX--;
		MoveHeart();
	}

	public void RightButtonPressed()
	{
		mOffsetHeartPosX++;
		MoveHeart();
	}

	public void SaveChanges()
	{
		MonoBehaviour.print("TODO Implement OK Button");
	}

	public float GetTimeOffset()
	{
		return mOffsetHeartPosX / bulletSpeed;
	}
}
