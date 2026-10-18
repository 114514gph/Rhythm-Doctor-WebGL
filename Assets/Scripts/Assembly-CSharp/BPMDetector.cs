using System;
using System.Collections.Generic;
using System.Linq;

public class BPMDetector
{
	private short[] leftChn;

	private double BPM;

	private double sampleRate = 44100.0;

	private double trackLength;

	public double getBPM()
	{
		return BPM;
	}

	public BPMDetector(string filename)
	{
		Detect();
	}

	public BPMDetector(short[] leftChn, short[] rightChn)
	{
		this.leftChn = leftChn;
		Detect();
	}

	private void Detect()
	{
		trackLength = (double)(float)leftChn.Length / sampleRate;
		int num = 3600;
		List<double> list = new List<double>();
		for (int i = 0; i < leftChn.Length - num - 1; i += num)
		{
			list.Add(rangeQuadSum(leftChn, i, i + num));
		}
		int num2 = 0;
		double average = 0.0;
		double num3 = 0.0;
		int num4 = 10;
		for (int j = num4; j <= list.Count - num4 - 1; j++)
		{
			double num5 = list[j];
			double num6 = rangeSum(list.ToArray(), j - num4, j - 1) + num5 + rangeSum(list.ToArray(), j + 1, j + num4);
			num6 /= (double)(num4 * 2 + 1);
			List<double> list2 = list.Skip(j - 5).Take(5).Concat(list.Skip(j + 1).Take(5))
				.ToList();
			average = list2.Average();
			num3 = list2.Select((double val) => (val - average) * (val - average)).Sum() / (double)list2.Count / Math.Pow(10.0, 22.0) * 0.009 + 1.385;
			if (num5 > num3 * num6)
			{
				num2++;
			}
		}
		BPM = (double)num2 / (trackLength / 60.0);
	}

	private static double rangeQuadSum(short[] samples, int start, int stop)
	{
		double num = 0.0;
		for (int i = start; i <= stop; i++)
		{
			num += Math.Pow(samples[i], 2.0);
		}
		return num;
	}

	private static double rangeSum(double[] data, int start, int stop)
	{
		double num = 0.0;
		for (int i = start; i <= stop; i++)
		{
			num += data[i];
		}
		return num;
	}
}
