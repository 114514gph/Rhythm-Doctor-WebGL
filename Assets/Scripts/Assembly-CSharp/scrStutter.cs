using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

public sealed class scrStutter : RDBase
{
	private static class AudioDspManager
	{
		private const int MaxSamples = 16;

		private static Queue<(double x, double y)> points { get; } = new Queue<(double, double)>();

		private static double intercept { get; set; }

		private static double slope { get; set; }

		private static double previousSmoothedDspTime { get; set; }

		private static double currentGameTime => Time.realtimeSinceStartupAsDouble;

		private static double currentDspTime => AudioSettings.dspTime;

		public static double smoothedDspTime
		{
			get
			{
				double num = currentGameTime * slope + intercept;
				if (num < previousSmoothedDspTime)
				{
					return previousSmoothedDspTime;
				}
				if (scnGame.instance.paused)
				{
					return previousSmoothedDspTime;
				}
				previousSmoothedDspTime = num;
				return num;
			}
		}

		public static void UpdateRegression()
		{
			if (scnGame.instance.paused)
			{
				points.Clear();
			}
			else
			{
				if (points.Any(((double x, double y) point) => point.y == currentDspTime))
				{
					return;
				}
				points.Enqueue((currentGameTime, currentDspTime));
				if (points.Count > 16)
				{
					points.Dequeue();
				}
				if (points.Count == 1)
				{
					slope = 0.0;
					intercept = points.Single().y;
					return;
				}
				double xMean = points.Select(((double x, double y) point) => point.x).Average();
				double yMean = points.Select(((double x, double y) point) => point.y).Average();
				double num = points.Select(((double x, double y) point) => (point.x - xMean) * (point.y - yMean)).Sum();
				double num2 = points.Select(((double x, double y) point) => (point.x - xMean) * (point.x - xMean)).Sum();
				slope = num / num2;
				intercept = yMean - slope * xMean;
			}
		}
	}

	private record PlaybackInfo(float secondsPerLoop, int totalStutters, bool enableStutterStack, int playbackSpeed)
	{
		public int? startFrame { get; set; }

		public int? endFrame { get; set; }

		public int? firstLoopCurrentFrame { get; set; }

		public double startTime { get; set; }

		public float secondsPerLoop { get; } = secondsPerLoop;

		public int totalStutters { get; } = totalStutters;

		public bool enableStutterStack { get; } = enableStutterStack;

		public int playbackSpeed { get; } = playbackSpeed;
	}

	private class PlaybackManager
	{
		private RenderTextureBuffer recordedFrames { get; }

		private PlaybackInfo playbackInfo
		{
			[return: MaybeNull]
			get;
			set;
		}

		public PlaybackManager(RenderTextureBuffer renderTextureBuffer)
		{
			recordedFrames = renderTextureBuffer;
		}

		public void OnRenderImage(Texture source, RenderTexture dest)
		{
			if (playbackInfo == null)
			{
				Graphics.Blit(source, dest);
				return;
			}
			double num = AudioDspManager.smoothedDspTime - playbackInfo.startTime;
			float num2 = playbackInfo.secondsPerLoop / (float)Math.Abs(playbackInfo.playbackSpeed);
			int num3 = (int)(num / (double)num2);
			if (num3 >= playbackInfo.totalStutters)
			{
				EndCurrentStutter();
				Graphics.Blit(source, dest);
				return;
			}
			if (!playbackInfo.endFrame.HasValue)
			{
				Graphics.Blit(recordedFrames[playbackInfo.firstLoopCurrentFrame.Value], dest);
				playbackInfo.firstLoopCurrentFrame++;
			}
			else
			{
				int value = playbackInfo.startFrame.Value;
				int value2 = playbackInfo.endFrame.Value;
				double num4 = num % (double)num2;
				float num5 = num2 / (float)(Math.Abs(value - value2) + 1);
				int index = ((playbackInfo.playbackSpeed > 0) ? (value + (int)(num4 / (double)num5)) : (value2 - (int)(num4 / (double)num5)));
				Graphics.Blit(recordedFrames[index], dest);
			}
			if (playbackInfo.enableStutterStack)
			{
				for (int i = 0; i < num3 * num3; i++)
				{
					Graphics.DrawTexture(new Rect(0f, RDBase.Vfx.RDHeight - RDBase.Vfx.RDHeight / 6f * (float)(i + 1), RDBase.Vfx.RDWidth, 0f - RDBase.Vfx.RDHeight), source);
				}
			}
		}

		public void StartPlayback(PlaybackInfo newStutterInfo)
		{
			EndCurrentStutter();
			newStutterInfo.startTime = AudioDspManager.smoothedDspTime;
			playbackInfo = newStutterInfo;
		}

		public void EndCurrentStutter()
		{
			if (!(playbackInfo == null))
			{
				recordedFrames.MarkFrameUnused(playbackInfo.startFrame.Value);
				playbackInfo = null;
			}
		}
	}

	private class RecordingManager
	{
		private readonly RenderTextureBuffer recordedFrames;

		private int recordingStutterCount { get; set; }

		private int numberOfStuttersToStopRecording { get; set; }

		private int currentRecordingFrame => recordedFrames.Count;

		public RecordingManager(RenderTextureBuffer renderTextureBuffer)
		{
			recordedFrames = renderTextureBuffer;
		}

		public void OnRenderImage(Texture source)
		{
			if (recordingStutterCount != 0)
			{
				Graphics.Blit(source, recordedFrames.EnqueueNewRenderTexture());
				recordingStutterCount -= numberOfStuttersToStopRecording;
				numberOfStuttersToStopRecording = 0;
			}
		}

		public int StartRecording()
		{
			recordingStutterCount++;
			recordedFrames.MarkFrameUsed(currentRecordingFrame);
			return currentRecordingFrame;
		}

		public int EndRecording()
		{
			numberOfStuttersToStopRecording++;
			return currentRecordingFrame;
		}
	}

	private class RenderTextureBuffer
	{
		private Queue<RenderTexture> renderTextures { get; } = new Queue<RenderTexture>();

		private ConcurrentBag<RenderTexture> unusedRenderTextures { get; } = new ConcurrentBag<RenderTexture>();

		private Dictionary<int, int> framesInUse { get; } = new Dictionary<int, int>();

		private int apparentOffset { get; set; }

		public RenderTexture this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("{0}={1} cannot be less than zero.", "index", index));
				}
				if (index < apparentOffset)
				{
					throw new ArgumentOutOfRangeException("index", string.Format("{0}={1} is less than offset {2}. ", "index", index, apparentOffset) + "The frame you are trying to access has already been garbage collected and reused.");
				}
				return renderTextures.ElementAt(index - apparentOffset);
			}
		}

		public int Count => renderTextures.Count + apparentOffset;

		public override string ToString()
		{
			return "RenderTextureBuffer(" + $"renderTextures.Count = {renderTextures.Count}, " + $"unusedRenderTextures.Count = {unusedRenderTextures.Count}, " + "framesInUse = { " + string.Join(", ", framesInUse) + " }, " + $"apparentOffset = {apparentOffset}" + ")";
		}

		public RenderTexture EnqueueNewRenderTexture()
		{
			RenderTexture renderTexture = (unusedRenderTextures.TryTake(out var result) ? result : new RenderTexture(RDBase.Vfx.RDWidthInt * 2, RDBase.Vfx.RDHeightInt * 2, 0, RenderTextureFormat.ARGB32)
			{
				filterMode = FilterMode.Point
			});
			renderTextures.Enqueue(renderTexture);
			return renderTexture;
		}

		public void MarkFrameUsed(int frameIndex)
		{
			framesInUse.TryAdd(frameIndex, 0);
			framesInUse[frameIndex]++;
		}

		public void MarkFrameUnused(int frameIndex)
		{
			if (!framesInUse.ContainsKey(frameIndex) || framesInUse[frameIndex] <= 0)
			{
				throw new ArgumentOutOfRangeException("frameIndex", $"{frameIndex} is not currently marked as used.");
			}
			framesInUse[frameIndex]--;
		}

		public void ReleaseAll()
		{
			while (renderTextures.Count > 0)
			{
				renderTextures.Dequeue().Release();
				apparentOffset++;
			}
			RenderTexture result;
			while (unusedRenderTextures.TryTake(out result))
			{
				result.Release();
			}
		}

		public void ReleaseUnused()
		{
			int num = ((framesInUse.Count == 0) ? renderTextures.Count : (framesInUse.Keys.Min() - apparentOffset));
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					unusedRenderTextures.Add(renderTextures.Dequeue());
				}
				apparentOffset += num;
			}
		}
	}

	private RenderTextureBuffer recordedFrames { get; set; }

	private RecordingManager recordingManager { get; set; }

	private PlaybackManager playbackManager { get; set; }

	private void Awake()
	{
		recordedFrames = new RenderTextureBuffer();
		recordingManager = new RecordingManager(recordedFrames);
		playbackManager = new PlaybackManager(recordedFrames);
	}

	private void OnDestroy()
	{
		recordedFrames.ReleaseAll();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		AudioDspManager.UpdateRegression();
		source.filterMode = FilterMode.Point;
		recordingManager.OnRenderImage(source);
		playbackManager.OnRenderImage(source, dest);
	}

	public void AddStutter(float playBeat, float sourceBeat, float length, int numOfLoops, bool stack = false, int playbackSpeed = 1)
	{
		if (playBeat < sourceBeat)
		{
			throw new ArgumentOutOfRangeException("playBeat", $"playBeat={playBeat} < sourceBeat={sourceBeat}: Playback cannot start before recording starts.");
		}
		switch (playbackSpeed)
		{
		case 0:
			throw new ArgumentOutOfRangeException("playbackSpeed", "playbackSpeed cannot be zero.");
		default:
			if (sourceBeat + length > playBeat)
			{
				throw new ArgumentOutOfRangeException("playbackSpeed", string.Format("{0} is set, but {1}={2} + ", "playbackSpeed", "sourceBeat", sourceBeat) + string.Format("{0}={1} > {2}={3}: ", "length", length, "playBeat", playBeat) + "Playback cannot start before recording finishes with a custom playback speed, because you may be playing in reverse, or it could cause the first loop of playback to finish before we're done recording.");
			}
			break;
		case 1:
			break;
		}
		if (length == 0f)
		{
			throw new ArgumentOutOfRangeException("length", "length cannot equal zero.");
		}
		float secondsPerLoop = base.conductor.BeatToTime(sourceBeat + length) - base.conductor.BeatToTime(sourceBeat);
		PlaybackInfo playbackInfo = new PlaybackInfo(secondsPerLoop, numOfLoops, stack, playbackSpeed);
		if (sourceBeat + length > playBeat)
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				int value = recordingManager.StartRecording();
				playbackInfo.startFrame = value;
				playbackInfo.firstLoopCurrentFrame = value;
			}, sourceBeat, "start recording parallel");
			scrExecuteOnCertainBeat.Add(() =>
			{
				playbackManager.StartPlayback(playbackInfo);
			}, playBeat, "start playback parallel");
			scrExecuteOnCertainBeat.Add(() =>
			{
				playbackInfo.endFrame = recordingManager.EndRecording();
			}, sourceBeat + length, "end recording parallel");
		}
		else
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				playbackInfo.startFrame = recordingManager.StartRecording();
			}, sourceBeat, "start recording");
			scrExecuteOnCertainBeat.Add(() =>
			{
				playbackInfo.endFrame = recordingManager.EndRecording();
			}, sourceBeat + length, "end recording");
			scrExecuteOnCertainBeat.Add(() =>
			{
				playbackManager.StartPlayback(playbackInfo);
			}, playBeat, "playback");
		}
	}

	public void EndCurrentStutter()
	{
		playbackManager.EndCurrentStutter();
	}

	public void EndStutterOnBeat(float beat)
	{
		scrExecuteOnCertainBeat.Add(EndCurrentStutter, beat);
	}

	public void ClearStutterCache()
	{
		recordedFrames.ReleaseUnused();
	}
}
