using UnityEngine;

public class RDRotatingVoxel : MonoBehaviour
{
	public float speed;

	private float speedMult = 1f;

	public float angle;

	public RotatingVoxelMode voxelMode;

	public RDRoom room;

	public Camera kaleidoCam;

	public VideoBloom bloomComp;

	public Transform pivot;

	public GameObject heartWhole;

	public GameObject heartHalf;

	public GameObject baseball;

	public Transform baseballPivot;

	public GameObject zenGarden;

	public Transform zenPivot;

	public GameObject cubes;

	public GameObject fallingBaseballs;

	private void Update()
	{
		SetBgColor(room.camera.backgroundColor);
		angle += Time.deltaTime * speed * speedMult;
		if (voxelMode == RotatingVoxelMode.ZenGarden)
		{
			zenPivot.localRotation = Quaternion.Euler(0f, angle, 0f);
		}
		else if (voxelMode == RotatingVoxelMode.Baseball)
		{
			baseballPivot.localRotation = Quaternion.Euler(0f, angle, 0f);
		}
		else
		{
			pivot.localRotation = Quaternion.Euler(0f, angle, 0f);
		}
	}

	private void LateUpdate()
	{
		kaleidoCam.enabled = room.camera.enabled;
	}

	public void SetSpeed(float speed)
	{
		this.speed = speed;
	}

	public void SetVoxelMode(RotatingVoxelMode newMode)
	{
		bool flag = voxelMode == RotatingVoxelMode.Heart || voxelMode == RotatingVoxelMode.BrokenHeart;
		bool num = newMode == RotatingVoxelMode.Heart || newMode == RotatingVoxelMode.BrokenHeart;
		ResetRotation();
		speedMult = ((voxelMode == RotatingVoxelMode.ZenGarden) ? 0.5f : ((voxelMode == RotatingVoxelMode.Baseball) ? 2f : 1f));
		if (num)
		{
			if (!flag)
			{
				bloomComp.enabled = true;
			}
		}
		else
		{
			bloomComp.enabled = false;
		}
		voxelMode = newMode;
		heartWhole.SetActive(voxelMode == RotatingVoxelMode.Heart);
		heartHalf.SetActive(voxelMode == RotatingVoxelMode.BrokenHeart);
		zenGarden.SetActive(voxelMode == RotatingVoxelMode.ZenGarden);
		baseball.SetActive(voxelMode == RotatingVoxelMode.Baseball);
	}

	public void SetBgColor(Color color)
	{
		kaleidoCam.backgroundColor = color;
	}

	public void ResetRotation()
	{
		pivot.localRotation = Quaternion.identity;
		zenPivot.localRotation = Quaternion.identity;
		baseballPivot.localRotation = Quaternion.identity;
		angle = 0f;
	}

	public void SetCubeMode(bool enableCubes)
	{
		cubes.SetActive(enableCubes);
	}

	public void SetFallingBaseballs(bool enableBaseballs)
	{
		fallingBaseballs.SetActive(enableBaseballs);
	}
}
