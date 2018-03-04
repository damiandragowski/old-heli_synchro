using System;
using System.Collections;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectPlay;

namespace VRnet
{
	/// <summary>
	/// Summary description for NETPlayer.
	/// </summary>
	public class NETPlayer
	{
		private GameWorld game = null;
		private Peer peerObject = null;
		private ConnectWizard Connect = null;
		private Hashtable playerList = new Hashtable();
		private bool isHost = false;
		private bool inSession = false;	




		public bool IsHost { get { return isHost; } }
		public bool InSession { get { return inSession; } }
		public Hashtable PlayerList { get {	lock (playerList) {	return playerList; } } }


		private Guid AppGuid = new Guid(0x147184df, 0x547d, 0x4d9e, 0xb6, 0x3c, 0x9f, 0x5c, 0xe9, 0x6a, 0xbf, 0x77);

		public NETPlayer(GameWorld gameworld)
		{
			//
			// TODO: Add constructor logic here
			//
			game = gameworld;
			peerObject = new Peer();

			peerObject.PlayerCreated += new PlayerCreatedEventHandler(game.PlayerCreated);
			peerObject.PlayerDestroyed += new PlayerDestroyedEventHandler(game.PlayerDestroyed);
			peerObject.Receive += new ReceiveEventHandler(game.DataReceived);
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

		public void SendGameState(MessageType i) 
		{
			if (inSession) 
			{
				byte messageType = (byte)i;
				NetworkPacket packet = new NetworkPacket();

				packet.Write(messageType);

				peerObject.SendTo((int)PlayerID.AllPlayers, packet, 0, SendFlags.Guaranteed | SendFlags.NoLoopback);
			}
		}

		public void SendTimeUpdate(float time) 
		{
			if (inSession) 
			{
				byte messageType = (byte)MessageType.GameTime;
				NetworkPacket packet = new NetworkPacket();

				packet.Write(messageType);
				packet.Write(time);

				peerObject.SendTo((int)PlayerID.AllPlayers, packet, 0, SendFlags.PriorityHigh | SendFlags.Sync| SendFlags.NoLoopback);
			}
		}


		public void SendPlayerUpdate(PlayerUpdate update) 
		{
			if (inSession) 
			{
				NetworkPacket packet = new NetworkPacket();

				packet.Write((byte)MessageType.PlayerUpdateID);
				packet.Write(update);

				peerObject.SendTo((int)PlayerID.AllPlayers, packet, 0, SendFlags.Sync| SendFlags.NoLoopback);
			}
		}

		public void SendPlayerMessage(MessageUpdate update, string text) 
		{
			if (inSession) 
			{
				NetworkPacket packet = new NetworkPacket();

				packet.Write((byte)MessageType.GameChat);
				packet.Write(update);

				packet.Write(text);

				peerObject.SendTo((int)PlayerID.AllPlayers, packet, 0, SendFlags.NoComplete | SendFlags.NoLoopback);
			}
		}


	}
}
