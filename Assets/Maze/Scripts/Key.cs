using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Collectible
{
    public Door door;

    // override parent collect method
    // to add door interation
	new public void Collect()
	{
        base.Collect();
        door.Unlock();
    }

}
