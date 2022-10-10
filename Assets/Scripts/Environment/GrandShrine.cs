using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandShrine : Checkpoint, IInteractable
{
    [SerializeField]
    private AbilityManager.Abilities grantedAbility;
    [SerializeField]
    private Transform cameraNode;
    [SerializeField]
    private ParticleSystem shrineParticles;
    [SerializeField]
    private ParticleSystem playerParticles;
    [SerializeField]
    private AudioClip getAbilityBGM;
    new public void OnInteract()
    {
        if (!AbilityManager.HasAbility(grantedAbility))
        {
            shrineParticles.Play();
            hintText.text = "";

            CameraManager.LockCamera(cameraNode);
            ScreenEffectManager.FlashScreen(new Color32(0xAD, 0xAA, 0x0F, 0xFF), 1);

            AbilityManager.EnableAbility(grantedAbility);
            EntityManager.SetPlayerAutoChannel(8);
            MusicManager.PlayCustomBGM(getAbilityBGM, 0.25f, 0.25f, false);

            TimingManager.ExecuteAfterDelay(0.5f, FloatPlayer);
            TimingManager.ExecuteAfterDelay(6.5f, DoScreenEffect);
            TimingManager.ExecuteAfterDelay(8f, EndAnimation);
        }
        else
        {
            base.OnInteract();
        }
    }

    private void FloatPlayer()
    {
        playerParticles.Play();
        playerParticles.transform.position = EntityManager.GetPlayerPosition();
        playerParticles.transform.SetParent(EntityManager.GetPlayerTransform());
        EntityManager.DoPlayerGetAbilityCutscene();
    }

    private void DoScreenEffect()
    {
        playerParticles.Stop();
        ScreenEffectManager.FlashScreen(Color.magenta, 0.5f);
    }

    private void EndAnimation()
    {
        shrineParticles.Stop();
        playerParticles.transform.SetParent(transform);
        CameraManager.UnlockCamera();
        MusicManager.PlayMapBGM(0.25f);
        ScreenEffectManager.ShowTutorial();
        hintText.text = "[E] Rest";
    }
}
