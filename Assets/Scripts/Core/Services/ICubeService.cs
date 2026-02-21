using System.Collections.Generic;
using UnityEngine;
using Game.Gameplay.Cubes;

namespace Game.Core.Services
{
    public interface ICubeService
    {
        Cube ActiveCube { get; }
        void InitializeGameplay(Transform spawnPoint);
        void ResetCubes();
        Cube GetCubeById(int id);
        Dictionary<int, Cube> GetActiveCubes();
        void RemoveCube(int id);
    }
}
