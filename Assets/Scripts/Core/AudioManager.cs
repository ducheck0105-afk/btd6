using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý toàn bộ âm thanh game. Singleton, sống xuyên scene (DontDestroyOnLoad).
/// Clip load lazy từ Resources/Audio/{tên} và cache lại.
///
/// Gọi ở đâu:
///   AudioManager.instance.PlayButton();       // click UI  → Button.mp3
///   AudioManager.instance.PlayPlaceUnit();     // đặt unit  → Dat nhan vat.mp3
///   AudioManager.instance.PlayAttack();        // tower bắn → Attack.mp3
///   AudioManager.instance.PlayMenuMusic();     // BGM menu  → Nhac chinh.mp3
///   AudioManager.instance.PlayIngameMusic();   // BGM chơi  → Nhac ingame.mp3
///
/// File mp3 phải nằm trong Assets/Resources/Audio/ (xem SETUP_GUIDE).
/// </summary>
public class AudioManager : SingletonMono<AudioManager>
{
    // ── Tên clip = tên file (không đuôi) trong Resources/Audio/ ──
    public const string MenuMusic   = "Nhac chinh";
    public const string IngameMusic = "Nhac ingame";
    public const string SfxButton   = "Button";
    public const string SfxPlace    = "Dat nhan vat";
    public const string SfxAttack   = "Attack";

    const string ResourceFolder = "Audio/";
    const string PrefMusicVol    = "audio_music_vol";
    const string PrefSfxVol       = "audio_sfx_vol";

    AudioSource _musicSource;
    AudioSource _sfxSource;

    readonly Dictionary<string, AudioClip> _cache = new();
    string _currentMusic;

    public float MusicVolume { get; private set; } = 1f;
    public float SfxVolume   { get; private set; } = 1f;

    protected override void Init()
    {
        // Sống xuyên scene (menu → gameplay không bị cắt nhạc / mất instance)
        DontDestroyOnLoad(gameObject);

        MusicVolume = PlayerPrefs.GetFloat(PrefMusicVol, 1f);
        SfxVolume   = PlayerPrefs.GetFloat(PrefSfxVol, 1f);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop        = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume      = MusicVolume;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop        = false;
        _sfxSource.playOnAwake = false;
        _sfxSource.volume      = SfxVolume;

        Debug.Log($"[AudioManager] Init — musicVol={MusicVolume:F2} sfxVol={SfxVolume:F2}");
    }

    // ── Load + cache ──────────────────────────────────────────────────────
    AudioClip GetClip(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogError("[AudioManager] GetClip — clipName rỗng.");
            return null;
        }
        if (_cache.TryGetValue(clipName, out var cached)) return cached;

        var clip = Resources.Load<AudioClip>(ResourceFolder + clipName);
        if (clip == null)
            Debug.LogError($"[AudioManager] Không tìm thấy clip '{clipName}' — kiểm tra file Assets/Resources/{ResourceFolder}{clipName}.mp3 (tên phải khớp, không đuôi). Xem SETUP_GUIDE.");
        _cache[clipName] = clip; // cache cả null để khỏi Load lại liên tục
        return clip;
    }

    // ── Music ─────────────────────────────────────────────────────────────
    public void PlayMusic(string clipName)
    {
        if (_currentMusic == clipName && _musicSource.isPlaying) return; // đang phát rồi → bỏ qua

        var clip = GetClip(clipName);
        if (clip == null) return;

        _currentMusic     = clipName;
        _musicSource.clip = clip;
        _musicSource.Play();
        Debug.Log($"[AudioManager] PlayMusic '{clipName}'");
    }

    public void StopMusic()
    {
        _musicSource.Stop();
        _currentMusic = null;
    }

    // ── SFX ───────────────────────────────────────────────────────────────
    public void PlaySfx(string clipName)
    {
        var clip = GetClip(clipName);
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, SfxVolume);
    }

    // ── Semantic helpers (dùng ở call-site cho rõ nghĩa) ────────────────────
    public void PlayMenuMusic()   => PlayMusic(MenuMusic);
    public void PlayIngameMusic() => PlayMusic(IngameMusic);
    public void PlayButton()      => PlaySfx(SfxButton);
    public void PlayPlaceUnit()   => PlaySfx(SfxPlace);
    public void PlayAttack()      => PlaySfx(SfxAttack);

    // ── Volume (dùng cho settings panel sau này) ────────────────────────────
    public void SetMusicVolume(float v)
    {
        MusicVolume = Mathf.Clamp01(v);
        _musicSource.volume = MusicVolume;
        PlayerPrefs.SetFloat(PrefMusicVol, MusicVolume);
    }

    public void SetSfxVolume(float v)
    {
        SfxVolume = Mathf.Clamp01(v);
        PlayerPrefs.SetFloat(PrefSfxVol, SfxVolume);
    }
}
