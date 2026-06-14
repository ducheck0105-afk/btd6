using System.Collections.Generic;
using BloonsTD.Core;
using BloonsTD.Data;
using UnityEngine;

namespace BloonsTD.Combat
{
    public class ProjectilePoolManager : SingletonMono<ProjectilePoolManager>
    {
        [SerializeField] int _defaultPoolSize = 20;

        readonly Dictionary<GameObject, ObjectPool<ProjectileBase>> _pools = new();

        public ProjectileBase Get(ProjectileData data, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(data.prefab, out var pool))
            {
                var comp = data.prefab.GetComponent<ProjectileBase>();
                pool = new ObjectPool<ProjectileBase>(comp, _defaultPoolSize, transform);
                _pools[data.prefab] = pool;
            }
            return pool.Get(position, rotation);
        }

        public void Return(GameObject prefab, ProjectileBase proj)
        {
            if (_pools.TryGetValue(prefab, out var pool))
                pool.Return(proj);
            else
                proj.gameObject.SetActive(false);
        }
    }
}
