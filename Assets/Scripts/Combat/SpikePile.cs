using BloonsTD.Data;
using BloonsTD.Units.Enemy;
using BloonsTD.Wave;
using UnityEngine;

namespace BloonsTD.Combat
{
    /// <summary>
    /// Đặt trên path, enemy đi qua nhận damage và tiêu thụ pierce.
    /// Despawn khi hết pierce hoặc cuối round.
    /// </summary>
    public class SpikePile : MonoBehaviour
    {
        int       _pierceLeft;
        float     _damage;
        TowerType _attackerType;

        public void Init(int pierceCount, float damage, TowerType attacker)
        {
            _pierceLeft   = pierceCount;
            _damage       = damage;
            _attackerType = attacker;

            if (WaveManager.instance != null)
                WaveManager.instance.OnRoundEnded += OnRoundEnded;

            Debug.Log($"[SpikePile] Khởi tạo tại {transform.position} — pierce={_pierceLeft} dmg={_damage}");
        }

        void OnDestroy()
        {
            if (WaveManager.instance != null)
                WaveManager.instance.OnRoundEnded -= OnRoundEnded;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.TryGetComponent<EnemyController>(out var ec) || ec.IsDead)
            {
                Debug.Log("0231940329");
                return;
            }
            DamageSystem.Apply(ec, _damage, isExplosion: false, isMagic: false, attacker: _attackerType);
            _pierceLeft--;
            Debug.Log($"[SpikePile] Trúng {ec.Data.enemyName} — pierce còn {_pierceLeft}");
            if (_pierceLeft <= 0) Despawn();
        }

        void OnRoundEnded(int _) => Despawn();

        void Despawn()
        {
            if (WaveManager.instance != null)
                WaveManager.instance.OnRoundEnded -= OnRoundEnded;
            Destroy(gameObject);
        }
    }
}
