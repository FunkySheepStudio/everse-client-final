using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FunkySheep.Buildings.States
{
    [CreateAssetMenu(menuName = "FunkySheep/Buildings/States/Downloading")]
    public class Downloading : FunkySheep.States.State
    {
        public FunkySheep.Types.String url;
    }
}
