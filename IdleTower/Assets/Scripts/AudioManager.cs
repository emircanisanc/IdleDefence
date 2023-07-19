using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://pixabay.com/users/universfield-28281460/?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=144746 UNIVERSFIELD

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] AudioClip defaultUISound;
    [SerializeField] AudioClip gunActiveSound;
    [SerializeField] AudioClip upgradeSound;
    [SerializeField] AudioClip gameWin;
    [SerializeField] AudioClip gameLose;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] AudioClip towerDamageSound;
    [SerializeField] AudioClip enemyDamageSound;
    [SerializeField] AudioClip towerExplosionSound;

    public void PlayClipAtCamera(AudioClip clip)
    {
        if (!clip)
            return;

        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    public void PlayTowerDamageSound(Vector3 position)
    {
        PlayClip(position, towerDamageSound);
    }
    
    public void PlayTowerExplosionSound(Vector3 position)
    {
        PlayClip(position, towerExplosionSound);
    }

    public void PlayEnemyDieSound(Vector3 position)
    {
        PlayClip(position, enemyDamageSound);
    }

    public void PlayClip(Vector3 position, AudioClip clip)
    {
        if (!clip)
            return;

        AudioSource.PlayClipAtPoint(clip, position);
    }

    public void PlayDefaultUISound()
    {
        PlayClipAtCamera(defaultUISound);
    }

    public void PlayUpgradeSound(Vector3 position)
    {
        PlayClip(position, upgradeSound);
    }

    public void PlaySetGunActiveSound(Vector3 position)
    {
        PlayClip(position, gunActiveSound);
    }

    public void PlayEndGame(bool win)
    {
        if (win)
            PlayClipAtCamera(gameWin);
        else
            PlayClipAtCamera(gameLose);
    }

    public void PlayExplosionSound(Vector3 position)
    {
        PlayClip(position, explosionSound);
    }
}
