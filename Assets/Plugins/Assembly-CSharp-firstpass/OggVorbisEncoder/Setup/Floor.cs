using System.Linq;

namespace OggVorbisEncoder.Setup
{
	public class Floor
	{
		public int[] PartitionClass { get; }

		public int[] ClassDimensions { get; }

		public int[] ClassSubs { get; }

		public int[] ClassBook { get; }

		public int[][] ClassSubBook { get; }

		public int Mult { get; }

		public int[] PostList { get; }

		public float MaxOver { get; }

		public float MaxUnder { get; }

		public float MaxError { get; }

		public float TwoFitWeight { get; }

		public float TwoFitAtten { get; }

		public int N { get; set; }

		public Floor(int[] partitionClass, int[] classDimensions, int[] classSubs, int[] classBook, int[][] classSubBook, int mult, int[] postList, float maxOver, float maxUnder, float maxError, float twoFitWeight, float twoFitAtten, int n)
		{
			PartitionClass = partitionClass;
			ClassDimensions = classDimensions;
			ClassSubs = classSubs;
			ClassBook = classBook;
			ClassSubBook = classSubBook;
			Mult = mult;
			PostList = postList;
			MaxOver = maxOver;
			MaxUnder = maxUnder;
			MaxError = maxError;
			TwoFitWeight = twoFitWeight;
			TwoFitAtten = twoFitAtten;
			N = n;
		}

		public Floor Clone()
		{
			return new Floor(PartitionClass.ToArray(), ClassDimensions.ToArray(), ClassSubs.ToArray(), ClassBook.ToArray(), ClassSubBook.Select((int[] s) => s.ToArray()).ToArray(), Mult, PostList.ToArray(), MaxOver, MaxUnder, MaxError, TwoFitWeight, TwoFitAtten, N);
		}
	}
}
