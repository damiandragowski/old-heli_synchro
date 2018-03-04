using System;
using System.Collections;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectPlay;
using Heli;
using System.Reflection;

namespace NetLayer
{
	/// <summary>
	/// Summary description for NETPlayer.
	/// </summary>
	public class NETPlayer
	{
		private NetLayer layer = null;
		public  Peer peerObject = null;
		private ConnectWizard Connect = null;
		private Hashtable playerList = new Hashtable();
		private bool isHost = false;
		private bool inSession = false;	




		public bool IsHost { get { return isHost; } }
		public bool InSession { get { return inSession; } }
		public Hashtable PlayerList { get {	lock (playerList) {	return playerList; } } }


		private Guid AppGuid = new Guid(0x147184df, 0x547d, 0x4d9e, 0xb6, 0x3c, 0x9f, 0x5c, 0xe9, 0x6a, 0xbf, 0x77);

		public NETPlayer(NetLayer netlayer)
		{
			//
			// TODO: Add constructor logic here
			//
			layer = netlayer;
			peerObject = new Peer();

			peerObject.PlayerCreated += new PlayerCreatedEventHandler(layer.PlayerCreated);
			peerObject.PlayerDestroyed += new PlayerDestroyedEventHandler(layer.PlayerDestroyed);
			peerObject.Receive += new ReceiveEventHandler(layer.DataReceived);
			peerObject.SessionTerminated += new SessionTerminatedEventHandler(SessionTerminated);

			Connect = new ConnectWizard(peerObject, AppGuid, "Helikopter");
			Connect.StartWizard();

			inSession = Connect.InSession;

			if (inSession) 
			{
				isHost = Connect.IsHost;
			}
		}

		private void SessionTerminated(object sender, SessionTerminatedEventArgs stea) 
		{
			inSession = false;
			isHost = false;
		}	



		public void SendTimeUpdate(float time,int timeCount) 
		{
			if (inSession) 
			{
				byte messageType = (byte)MessageType.GameTime;
				NetworkPacket packet = new NetworkPacket();

				packet.Write(messageType);
				packet.Write(time);
				packet.Write(timeCount);

				peerObject.SendTo((int)PlayerID.AllPlayers, packet, 0, SendFlags.PriorityHigh | SendFlags.Sync| SendFlags.NoLoopback);
			}
		}

		public void SendShutDown()
		{
			if (inSession) 
			{
				NetworkPacket packet = new NetworkPacket();

				packet.Write((byte)MessageType.ServerDown);
				peerObject.SendTo((int)PlayerID.AllPlayers, packet, 0, SendFlags.Sync| SendFlags.NoLoopback);
			}
		}

		public void SendPlayerUpdate(HeliSynchStru update) 
		{
			if (inSession) 
			{
				NetworkPacket packet = new NetworkPacket();

				packet.Write((byte)MessageType.PlayerUpdatePosition);
				Type type = update.GetType();
				FieldInfo [] fields = type.GetFields();
				foreach( FieldInfo im in  fields)
				{
					//im.GetValue
					ValueType tt = (ValueType)im.GetValue(update);
					packet.Write(tt);
				}

				peerObject.SendTo((int)PlayerID.AllPlayers, packet, 0, SendFlags.Sync| SendFlags.NoLoopback);
			}
		}
}
}
