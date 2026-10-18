using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class PlatformHelper
{
	public struct Rectangle
	{
		public int Left;

		public int Top;

		public int Right;

		public int Bottom;

		public override string ToString()
		{
			return $"Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}";
		}
	}

	public class Monitor
	{
		public Rectangle bounds;

		public int scale;

		public Monitor(Rectangle bounds, int scale)
		{
			this.bounds = bounds;
			this.scale = scale;
		}

		public override string ToString()
		{
			return $"bounds: {bounds}, scale: {scale}";
		}
	}

	public const int DontModify = -100000;

	public static PlatformHelper instance;

	public float scalingFactor = 1f;

	public int currentMonitorIndex = -1;

	private List<Monitor> monitors;

	private Vector2Int windowDanceResolution = Vector2Int.zero;

	public UnityPlayerWindow playerWindow;

	public Vector2Int WindowDanceResolution
	{
		get
		{
			if (windowDanceResolution == Vector2Int.zero)
			{
				Vector2Int vector2Int = windowDanceResolution;
				windowDanceResolution = GetWindowDanceResolution();
				if (vector2Int != windowDanceResolution)
				{
					Debug.Log($"Window Dance Resolution: {windowDanceResolution}");
				}
			}
			return windowDanceResolution;
		}
	}

	public List<Monitor> Monitors
	{
		get
		{
			if (monitors == null)
			{
				int num = ((monitors == null) ? (-1) : monitors.Count);
				monitors = instance.GetMonitors();
				if (num != monitors.Count)
				{
					Debug.Log($"Found {monitors.Count} monitors");
				}
			}
			return monitors;
		}
	}

	public virtual void InvalidateMonitors()
	{
		monitors = null;
		windowDanceResolution = Vector2Int.zero;
	}

	public abstract Vector2Int GetWindowDanceResolution();

	protected abstract List<Monitor> GetMonitors();

	public virtual Vector2Int TranslateWindowPos(Vector2Int position, Vector2Int windowSize)
	{
		return position;
	}

	public void ResetWindowStageValues()
	{
		windowDanceResolution = Vector2Int.zero;
		monitors = null;
	}

	public List<Vector2Int> GetMonitorsVertices(List<Monitor> monitors)
	{
		List<Vector2Int> list = new List<Vector2Int>();
		foreach (Monitor monitor in monitors)
		{
			list.Add(new Vector2Int(monitor.bounds.Left, monitor.bounds.Top));
			list.Add(new Vector2Int(monitor.bounds.Right, monitor.bounds.Top));
			list.Add(new Vector2Int(monitor.bounds.Right, monitor.bounds.Bottom));
			list.Add(new Vector2Int(monitor.bounds.Left, monitor.bounds.Bottom));
		}
		return list;
	}

	public Rectangle GetMonitorsRect(List<Monitor> monitors, bool onlyHorizontal)
	{
		if (monitors.Count == 0)
		{
			Debug.Log("monitors.Count is 0!");
			int num = -1;
			int num2 = -1;
			int num3 = 0;
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];
				int num4 = resolution.width * resolution.height;
				if (num4 > num2)
				{
					num2 = num4;
					num = num3;
				}
				num3++;
			}
			Resolution resolution2 = Screen.resolutions[num];
			return new Rectangle
			{
				Left = 0,
				Top = 0,
				Bottom = resolution2.height,
				Right = resolution2.width
			};
		}
		if (monitors.Count == 1)
		{
			return monitors[0].bounds;
		}
		List<Rectangle> monitorsBounds = new List<Rectangle>();
		foreach (Monitor monitor in monitors)
		{
			monitorsBounds.Add(monitor.bounds);
		}
		Rectangle currentVertexBounds = monitorsBounds[0];
		List<Vector2Int> monitorsVertices = GetMonitorsVertices(monitors);
		int top = 0;
		int bottom = 0;
		int maxTop = 0;
		int maxBottom = 0;
		List<Rectangle> list = new List<Rectangle>();
		int num5 = 0;
		int num6 = 0;
		foreach (Vector2Int item2 in monitorsVertices)
		{
			currentVertexBounds = monitorsBounds[num5];
			num6++;
			if (num6 == 4)
			{
				num6 = 0;
				num5++;
			}
			List<int> list2 = GetSide(item2.y, item2.x, leftSide: true);
			List<int> list3 = GetSide(item2.y, item2.x, leftSide: false);
			foreach (int left in list2)
			{
				foreach (int right in list3)
				{
					if (right > left)
					{
						bottom = (top = (maxTop = (maxBottom = item2.y)));
						GetHeight(left, right);
						if (bottom > top && right > left && list.FindIndex((Rectangle r) => top == r.Top && left == r.Left && right == r.Right && bottom == r.Bottom) == -1)
						{
							Rectangle item = new Rectangle
							{
								Top = top,
								Left = left,
								Right = right,
								Bottom = bottom
							};
							list.Add(item);
						}
					}
				}
			}
		}
		list.AddRange(monitorsBounds.Distinct().ToList());
		if (currentMonitorIndex >= 0)
		{
			Rectangle bounds = monitors[currentMonitorIndex].bounds;
			Vector2Int vector2Int = new Vector2Int(bounds.Left + (bounds.Right - bounds.Left) / 2, bounds.Top + (bounds.Bottom - bounds.Top) / 2);
			foreach (Rectangle item3 in new List<Rectangle>(list))
			{
				if (((item3.Top > bounds.Top || item3.Bottom < bounds.Bottom) && (item3.Left > vector2Int.x || item3.Right < vector2Int.x)) || ((item3.Left > bounds.Left || item3.Right < bounds.Right) && (item3.Top > vector2Int.y || item3.Bottom < vector2Int.y)))
				{
					list.Remove(item3);
				}
			}
		}
		int num7 = 0;
		Rectangle result = default;
		foreach (Rectangle item4 in list)
		{
			int num8 = Mathf.Abs((item4.Bottom - item4.Top) * (item4.Right - item4.Left));
			if (num8 > num7)
			{
				num7 = num8;
				result = item4;
			}
		}
		return result;
		void GetHeight(int num9, int num10)
		{
			bool flag = num9 < num10;
			List<Rectangle> sideMonitors = new List<Rectangle>(monitorsBounds.FindAll((Rectangle m) => m.Left <= num9 && m.Right > num9));
			List<Rectangle> sideMonitors2 = new List<Rectangle>(monitorsBounds.FindAll((Rectangle m) => m.Left < num10 && m.Right >= num10));
			List<Rectangle> list4 = GetHeightMonitors(sideMonitors, isTop: true);
			List<Rectangle> list5 = GetHeightMonitors(sideMonitors2, isTop: true);
			int num11 = ((list4.Count > 0) ? list4[list4.Count - 1].Top : top);
			int num12 = ((list5.Count > 0) ? list5[list5.Count - 1].Top : top);
			maxTop = ((num11 > num12) ? num11 : num12);
			if (maxTop < top)
			{
				List<Rectangle> list6 = new List<Rectangle>(list4);
				list6.AddRange(list5);
				List<int> list7 = new List<int>();
				foreach (Rectangle item5 in list6)
				{
					if (item5.Bottom < top && item5.Bottom >= maxTop)
					{
						list7.Add(item5.Bottom);
					}
					if (item5.Top < top && item5.Top >= maxTop)
					{
						list7.Add(item5.Top);
					}
				}
				list7 = list7.Distinct().ToList();
				list7.Sort((int a, int b) => b.CompareTo(a));
				foreach (int item6 in list7)
				{
					if (item6 < top && (!onlyHorizontal || item6 >= currentVertexBounds.Top))
					{
						List<int> source = GetSide(item6, num9, !flag);
						int num13 = ((!flag) ? source.Min() : source.Max());
						if ((flag && num13 < num10) || (!flag && num13 > num10))
						{
							break;
						}
						top = item6;
					}
				}
			}
			List<Rectangle> list8 = GetHeightMonitors(sideMonitors, isTop: false);
			List<Rectangle> list9 = GetHeightMonitors(sideMonitors2, isTop: false);
			int num14 = ((list8.Count > 0) ? list8[list8.Count - 1].Bottom : bottom);
			int num15 = ((list9.Count > 0) ? list9[list9.Count - 1].Bottom : bottom);
			maxBottom = ((num14 < num15) ? num14 : num15);
			if (maxBottom > bottom)
			{
				List<Rectangle> list10 = new List<Rectangle>(list8);
				list10.AddRange(list9);
				List<int> list11 = new List<int>();
				foreach (Rectangle item7 in list10)
				{
					if (item7.Bottom > bottom && item7.Bottom <= maxBottom)
					{
						list11.Add(item7.Bottom);
					}
					if (item7.Top > bottom && item7.Top <= maxBottom)
					{
						list11.Add(item7.Top);
					}
				}
				list11 = list11.Distinct().ToList();
				list11.Sort();
				foreach (int item8 in list11)
				{
					if (item8 >= bottom && (!onlyHorizontal || item8 <= currentVertexBounds.Bottom))
					{
						List<int> source2 = GetSide(item8, num9, !flag);
						int num16 = ((!flag) ? source2.Min() : source2.Max());
						if ((flag && num16 < num10) || (!flag && num16 > num10))
						{
							break;
						}
						bottom = item8;
					}
				}
			}
		}
		List<Rectangle> GetHeightMonitors(List<Rectangle> sideMonitors, bool isTop)
		{
			List<Rectangle> list4 = new List<Rectangle>();
			foreach (Rectangle sideMonitor in sideMonitors)
			{
				if ((isTop && sideMonitor.Top <= top) || (!isTop && sideMonitor.Bottom >= bottom))
				{
					list4.Add(sideMonitor);
				}
			}
			list4.Sort((Rectangle a, Rectangle b) => ((isTop && a.Top < b.Top) || (!isTop && a.Top > b.Top)) ? 1 : (-1));
			int num9 = 0;
			for (int num10 = 1; num10 < list4.Count; num10++)
			{
				if ((isTop && list4[num10].Bottom != list4[num10 - 1].Top) || (!isTop && list4[num10].Top != list4[num10 - 1].Bottom))
				{
					num9 = num10;
					break;
				}
			}
			if (list4.Count > 0 && num9 > 0)
			{
				list4.RemoveRange(num9, list4.Count - num9);
			}
			return list4;
		}
		List<int> GetSide(int height, int origin, bool leftSide)
		{
			List<Rectangle> list4 = new List<Rectangle>();
			list4.AddRange(monitorsBounds.FindAll((Rectangle monitor) => monitor.Top <= height && monitor.Bottom >= height));
			List<Rectangle> list5 = new List<Rectangle>();
			list5.AddRange(list4.FindAll((Rectangle monitor) =>
			{
				if (!leftSide)
				{
					if (monitor.Left >= origin)
					{
						return monitor.Right > origin;
					}
					return false;
				}
				return monitor.Left < origin && monitor.Right <= origin;
			}));
			list5.Sort((Rectangle a, Rectangle b) =>
			{
				if (leftSide)
				{
					if (a.Left >= b.Left)
					{
						return -1;
					}
					return 1;
				}
				return (a.Right > b.Right) ? 1 : (-1);
			});
			int num9 = 0;
			for (int num10 = 1; num10 < list5.Count; num10++)
			{
				if (list5[num10].Left != list5[num10 - 1].Right)
				{
					num9 = num10;
					break;
				}
			}
			if (list5.Count > 0)
			{
				if (origin != list5[0].Left && origin != list5[0].Right)
				{
					list5 = new List<Rectangle>();
				}
				else if (num9 > 0)
				{
					list5.RemoveRange(num9, list5.Count - num9);
				}
			}
			List<int> list6 = new List<int> { origin };
			foreach (Rectangle item9 in list5)
			{
				if (leftSide)
				{
					if (item9.Right < origin)
					{
						list6.Add(item9.Right);
					}
					if (item9.Left < origin)
					{
						list6.Add(item9.Left);
					}
				}
				else
				{
					if (item9.Right > origin)
					{
						list6.Add(item9.Right);
					}
					if (item9.Left > origin)
					{
						list6.Add(item9.Left);
					}
				}
			}
			return list6.Distinct().ToList();
		}
	}
}
