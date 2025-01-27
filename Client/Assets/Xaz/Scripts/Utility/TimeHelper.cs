//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xaz
{
	
	static public class TimeHelper
	{
		static readonly private DateTime m_StandardTime;

		static TimeHelper()
		{
			m_StandardTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
		}

		static private bool m_Synced;
		static private DateTime m_LocalTimeAtSync;
		static private DateTime m_RemoteTimeAtSync;

		static public DateTime now
		{
			get
			{
				DateTime now = DateTime.UtcNow;
				if (m_Synced) {
					var deltaTime = now - m_LocalTimeAtSync;
					now = m_RemoteTimeAtSync + deltaTime;
				}
				return now;
			}
		}

		static public int timestamp
		{
			get
			{
                return Convert.ToInt32(Math.Floor(totalSeconds));
			}
		}

		static public int hours
		{
			get
			{
				return (now - m_StandardTime).Hours;
			}
		}

		static public double totalHours
		{
			get
			{
				return (now - m_StandardTime).TotalHours;
			}
		}

		static public int minutes
		{
			get
			{
				return (now - m_StandardTime).Minutes;
			}
		}

		static public double totalMinutes
		{
			get
			{
				return (now - m_StandardTime).TotalMinutes;
			}
		}

		static public int seconds
		{
			get
			{
				return (now - m_StandardTime).Seconds;
			}
		}

		static public double totalSeconds
		{
			get
			{
				return (now - m_StandardTime).TotalSeconds;
			}
		}

		static public int milliseconds
		{
			get
			{
				return (now - m_StandardTime).Milliseconds;
			}
		}

		static public double totalMilliseconds
		{
			get
			{
				return (now - m_StandardTime).TotalMilliseconds;
			}
		}

		static public int dayOfWeek
		{
			get
			{
				return ((int)now.DayOfWeek == 0) ? 7 : (int)now.DayOfWeek;;
			}
		}

		static public void Sync(double timestamp)
		{
			m_Synced = true;
			m_LocalTimeAtSync = DateTime.UtcNow;
			m_RemoteTimeAtSync = m_StandardTime.AddSeconds(timestamp);
		}
	}
}
