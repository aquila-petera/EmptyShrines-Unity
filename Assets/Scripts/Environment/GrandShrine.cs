using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrandShrine : Checkpoint, IInteractable
{
    [SerializeField]
    private AbilityManager.Abilities grantedAbility;
    [SerializeField]
    private Transform cameraNode;

    new public void OnInteract()
    {
        if (!AbilityManager.HasAbility(grantedAbility))
        {
            ParticleManager.PlayParticleSystemAtPosition("Dots", transform.position, Color.yellow);

            CameraManager.LockCamera(cameraNode);

            AbilityManager.EnableAbility(grantedAbility);
            EntityManager.SetPlayerAutoChannel(3);

            TimingManager.ExecuteAfterDelay(1.5f, DoParticleEffect);
            TimingManager.ExecuteAfterDelay(2.25f, DoScreenEffect);
            TimingManager.ExecuteAfterDelay(3f, EndAnimation);
        }
        else
        {
            base.OnInteract();
        }
    }

    private void DoParticleEffect()
    {
        ParticleManager.PlayParticleSystemAtPosition("Dots", EntityManager.GetPlayerPosition(), Color.magenta);
    }

    private void DoScreenEffect()
    {
        ScreenEffectManager.FadeScreen(Color.magenta, 0.5f, ScreenEffectManager.EffectType.FLASH);
    }

    private void EndAnimation()
    {
        CameraManager.UnlockCamera();
    }
}
