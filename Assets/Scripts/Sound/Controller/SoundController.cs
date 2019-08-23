using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : SingletonMono<SoundController> {

	#region Variables And Properties

	public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    public AudioClip readySound;
    public AudioClip goSound;
    public AudioClip tornadoSound;
    public AudioClip lightningSound;
    public AudioClip glacierBreakSound;
    public AudioClip sharkDeathSound;
    public AudioClip waveDeathSound;
    public AudioClip correctAnswerSound;
    public AudioClip timeUpSound;
    public AudioClip clickSound;
    public AudioClip coinClinkSound;
    public AudioClip waterSplashSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip newQuestionSound;
    public AudioClip[] screams;

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
                return readySound;
            case Sfx.Go:
                return goSound;
            case Sfx.Tornado:
                return tornadoSound;
            case Sfx.Lightning:
                return lightningSound;
            case Sfx.GlacierBreak:
                return glacierBreakSound;
            case Sfx.TimeUp:
                return timeUpSound;
            case Sfx.Click:
                return clickSound;
            case Sfx.CoinClink:
                return coinClinkSound;
            case Sfx.WaterSplash:
                return waterSplashSound;
            case Sfx.Win:
                return winSound;
            case Sfx.Lose:
                return loseSound;
            case Sfx.NewQuestion:
                return newQuestionSound;
            case Sfx.Correct:
                return correctAnswerSound;
            case Sfx.SharkDeath:
                return sharkDeathSound;
            case Sfx.WaveDeath:
                return waveDeathSound;
            case Sfx.Scream:
                return screams[Random.Range(0,screams.Length-1)];
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

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    #endregion Music

}
