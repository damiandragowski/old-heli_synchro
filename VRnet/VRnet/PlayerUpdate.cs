using System;
using Microsoft.DirectX;

namespace VRnet
{
	public struct PlayerUpdate 
	{
		public Vector2 position;
		public Vector2 velocity;
	}

	public struct MessageUpdate
	{
		public int Length;
	}
}