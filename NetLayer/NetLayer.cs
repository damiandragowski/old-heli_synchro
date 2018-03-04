using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectPlay;
using System.Reflection;
using DXGfx;
using Core;
using Heli;


namespace NetLayer
{
	/// <summary>
	/// Summary description for NetLayer.
	/// </summary>
	public class NetLayer:Core.ILayer
	{
		public  SynchFactory factory = null;
		public  int bAppState = 0;
		bool	bSingle = true;
		private int interval=10000;

		private NETPlayer peer=null;
		private Hashtable	otherPlayers = new Hashtable();
		public Timer timer = new System.Windows.Forms.Timer();
		public Timer timer2 = new System.Windows.Forms.Timer();	

		private int ID;
		private string Name;
		public  float atime=0;
		private float lastTime=0;
		private int timeCount=0;
		private const int TC=3;
		private GameLevel curLevel=null;
		private Heli.Helicopter heli=null;

		private void TimerEvent(Object myObject, EventArgs myEventArgs)
		{
			peer.SendTimeUpdate(atime,0);
			timeCount=1;
			timer2.Start();
		}

		private void TimerEvent2(Object myObject, EventArgs myEventArgs)
		{
			if ( timeCount == TC )
				timer2.Stop();
			else
				peer.SendTimeUpdate(atime,timeCount);
			timeCount++;
		}

		public NetLayer(GameLevel _curLevel, Heli.Helicopter h)
		{
			//HeliSynchStru update = new HeliSynchStru();
			//Type tt = update.GetType();
			//FieldInfo [] ff = tt.GetFields();
			//foreach( FieldInfo f in ff ) 
			//{
			//	f.SetValue(update, new Vector3(1,2,3));
			//}

	
			curLevel = _curLevel;
			heli=h;
			//
			// TODO: Add constructor logic here
			//
			SynchFactory.CreateSynchFactory();
			factory = SynchFactory.Instance;
		}
		public void Initialize()
		{
			SingMultiDialog form = new SingMultiDialog();
			form.ShowDialog();
			bAppState = form.retValue;
			if ( bAppState == 2 ) 
			{
				bSingle = false;
				peer = new NETPlayer(this);
				if (peer.InSession) 
					if (peer.IsHost) 
					{
						timer.Tick += new EventHandler(TimerEvent);
						timer.Interval = interval;
						timer.Start();
						timer2.Tick += new EventHandler(TimerEvent2);
						timer2.Interval = interval;
					
					}
			}
			
		}
		public void Update(float elapsedtime)
		{
			if ( !bSingle ) 
			{
				if ( lastTime > 0.1 ) // 100 ms 
				{
					// send update of me.
					lastTime=0;
					if (peer.InSession  && otherPlayers.Count > 0)
						SendMyPlayerUpdate();

				} else 
					lastTime+=elapsedtime;
				// factory interpolate
				factory.Interpolate(elapsedtime);

			}
			atime+=elapsedtime;
			DxControl.GfxLayer.AddDebugInfo("Time:", ""+atime);
		}

		public void SendMyPlayerUpdate() 
		{
			Heli.HeliSynchStru update = (Heli.HeliSynchStru)heli.objectToSynch;
			((Heli.HeliSynchStru)heli.objectToSynch).time = atime;

			/*
			Type type = Type.GetType("HeliSynchStru");
			foreach( FieldInfo i in  type.GetFields())
			{
				
			}*/
			peer.SendPlayerUpdate(update);
		}

		public int Interval
		{
			get
			{
				return interval;
			}
			set
			{
				interval = value;
			}
		}

		public void Close()
		{
			if (peer.InSession) 
				if (peer.IsHost) 
				{
					peer.SendShutDown();
				}
		}
		public void PlayerCreated(object sender, PlayerCreatedEventArgs pcea) 
		{
			Peer peer1 = (Peer) sender;
			int playerID = pcea.Message.PlayerID;
			PlayerInformation playerInfo = peer1.GetPeerInformation(playerID);


			if (playerInfo.Local) 
			{
				ID = playerID;
				Name = playerInfo.Name;
			}
			else 
			{	
				Heli.Helicopter h=new Heli.Helicopter(playerInfo.Name+playerID,@"dane\heliData.xml", true);
				RemotePlayer newPlayer = new RemotePlayer(playerID, playerInfo.Name, h);

				
				curLevel.AddObject(h);
				
				factory.InsertObject(h);
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
						object playerObject = otherPlayers[PlayerID];
						if (null == playerObject)
							return;
						RemotePlayer player = (RemotePlayer) playerObject;
						factory.RemoveObject(player.heli);
						curLevel.RemoveObject(player.heli);
						player.heli = null;
						otherPlayers.Remove(PlayerID);
				}
			}
		}
		private float [] timetable = new float[TC];

		public void DataReceived(object sender, ReceiveEventArgs rea) 
		{
			int senderID = rea.Message.SenderID;
			
			byte mType = (byte)rea.Message.ReceiveData.Read(typeof(byte));
			MessageType messageType = (MessageType)mType;
			switch (messageType) 
			{
				case MessageType.GameTime: 
				{	

					float stime = (float)rea.Message.ReceiveData.Read(typeof(float));
					int	  timeCount = (int)rea.Message.ReceiveData.Read(typeof(int));
				
					rea.Message.ReceiveData.Dispose();

					if( timeCount == 0 ) 
						atime = stime;
					else 
					{
						timetable[timeCount-1]=atime-stime;
					}
					if ( timeCount == TC ) 
					{
						float delta=0;
						for ( int i=0;i<TC-1;++i )
							delta+=timetable[i];
						delta = delta/(float)(TC-1);
					}

					break;
				}
				case MessageType.ServerDown:
				{
					MessageBox.Show("Server is Down. bye bye");
					Application.Exit();
					break;
				}
				case MessageType.PlayerUpdatePosition: 
				{	

					HeliSynchStru temp = new HeliSynchStru();
					Type type = temp.GetType();
					FieldInfo [] fields = type.GetFields();
					foreach( FieldInfo im in  fields)
					{
						Type tt = im.FieldType;
						object tak = (object)rea.Message.ReceiveData.Read(tt); 
						im.SetValue(temp, tak);


					}
					rea.Message.ReceiveData.Dispose();

					lock (otherPlayers) 
					{
						object playerObject = otherPlayers[senderID];
						if (null == playerObject)
							return;
						RemotePlayer player = (RemotePlayer) playerObject;

						temp.position = (atime-temp.time)*temp.velocity + temp.position;

						temp.pmatrix.M41=temp.position.X;
						temp.pmatrix.M42=temp.position.Y;
						temp.pmatrix.M43=temp.position.Z;

						player.heli.objectToSynch = temp;
						player.UpdateTime = DateTime.Now;
						player.Active = true;

						otherPlayers[senderID] = player;
					}

					break;
				}
			}
		}
	}
}
