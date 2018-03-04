using System;
using Microsoft.DirectX;
using Heli;
using MathLib;

namespace NetLayer
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
		public Helicopter heli=null;
		/*
		Vector2 position = new Vector2(0,0);
		Vector2 velocity = new Vector2(0,0);
*/
		public RemotePlayer(int playerID, string hostName,Helicopter h)
		{
			//
			// TODO: Add constructor logic here
			//
			this.playerID = playerID;
			this.hostName = hostName;
			this.heli = h;
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

		/*
		public void Update(float time)
		{
			position.X = position.X + time * velocity.X;
			position.Y = position.Y + time * velocity.Y;
		}*/

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
