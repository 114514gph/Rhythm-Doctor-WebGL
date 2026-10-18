#if UNITY_EDITOR || !UNITY_WEBGL
namespace Rewired.Demos.CustomPlatform
{
	public struct MyPlatformControllerIdentifier
	{
		public ushort vendorId;

		public ushort productId;
	}
}
#endif
