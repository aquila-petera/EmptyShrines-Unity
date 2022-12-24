using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CharacterMovement : PhysicsEntity
{
    [Header("General")]
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float jumpSpeed = 2f;
    [SerializeField]
    private float attackRange = 0.4f;
    [SerializeField]
    private float invulnTime = 1.2f;
    [SerializeField]
    private TMP_Text channelText;
    [SerializeField]
    private int maxHp = 3;
    [SerializeField]
    private int maxKi = 10;
    [SerializeField]
    private MeditationController meditation;

    [Header("Gliding")]
    [SerializeField]
    private float baseFallSpeed = 2;
    [SerializeField]
    private float glideFallSpeed = 0.5f;
    [SerializeField]
    private ParticleSystem glideParticles;

    [Header("SFX")]
    [SerializeField]
    private AudioClip staffSwingSound;
    [SerializeField]
    private AudioClip staffHitSound;

    private Animator animator;
    private bool inControl = true;
    private bool attacking = false;
    private bool channeling = false;
    private bool focusing = false;
    private bool meditating = false;
    private bool invuln = false;
    private string channelString = "";
    private SpriteFlipper spriteFlipper;
    private IInteractable interactable;
    private int hitPoints;
    private int kiPoints;
    private AudioSource audioSource;
    private float footstepTimer;
    private float footstepInterval = 0.25f;
    private bool gliding = false;
    private float baseGravity;

    public bool HasControl()
    {
        return inControl;
    }

    public void SetControlEnabled(bool enabled)
    {
        xspd = enabled ? xspd : 0;
        inControl = enabled;
    }

    public void AutoMove(bool left, float duration = 0)
    {
        inControl = false;
        ResetState();
        xspd = left ? -moveSpeed : moveSpeed;
        animator.SetInteger("MoveSpeed", (int)xspd);
        if (duration > 0)
            TimingManager.ExecuteAfterDelay(duration, RestoreControl);
    }

    public void AutoChannel(float duration)
    {
        xspd = 0;
        inControl = false;
        ResetState();
        animator.SetBool("Channeling", true);
        inControl = false;
        TimingManager.ExecuteAfterDelay(duration, RestoreControl);
    }

    public void FloatInAir(float duration)
    {
        gravity = 0;
        yspd = 0.5f;
        TimingManager.ExecuteAfterDelay(0.5f, StopMovement);
        TimingManager.ExecuteAfterDelay(duration + 0.5f, LandFromAir);
    }

    private void LandFromAir()
    {
        yspd = -0.5f;
        TimingManager.ExecuteAfterDelay(0.5f, ResetGravity);
    }

    private void ResetGravity()
    {
        gravity = baseGravity;
    }

    public void SetInteractable(IInteractable obj)
    {
        interactable = obj;
    }

    public void TakeDamage()
    {
        if (!invuln)
        {
            invuln = true;
            attacking = false;
            ResetState();
            hitPoints--;
            EventManager.InvokeEvent(new PlayerHealthChangeEvent(1));
            if (hitPoints <= 0)
            {
                Faint();
            }
            else
            {
                animator.SetTrigger("Hurt");
                StartCoroutine(DoHurtSequence());
            }
        }
    }

    public void FullHeal()
    {
        int amt = maxHp - hitPoints;
        hitPoints = maxHp;
        EventManager.InvokeEvent(new PlayerHealthChangeEvent(-amt));
    }

    public void SetHitPoints(int hp)
    {
        hitPoints = hp;
    }

    public int GetHitPoints()
    {
        return hitPoints;
    }

    public int GetMaxHitPoints()
    {
        return maxHp;
    }

    public void SetKiPoints(int ki)
    {
        kiPoints = ki;
    }

    public bool ChangeKiPoints(int delta)
    {
        bool afford = kiPoints + delta >= 0;
        kiPoints = Mathf.Clamp(kiPoints + delta, 0, maxKi);
        EventManager.InvokeEvent(new PlayerKiChangeEvent(kiPoints, maxKi));
        return afford;
    }

    public int GetKiPoints()
    {
        return kiPoints;
    }

    public int GetMaxKiPoints()
    {
        return maxKi;
    }

    private void Faint()
    {
        animator.SetTrigger("Faint");
        CameraManager.UnlockCamera();
        inControl = false;
        xspd = 0;
        spriteFlipper.Spin();
        TimingManager.ExecuteAfterDelay(1, EntityManager.RespawnPlayer);
    }

    private void RestoreControl()
    {
        inControl = true;
        ResetState();
        xspd = 0;
        animator.SetInteger("MoveSpeed", 0);
    }

    new private void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        spriteFlipper = GetComponent<SpriteFlipper>();
        audioSource = GetComponent<AudioSource>();
        hitPoints = maxHp;
        groundSnapFlag = true;
        baseGravity = gravity;

        EntityManager.RegisterPlayer(gameObject);
        EventManager.BindEvent(typeof(ObjectHitEvent), new Action<BasicEvent>(OnHitObject));
        EventManager.BindEvent(typeof(PlayerFinishedMeditatingEvent), new Action<BasicEvent>(OnMeditationEnd));
    }

    private void OnDestroy()
    {
        EventManager.UnbindEvent(typeof(ObjectHitEvent), OnHitObject);
        EventManager.UnbindEvent(typeof(PlayerFinishedMeditatingEvent), new Action<BasicEvent>(OnMeditationEnd));
    }

    private void Update()
    {
        gliding = false;
        if (inControl && IsIdleState() && !attacking)
        {
            if (Input.GetKey(KeyCode.S))
            {
                spriteFlipper.Flip(true);
                xspd = -moveSpeed;
                animator.SetInteger("MoveSpeed", 1);
                if (grounded && footstepTimer <= 0)
                {
                    SFXManager.PlaySound(audioSource, GetFootstepSound(), true, 1, RNGManager.RandomRange(0.9f, 1.1f));
                    footstepTimer = footstepInterval;
                }
            }
            else if (Input.GetKey(KeyCode.F))
            {
                spriteFlipper.Flip(false);
                xspd = moveSpeed;
                animator.SetInteger("MoveSpeed", -1);
                if (grounded && footstepTimer <= 0)
                {
                    SFXManager.PlaySound(audioSource, GetFootstepSound(), true, 1, RNGManager.RandomRange(0.9f, 1.1f));
                    footstepTimer = footstepInterval;
                }
            }
            else
            {
                xspd = 0;
                animator.SetInteger("MoveSpeed", 0);
            }
            if (grounded && Input.GetKeyDown(KeyCode.E))
            {
                interactable?.OnInteract();
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                animator.SetTrigger("Attack");
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (grounded)
                {
                    yspd = jumpSpeed;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (AbilityManager.HasAbility(AbilityManager.Abilities.ABILITY_GLIDE))
                {
                    gliding = true;
                }
            }
        }
        else if (channeling)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.Backspace) && channelString.Length > 0)
                {
                    channelString = channelString.Substring(0, channelString.Length - 1);
                    channelText.text = channelString;
                    TextLabelManager.UpdateChannelString(channelString);
                }
                else
                {
                    for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
                    {
                        if (Input.GetKeyDown((KeyCode)i))
                        {
                            char typeChar = 'a';
                            typeChar += (char)(i - (int)KeyCode.A);
                            channelString += typeChar;
                            channelText.text = channelString;
                            TextLabelManager.UpdateChannelString(channelString);
                        }
                    }
                }
            }
        }
        else if (meditating)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                meditation.Choose(true);
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                meditation.Choose(false);
            }
        }

        if (inControl && (IsIdleState() || channeling) && Input.GetKeyDown(KeyCode.Space))
        {
            ToggleChannel(!channeling);
        }
        if (inControl && IsIdleState() && Input.GetKeyDown(KeyCode.Semicolon))
        {
            ToggleFocus(true);
        }
        if (focusing && Input.GetKeyUp(KeyCode.Semicolon))
        {
            ToggleFocus(false);
        }

        if (footstepTimer > 0)
        {
            footstepTimer -= Time.deltaTime;
        }
    }

    private void ResetState()
    {
        ToggleChannel(false);
        ToggleFocus(false);
        ToggleMeditate(false);
    }

    private bool IsIdleState()
    {
        return !channeling && !focusing && !meditating;
    }

    private void LateUpdate()
    {
        animator.SetInteger("VertSpeed", yspd == 0 ? 0 : (int)Mathf.Sign(yspd));
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Land");
        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Faint");

        if (groundSnapFlag)
        {
            TrySnapToGround();
        }
    }

    new private void FixedUpdate()
    {
        if (!grounded && yspd < 0 && gliding)
        {
            maxFallSpeed = glideFallSpeed;
            if (!glideParticles.isPlaying)
            {
                glideParticles.Play();
            }
        }
        else
        {
            maxFallSpeed = baseFallSpeed;
            glideParticles.Stop();
        }

        base.FixedUpdate();

        if ((channeling || focusing) && grounded)
        {
            xspd = 0;
        }
    }

    private AudioClip GetFootstepSound()
    {
        RaycastHit hit;
        if (Physics.BoxCast(collider.bounds.center + Vector3.up * Time.fixedDeltaTime, collider.bounds.extents, Vector3.down, out hit, Quaternion.identity, Time.fixedDeltaTime * 2, 1 << LayerMask.NameToLayer("Solid")))
        {
            FootstepSoundOverride fso = hit.collider.GetComponent<FootstepSoundOverride>();
            if (fso != null)
            {
                return fso.GetFootstepSound();
            }
        }
        return SFXManager.GetFootstepSound();
    }

    private void StartAttack()
    {
        SFXManager.PlaySound(audioSource, staffSwingSound, true);
        attacking = true;
        if (grounded)
            xspd = 0;
        int facing = spriteFlipper.IsFacingLeft() ? -1 : 1;
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, collider.size / 2, Vector3.right * facing, transform.rotation, attackRange);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                IHittable hittable = hit.collider.GetComponent<IHittable>();
                if (hittable != null)
                {
                    if (hittable.OnHit(transform.position))
                    {
                        SFXManager.PlaySound(audioSource, staffHitSound, true);
                    }
                }
            }
        }
    }

    private void StopAttack()
    {
        attacking = false;
    }

    private void ToggleChannel(bool channel)
    {
        if (channeling && !channel)
            TextLabelManager.SubmitChannelString(channelString);
        channeling = channel;
        animator.SetBool("Channeling", channel);
        channelString = "";
        channelText.text = "";
        TextLabelManager.UpdateChannelString("");
    }

    private void ToggleFocus(bool focus)
    {
        focusing = focus;
        animator.SetBool("Focusing", focus);
        if (focus)
        {
            TextLabelManager.StartTranslation();
        }
        else
        {
            TextLabelManager.StopTranslation();
        }
    }

    public void ToggleMeditate(bool meditate)
    {
        meditating = meditate;
        // Intentionally using the same focusing animator state
        animator.SetBool("Focusing", meditate);
        if (meditating)
        {
            meditation.GenerateNewPrompt();
            meditation.gameObject.SetActive(true);
        }
        else
        {
            meditation.gameObject.SetActive(false);
        }
    }

    private void OnHitObject(BasicEvent ev)
    {;
        ObjectHitEvent objectHitEvent = ev as ObjectHitEvent;
        Vector3 pos = transform.position + (objectHitEvent.hitObj.transform.position - transform.position) / 2;
        ParticleManager.PlayParticleSystemAtPosition("Stars", pos);
    }

    private void OnMeditationEnd(BasicEvent ev)
    {
        PlayerFinishedMeditatingEvent pfmEv = ev as PlayerFinishedMeditatingEvent;
        ToggleMeditate(false);
    }

    protected override void OnLand()
    {
        SFXManager.PlaySound(audioSource, GetFootstepSound(), true, 1f, 0.8f);
        animator.SetTrigger("Land");
    }

    protected override void OnCrush()
    {
        EntityManager.PlayerFall();
    }

    private IEnumerator DoHurtSequence()
    {
        float duration = invulnTime;
        float flashDelay = 0.1f;
        float timer = 0;
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        while (timer < invulnTime)
        {
            timer += Time.deltaTime;
            flashDelay -= Time.deltaTime;
            if (flashDelay <= 0)
            {
                flashDelay = 0.1f;
                sprite.enabled = !sprite.enabled;
            }
            yield return new WaitForEndOfFrame();
        }
        sprite.enabled = true;
        invuln = false;
        yield return null;
    }
}
