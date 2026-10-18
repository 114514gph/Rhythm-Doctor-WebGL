using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitStripManager : RDBase
{
	[NonSerialized]
	public RDHitStrip[] hitstrips = new RDHitStrip[16];

	public Dictionary<RDPlayer, Row[]> lastActiveRowInRoomForPlayer = new Dictionary<RDPlayer, Row[]>();

	private const float maxDistanceToMerge = 40f;

	private const float maxTimeToMerge = 0.025f;

	public void ShowHitstrip(RDPlayer player, Beat heldBeat = null)
	{
		if (base.game.currentLevel.noHitStrips)
		{
			return;
		}
		List<Row> list = new List<Row>();
		List<Row> list2 = new List<Row>();
		Row[] rows = base.game.rows;
		foreach (Row row in rows)
		{
			if (row.GetCurrentPlayer() != player || row.ent == null)
			{
				continue;
			}
			RowVisibilityMode appearance = row.ent.appearance;
			if (appearance == RowVisibilityMode.Hidden || appearance == RowVisibilityMode.OnlyCharacter)
			{
				continue;
			}
			if ((bool)heldBeat)
			{
				if (row.HasBeatsAutohitByHeldbeat(heldBeat))
				{
					list.Add(row);
				}
				continue;
			}
			foreach (Beat beat in base.game.beats)
			{
				if (beat != null && beat.row == row && row.playerBox.HitCheckBeat(0.4f, beat))
				{
					list.Add(row);
					if (row.playerBox.HitCheckBeat(scnGame.GetHitMargin(player), beat))
					{
						list2.Add(row);
					}
					break;
				}
			}
		}
		if (list.Count == 0)
		{
			Pop(player);
			return;
		}
		if (list2.Count > 0)
		{
			list = list2;
		}
		List<List<Row>> list3 = new List<List<Row>>();
		Dictionary<Row, double> closestBeatOnRow = new Dictionary<Row, double>();
		foreach (Row item in list)
		{
			double num = double.MaxValue;
			foreach (Beat beat2 in base.game.beats)
			{
				float num2 = Mathf.Abs((float)(beat2.inputTime - base.conductor.audioPos));
				if ((double)num2 < num)
				{
					num = num2;
				}
			}
			closestBeatOnRow.Add(item, num);
		}
		(from r in list
			orderby r.room, r.ent.playerBox.transform.eulerAngles.z, closestBeatOnRow[r]
			select r).ToList();
		List<Row> list4 = new List<Row> { list[0] };
		if (list.Count > 1)
		{
			for (int num3 = 1; num3 < list.Count; num3++)
			{
				Row row2 = list[num3];
				Transform hitstripCenter = row2.ent.hitstripCenter;
				float z = row2.ent.rowContainer.eulerAngles.z;
				Row row3 = list[num3 - 1];
				Transform hitstripCenter2 = row3.ent.hitstripCenter;
				float z2 = row3.ent.rowContainer.eulerAngles.z;
				if (row2.room != row3.room || !Mathf.Approximately(z, z2) || !(closestBeatOnRow[row2] - closestBeatOnRow[row3] <= 0.02500000037252903) || !(GetDistanceAlongAxis(hitstripCenter.transform.position, hitstripCenter2.transform.position, MathF.PI / 180f * z) <= 40f))
				{
					list3.Add(list4);
					list4 = new List<Row>();
				}
				list4.Add(row2);
			}
		}
		list3.Add(list4);
		foreach (List<Row> item2 in list3)
		{
			FlashGroup(item2, player, heldBeat);
		}
	}

	private static float GetDistanceAlongAxis(Vector2 posA, Vector2 posB, float rotationRadians)
	{
		Vector2 lhs = posB - posA;
		Vector2 rhs = new Vector2(Mathf.Cos(rotationRadians), Mathf.Sin(rotationRadians));
		return Mathf.Abs(Vector2.Dot(lhs, rhs));
	}

	private void FlashGroup(List<Row> group, RDPlayer player, bool isHeldBeat)
	{
		RDHitStrip rDHitStrip = null;
		RDHitStrip rDHitStrip2 = null;
		foreach (Row item in group)
		{
			for (int i = 0; i < hitstrips.Length; i++)
			{
				RDHitStrip rDHitStrip3 = hitstrips[i];
				if (rDHitStrip3 == null)
				{
					if ((object)rDHitStrip2 == null)
					{
						rDHitStrip2 = (hitstrips[i] = RDBase.Vfx.hitStripPrefab.Instantiate(base.transform, "Hitstrip" + i).GetComponent<RDHitStrip>());
					}
					break;
				}
				if (!rDHitStrip3.flashing && (object)rDHitStrip2 == null)
				{
					rDHitStrip2 = rDHitStrip3;
				}
				if (rDHitStrip3.rows.Contains(item))
				{
					rDHitStrip = rDHitStrip3;
					break;
				}
			}
			if (rDHitStrip != null)
			{
				break;
			}
		}
		if ((object)rDHitStrip == null)
		{
			rDHitStrip = rDHitStrip2;
		}
		rDHitStrip.rows = group;
		rDHitStrip.SetPlayer(player);
		rDHitStrip.Flash(isHeldBeat);
	}

	public void Release(RDPlayer player)
	{
		if (base.game.currentLevel.noHitStrips)
		{
			return;
		}
		RDHitStrip[] array = hitstrips;
		foreach (RDHitStrip rDHitStrip in array)
		{
			if (!(rDHitStrip == null))
			{
				Row mainRow = rDHitStrip.mainRow;
				if (mainRow != null && mainRow.GetCurrentPlayer() == player)
				{
					rDHitStrip.StopHolding();
				}
				continue;
			}
			break;
		}
	}

	public void SetHoldProgress(RDPlayer player, float progress)
	{
		if (base.game.currentLevel.noHitStrips)
		{
			return;
		}
		RDHitStrip[] array = hitstrips;
		foreach (RDHitStrip rDHitStrip in array)
		{
			if (!(rDHitStrip == null))
			{
				Row mainRow = rDHitStrip.mainRow;
				if (mainRow != null && mainRow.GetCurrentPlayer() == player && rDHitStrip.flashing && rDHitStrip.holding)
				{
					rDHitStrip.SetHoldProgress(progress);
					rDHitStrip.holdParticlesSpeed = 1f + progress * 4f;
				}
				continue;
			}
			break;
		}
	}

	private void Pop(RDPlayer player)
	{
		if (base.game.currentLevel.noPopHitstrips || base.game.currentLevel.showHitstripOnlyOnActiveBeats || !lastActiveRowInRoomForPlayer.TryGetValue(player, out var value))
		{
			return;
		}
		Row[] array = value;
		foreach (Row row in array)
		{
			if (row != null)
			{
				RowVisibilityMode appearance = row.ent.appearance;
				if (appearance != RowVisibilityMode.Hidden && appearance != RowVisibilityMode.OnlyCharacter)
				{
					FlashGroup(new List<Row> { row }, player, isHeldBeat: false);
				}
			}
		}
	}

	public void PingRow(Row row)
	{
		RDPlayer currentPlayer = row.GetCurrentPlayer();
		lastActiveRowInRoomForPlayer.TryAdd(currentPlayer, new Row[4]);
		lastActiveRowInRoomForPlayer[currentPlayer][row.room] = row;
	}
}
