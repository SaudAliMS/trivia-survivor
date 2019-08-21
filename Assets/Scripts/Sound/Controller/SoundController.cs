using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : SingletonMono<SoundController> {

	#region Variables And Properties

	public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    public AudioClip ReadySound;
    public AudioClip GoSound;
    public AudioClip TornadoSound;
    public AudioClip LightningSound;
    public AudioClip GlacierBreakSound;
    public AudioClip TimeUpSound;
    public AudioClip ClickSound;
    public AudioClip CoinClinkSound;

    private bool musicState = true;
	private bool soundState = true;

    private void Start()
    {
        PlayMusic();
    }

    public bool MusicState
	{
		get
		{
			return musicState;
		}
		set
		{
			musicState = value;
			SaveState ();
            PlayMusic();

        }
	}

	public bool SoundState
	{
		get
		{
			return soundState;
		}
		set
		{
			soundState = value;
			SaveState ();
		}
	}

	#endregion Variables And Properties

	#region Save/Load State

	public void SaveState()
	{
		DatabaseManager.SetBool (GameConstants.MusicState, musicState);
		DatabaseManager.SetBool (GameConstants.SoundState, soundState);
	}

	public void LoadState()
	{
		soundState = DatabaseManager.GetBool (GameConstants.SoundState, true);
		musicState = DatabaseManager.GetBool (GameConstants.MusicState, true);
	}

	#endregion Save/Load State

	#region Sfx

	public void PlaySfx(Sfx type, float volume = 1, float pitch = 1)
	{
        if (soundState) 
		{
			sfxAudioSource.volume = volume;
			sfxAudioSource.pitch = pitch;
			sfxAudioSource.PlayOneShot (GetSfxAudioClip (type));
		}
	}

	private AudioClip GetSfxAudioClip(Sfx type)
	{
		switch (type) 
		{
            case Sfx.Ready:
                return ReadySound;
            case Sfx.Go:
                return GoSound;
            case Sfx.Tornado:
                return TornadoSound;
            case Sfx.Lightning:
                return LightningSound;
            case Sfx.GlacierBreak:
                return GlacierBreakSound;
            case Sfx.TimeUp:
                return TimeUpSound;
            case Sfx.Click:
                return ClickSound;
            case Sfx.CoinClink:
                return CoinClinkSound;
            default:
				return null;
		}
	}

	public void StopSfx()
	{
		sfxAudioSource.Stop ();
	}

    #endregion Sfx

    #region Music

    public void PlayMusic()
    {
        if (musicState)
        {
            musicAudioSource.Play();
        }
        else 
        {
            musicAudioSource.Stop();
        }
    }

    #endregion Music

}
