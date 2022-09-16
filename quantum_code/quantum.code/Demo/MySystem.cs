using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantum
{
    public unsafe class MySystem:SystemMainThreadFilter<MySystem.Filter>,ISignalOnPlayerDataSet
    {
        public void OnPlayerDataSet(Frame f, PlayerRef player)
        {
            var data = f.GetPlayerData(player);
            var prototype = f.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);
            var e=f.Create(prototype);
            if (f.Unsafe.TryGetPointer<PlayerLink>(e, out var pl))
            {
                pl->Player= player;
                f.Add(e, pl);
            }
            
            if (f.Unsafe.TryGetPointer<Transform3D>(e, out var t))
            {
                t->Position.X = 0 + player;

            }
        }

        public override void Update(Frame f, ref Filter filter)
        {
            var  input =f.GetPlayerInput(filter.link->Player);
            if (input->Jump.WasPressed)
            {
                filter.KCC->Jump(f);

            }
            filter.KCC->Move(f, filter.Entity, input->Direction.XOY);
            if(input->Direction != default)
            {
                filter.Transform->Rotation = Photon.Deterministic.FPQuaternion.LookRotation(input->Direction.XOY);

            }
        }

        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* KCC;
            public Transform3D* Transform;
            public PlayerLink* link;
        }
    }
}
