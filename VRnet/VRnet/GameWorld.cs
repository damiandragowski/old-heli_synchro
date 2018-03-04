using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.DirectPlay;

namespace VRnet
{
	/// <summary>
	/// Summary description for GameWorld.
	/// </summary>
	public class GameWorld
	{
		internal MainForm mainform;

		// private Microsoft.DirectX.Direct3D.Device device = null;
		private Microsoft.DirectX.DirectInput.Device keyb;

		private float lastFrameTime = 0.0f;
		private Hashtable	otherPlayers = new Hashtable();
		private NETPlayer peer;
		public Timer timer = new System.Windows.Forms.Timer();
		private float serverTime=0;
		private float appTime=0;
		private float appTimeOld =0;

		private void TimerEvent(Object myObject, EventArgs myEventArgs)
		{
			peer.SendTimeUpdate(DXUtil.Timer(DirectXTimer.GetApplicationTime));
		}



		public GameWorld(MainForm _mainform)
		{
			//
			// TODO: Add constructor logic here
			//
			mainform = _mainform;

			DXUtil.Timer(DirectXTimer.Start);


			peer = new NETPlayer(this);

			if (peer.InSession) 
				if (peer.IsHost) 
				{
					timer.Tick += new EventHandler(TimerEvent);
					timer.Interval = 4500;
					timer.Start();
				}

			InitializeKeyboard();
		}

		public void InitializeKeyboard()
		{
			keyb = new Microsoft.DirectX.DirectInput.Device(SystemGuid.Keyboard);
			keyb.SetCooperativeLevel(mainform, CooperativeLevelFlags.Background | CooperativeLevelFlags.NonExclusive);
			keyb.Acquire();
		}


		public void Initialize()
		{
			if (peer.InSession) 
			{
				if (peer.IsHost) 
				{
					peer.SendGameState(MessageType.GameRunning);
				}
				else 
				{

				}
			}

		}

		private float Speed = 5f;

		public void Loop()
		{
			float minFrameTime = Speed * 0.005f;

			if (lastFrameTime < minFrameTime) 
			{
				lastFrameTime += DXUtil.Timer(DirectXTimer.GetElapsedTime);
				return;
			}

			// read input 

			ReadKeyboard();


			if ( appTime - appTimeOld > 0.1 ) 
			{
				appTimeOld = appTime;
				if (peer.InSession  && otherPlayers.Count > 0)
					SendMyPlayerUpdate();
			}

			DateTime now = DateTime.Now;

			lock (otherPlayers) 
			{
				foreach (RemotePlayer player in otherPlayers.Values) 
				{
					if (!player.Active)
						continue;

					TimeSpan delta = now - player.UpdateTime;
					if (delta.Seconds > RemoteTickTimeout) 
					{
						player.Active = false;
					}
				}
			}

			// compute all
			if (peer.InSession) 
			{
				if (peer.IsHost) 
					appTime = DXUtil.Timer(DirectXTimer.GetApplicationTime);
				else
					appTime = DXUtil.Timer(DirectXTimer.GetApplicationTime) + serverTime;
			} 

			mainform.Text = "" + appTime;

			Update(appTime);

			// network respond

			// render 

			// me
			mainform.clear();
			mainform.setDot(position);

			// others
			lock (otherPlayers) 
			{
				foreach (RemotePlayer player in otherPlayers.Values) 
				{
					if (!player.Active)
						continue;
					mainform.setDot(player.Position);
				}
			}
			lastFrameTime = 0.0f;
		}

		float otime=0f;
		private void Update(float time)
		{
			// me
			position.X = position.X + (time-otime) * velocity.X;
			position.Y = position.Y + (time-otime) * velocity.Y;
			// others
			lock (otherPlayers) 
			{
				foreach (RemotePlayer player in otherPlayers.Values) 
				{
					if (!player.Active)
						continue;
					player.Update(time-otime);
				}
			}
			otime=time;
		}

		private void ReadKeyboard()
		{
			KeyboardState keys = keyb.GetCurrentKeyboardState();
									
			if(keys[Key.UpArrow])
				velocity.Y-=2;
			if(keys[Key.DownArrow])
				velocity.Y+=2;
			if(keys[Key.RightArrow])
				velocity.X+=2;
			if(keys[Key.LeftArrow])
				velocity.X-=2;			
		}
	


		private int RemoteTickTimeout = 2;

		private int ID;
		private string Name;
		private Vector2 position = new Vector2(0,0);
		private Vector2 velocity = new Vector2(0,0);

		public void PlayerCreated(object sender, PlayerCreatedEventArgs pcea) 
		{
			Peer peer = (Peer) sender;
			int playerID = pcea.Message.PlayerID;
			PlayerInformation playerInfo = peer.GetPeerInformation(playerID);


			if (playerInfo.Local) 
			{
				ID = playerID;
				Name = playerInfo.Name;
			}
			else 
			{		
				RemotePlayer newPlayer = new RemotePlayer(playerID, playerInfo.Name);
				lock (otherPlayers) 
				{
					otherPlayers.Add(playerID, newPlayer);
				}
			}
		}

		public void PlayerDestroyed(object sender, PlayerDestroyedEventArgs pdea) 
		{
			int PlayerID = pdea.Message.PlayerID;
			if ( PlayerID != ID ) 
			{
				lock (otherPlayers) 
				{
					otherPlayers.Remove(PlayerID);
				}
			}
		}

		public void DataReceived(object sender, ReceiveEventArgs rea) 
		{
			int senderID = rea.Message.SenderID;
			
			byte mType = (byte)rea.Message.ReceiveData.Read(typeof(byte));
			MessageType messageType = (MessageType)mType;
			switch (messageType) 
			{
				case MessageType.PlayerUpdateID: 
				{	

					PlayerUpdate update = (PlayerUpdate)rea.Message.ReceiveData.Read(typeof(PlayerUpdate));
				
					rea.Message.ReceiveData.Dispose();

					lock (otherPlayers) 
					{
						object playerObject = otherPlayers[senderID];
						if (null == playerObject)
							return;
						RemotePlayer player = (RemotePlayer) playerObject;

						player.Position = update.position;
						player.Velocity = update.velocity;
						player.UpdateTime = DateTime.Now;
						player.Active = true;

						otherPlayers[senderID] = player;
					}

					break;
				}


				case MessageType.GameTime: 
				{	

					float stime = (float)rea.Message.ReceiveData.Read(typeof(float));
				
					rea.Message.ReceiveData.Dispose();

					serverTime = stime-DXUtil.Timer(DirectXTimer.GetApplicationTime);

					break;
				}
				case MessageType.GameChat: 
				{
					MessageUpdate update = (MessageUpdate)rea.Message.ReceiveData.Read(typeof(MessageUpdate));
				
					string s = "";

					s += rea.Message.ReceiveData.ReadString();

					rea.Message.ReceiveData.Dispose();
				
					lock (otherPlayers) 
					{
						object playerObject = otherPlayers[senderID];
						if (null == playerObject)
							return;
						RemotePlayer player = (RemotePlayer) playerObject;
						
						mainform.textBox2.Text += player.ToString() + s + "\r\n";
					}

					break;
				}

			}
		}

		public void SendMyMessage(string text) 
		{
			MessageUpdate update = new MessageUpdate();
			update.Length= text.Length;
			peer.SendPlayerMessage(update, text);
		}

		public void SendMyPlayerUpdate() 
		{
			PlayerUpdate update = new PlayerUpdate();
			update.position = position;
			update.velocity = velocity;
			peer.SendPlayerUpdate(update);
		}



	}
}
