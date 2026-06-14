using UnityEngine;

namespace BloonsTD.Units
{
    /// <summary>
    /// Driver animation 4 hướng cho hero + tower.
    /// Animator dùng blend-tree 2D theo (FaceX, FaceY) cho state Idle + Attack.
    /// AN TOÀN khi chưa gán controller / chưa có sprite frame: mọi call thành no-op,
    /// nên có thể gắn sẵn vào prefab rồi kéo art vào sau.
    ///
    /// Param Animator (do AnimationSetup sinh ra):
    ///   FaceX (float), FaceY (float)  — hướng nhìn, -1..1
    ///   Attack (trigger)              — phát 1 nhịp đánh
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class UnitAnimator : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [Tooltip("Tốc độ xoay mượt về hướng mục tiêu (cao = quay nhanh).")]
        [SerializeField] float _faceLerp = 12f;

        static readonly int FaceXHash  = Animator.StringToHash("FaceX");
        static readonly int FaceYHash  = Animator.StringToHash("FaceY");
        static readonly int AttackHash = Animator.StringToHash("Attack");

        Vector2 _face       = Vector2.down;
        Vector2 _faceTarget = Vector2.down;
        bool    _ready;

        void Awake()
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            _ready = _animator != null && _animator.runtimeAnimatorController != null;
            if (!_ready)
                Debug.LogWarning($"[UnitAnimator] {name} — chưa gán Animator/controller. Anim sẽ bỏ qua. " +
                                 "Chạy menu BloonsTD/Setup/Animation/Generate All Animators hoặc gán Anim_<Tên>.controller.");
            else
                Debug.Log($"[UnitAnimator] {name} — sẵn sàng (controller={_animator.runtimeAnimatorController.name}).");
        }

        /// <summary>Đặt hướng nhìn (sẽ lerp mượt). Vector zero → bỏ qua, giữ hướng cũ.</summary>
        public void SetFacing(Vector2 dir)
        {
            if (dir.sqrMagnitude < 0.0001f) return;
            _faceTarget = dir.normalized;
        }

        /// <summary>Phát 1 nhịp đánh (trigger). No-op nếu chưa ready.</summary>
        public void PlayAttack()
        {
            if (!_ready) return;
            _animator.SetTrigger(AttackHash);
        }

        void Update()
        {
            if (!_ready) return;
            _face = Vector2.Lerp(_face, _faceTarget, Time.deltaTime * _faceLerp);
            _animator.SetFloat(FaceXHash, _face.x);
            _animator.SetFloat(FaceYHash, _face.y);
        }
    }
}
