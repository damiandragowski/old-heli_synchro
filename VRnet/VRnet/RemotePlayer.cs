using System;
using Microsoft.DirectX;

namespace VRnet
{
	/// <summary>
	/// Summary description for RemotePlayer.
	/// </summary>
	public class RemotePlayer
	{
		int playerID;
		string hostName;
		DateTime updateTime;			
		bool active = true;				
		Vector2 position = new Vector2(0,0);
		Vector2 velocity = new Vector2(0,0);

		public RemotePlayer(int playerID, string hostName)
		{
			//
			// TODO: Add constructor logic here
			//
			this.playerID = playerID;
			this.hostName = hostName;
		}
		public override string ToString() 
		{
			return hostName;
		}
		public int PlayerID 
		{
			get 
			{
				return playerID;
			}
		}

		public Vector2 Position 
		{
			get 
			{
				return position;
			}
			set 
			{
				position = value;
			}
		}

		public Vector2 Velocity 
		{
			get 
			{
				return velocity;
			}
			set 
			{
				velocity = value;
			}
		}


		public DateTime UpdateTime 
		{
			get 
			{
				return updateTime;
			}
			set 
			{
				updateTime = value;
			}
		}

		public void Update(float time)
		{
			position.X = position.X + time * velocity.X;
			position.Y = position.Y + time * velocity.Y;
		}

		public bool Active 
		{
			get 
			{
				return active;
			}
			set 
			{
				active = value;
			}
		}

	}
}
