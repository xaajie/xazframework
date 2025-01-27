//------------------------------------------------------------
// Xaz Framework
// Feedback: qq515688254
//------------------------------------------------------------
﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xaz
{
	sealed public class UIComponentCollection : MonoBehaviour
	{
		[SerializeField]
		internal List<Component> components = new List<Component>();

		public T Get<T>(int index) where T : Component
		{
			return (T)components[index];
		}
	}
}
