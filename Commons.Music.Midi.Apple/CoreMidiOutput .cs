﻿using System;
using CoreMidi;
using System.Threading.Tasks;

#if __MOBILE__
namespace Commons.Music.Midi.iOS
#else
namespace Commons.Music.Midi.macOS
#endif
{
    public class CoreMidiOutput : IMidiOutput
	{
		public CoreMidiOutput(IMidiPortDetails details)
		{
			this.details = details;
			client = new MidiClient("outputclient");
			port = client.CreateOutputPort("outputport");
			Connection = MidiPortConnectionState.Open;
		}

		protected CoreMidiOutput()
        {

        }

		protected IMidiPortDetails details;
		private MidiClient client;
		private MidiPort port;

		public IMidiPortDetails Details => details;

		public MidiPortConnectionState Connection { get; private set; }

		public Task CloseAsync()
		{
			if(details is CoreMidiPortDetails cmDetails)
            {
				port.Disconnect(cmDetails.Endpoint);
				cmDetails.Dispose();
			}
			port.Dispose();
			client.Dispose();
			Connection = MidiPortConnectionState.Closed;
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			CloseAsync().Wait();
		}

		MidiPacket[] arr = new MidiPacket[1];
		public virtual void Send(byte[] mevent, int offset, int length, long timestamp)
		{
			if (details is CoreMidiPortDetails cmDetails)
			{
				unsafe
				{
					fixed (byte* ptr = mevent)
					{
						arr[0] = new MidiPacket(timestamp, (ushort)length, (IntPtr)(ptr + offset));
						port.Send(cmDetails.Endpoint, arr);
					}
				}
			}
		}
	}
}