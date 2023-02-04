using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class Items
{
	public static GameObject[] items;

	static Items()
	{
		items = new[]
		{
            //Resources.Load<GameObject>("Prefabs/Kazik"),
            Resources.Load<GameObject>("Prefabs/Eiffel"),
			Resources.Load<GameObject>("Prefabs/Pisa"),
			Resources.Load<GameObject>("Prefabs/StatueOfLiberty"),
			Resources.Load<GameObject>("Prefabs/Pillar"),
			Resources.Load<GameObject>("Prefabs/Spaceship"),
			Resources.Load<GameObject>("Prefabs/BurjKhalifa"),
		};
	}
}