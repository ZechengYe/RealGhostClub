using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum BattleState 
{
    START,
    HOSTTURN, HOSTMENU,
    SUMMONTURN, SUMMONMENU,
    DIRECTORTURN, DIRECTORMENU,
    SOUNDTURN, SOUNDMENU,
    CAMERATURN, CAMERAMENU,
    INTERNTURN, INTERNMENU,
    BOSSTURN,
    WON, LOST
}

public class BattleSystem : MonoBehaviour
{
    #region Declaration of variables
    public bool DamageMonitorOn = false;
    public BattleState state;

    public GameObject hostPrefab;
    public GameObject summonPrefab;
    public GameObject directorPrefab;
    public GameObject soundPrefab;
    public GameObject cameraPrefab;
    public GameObject internPrefab;
    public GameObject bossPrefab;
    
    //we can seperate the slider code later?
    public Slider SoundInspiration;
    
    //health sliders
    
    public Slider bossHealth01;
    public Slider bossHealth02;
    public Slider bossHealth03;
    public Slider bossHealth04;
    
    public Slider bossSpirit01;
    public Slider bossSpirit02;
    public Slider bossSpirit03;
    public Slider bossSpirit04;

    public Slider TeamHealth;
    public Slider SummonHealth;

    private IEnumerator interncoroutine;

    public Transform hostBattleStation;
    public Transform summonBattleStation;
    public Transform directorBattleStation;
    public Transform soundBattleStation;
    public Transform cameraBattleStation;
    public Transform internBattleStation;
    public Transform bossBattleStation;

    Unit hostUnit;
    Unit summonUnit;
    Unit directorUnit;
    Unit soundUnit;
    Unit cameraUnit;
    Unit internUnit;
    Unit bossUnit;
    
    //Cameras for Cameraman
    public CinemachineVirtualCamera vCam1;
    public CinemachineVirtualCamera vCam2;
    bool vCam1On = true;
    
    //Music for Soundguy
    public AudioSource MusicSource;//The AudioSource component for playing music
    public List<AudioClip> songs = new List<AudioClip>(); // List of songs to play

    public float fadeTime = 0.05f; //Duration for the fade effect, Ezreal made it 0.05
    
    private int currentSongIndex = -1; //Keeps track of the currently playing song
    public int songPlaying = 0;
    
    bool inspirationFull = false;
    private bool isGamePaused = false;
    #endregion
    private bool isTurnStart = true;
    private bool canUseAlpha1 = true; 

    //I put the shared health management system here instead of unit, so it's more manageable
    public int teamphysicalHP = 1000;
    public int maxTeamPhysicalHP = 1000;
    public int teamMagicalHP = 500;
    public int maxTeamMagicalHP = 500;
    
    private string uiMessage; 
    
    //shaking camera 
    // How long the object should shake for.
    public float shakeDuration = 0f;
    public float decreaseFactor = 1.0f;

    public bool shaketrue= false;

    //vfx sfx animations
    public GameObject vfxAnimations;
    public AudioSource sfxAudioSource;
    public AudioClip hostClip01;
    public AudioClip hostClip02;
    public AudioClip hostComboClip;
    public AudioClip summon01;
    public AudioClip summon02;
    public AudioClip director01;
    public AudioClip Camera;
    public AudioClip Sound;
    public AudioClip Intern01;
    public AudioClip Intern02;
    public AudioClip singlehit;


    public AudioClip Boss01;
    public AudioClip Boss02;

    public AudioClip BossHit01;
    public AudioClip BossHit02;

    public GameObject DirectorBoard;
    public GameObject winUI;
    public GameObject loseUI;

    //testing
    public TextMeshProUGUI testingPrompts;
    public bool TakeSharedPhysicalDamage(int physicalDmg)
    {
        teamphysicalHP -= physicalDmg;
        if (teamphysicalHP <= 0)
        {
            teamphysicalHP = 0;
            EndBattle();
            return true; // Indicates death
        }
        return false;
    }
    public bool TakeSharedMagicalDamage(int magicalDmg)
    {
        teamMagicalHP -= magicalDmg;
        if (teamMagicalHP <= 0)
        {
            teamMagicalHP = 0;
            EndBattle();
            return true; // Indicates death
        }
        return false;
    }
    
    public static bool bossWeak = false;
    //maybe different music will have different boost such as double magic damage, etc
    public SpriteRenderer bossSpriteRenderer;
    
    void Start()
    {
        //this function show the name of all connected controllers
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            Debug.Log(Gamepad.all[i].name);
        }

        //set game state
        state = BattleState.START;
        Setup();
        
        // Optionally, start playing the initial song here
        if (songs.Count > 0)
        {
            ChangeSong(0); // Start with the first song
        }
        
    }

    void Update()
    {
        switch (state)
        {
            //connect everyone's turn and branch
            //add actual damage monitor and function into new state machine
            case BattleState.HOSTTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Host's turn, Press Q/leftShoulder to open skill tree");
                    GameManager.instance.turnStatus = TurnStatus.HostTurn;
                    isTurnStart = false;
                }

                if(Input.GetKeyDown(KeyCode.Q) || Gamepad.all[0].leftShoulder.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;
                
            case BattleState.HOSTMENU:
                if (isTurnStart)
                {
                    UpdateUIText("left to use skill 1,right to use skill2, Left & Right Shoulder Combo when inspiration full");
                    GameManager.instance.turnStatus = TurnStatus.HostBranch;
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    HostSkill_A();
                }
                
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    HostSkill_B();
                }

                if (Gamepad.all[0].leftShoulder.isPressed && Gamepad.all[0].rightShoulder.isPressed)
                {
                    HostComboAttack();
                }
                break;

            case BattleState.SUMMONTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Summon's turn, Press E/rightshoulder to open skill tree");
                    GameManager.instance.turnStatus = TurnStatus.SummonTurn;
                    isTurnStart = false;
                }
                //just for now...to check if things work
                if(Input.GetKeyDown(KeyCode.E) || Gamepad.all[0].rightShoulder.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.SUMMONMENU:
                if (isTurnStart)
                {
                    UpdateUIText("Summon Press 1/left to use skill 1, Press 2/right to use skill2");
                    GameManager.instance.turnStatus = TurnStatus.SummonBranch;
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    SummonSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    SummonSkill_B();
                }
                break;

            case BattleState.DIRECTORTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Director's turn, Press W/triangle to open skill tree");
                    GameManager.instance.turnStatus = TurnStatus.DirectorTurn;
                    isTurnStart = false;
                }
                //just for now...to check if things work
                if(Input.GetKeyDown(KeyCode.W) || Gamepad.all[0].buttonNorth.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.DIRECTORMENU:
                if (isTurnStart)
                {
                    UpdateUIText("Director Press 1/left or 2/right to attack");
                    GameManager.instance.turnStatus = TurnStatus.DirectorBranch;
                    isTurnStart = false;
                    canUseAlpha1 = true;
                }
                if ((Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed) && canUseAlpha1)
                {
                    DirectorSkill_A();  
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)//it's the pause switch
                {
                    if(!isGamePaused)
                    {
                        Time.timeScale = 0f;
                        isGamePaused = true;
                        canUseAlpha1 = false;
                        DirectorBoard.SetActive(true);
                        sfxAudioSource.clip = director01;
                        sfxAudioSource.Play();
                        UpdateUIText("Paused, Press 3/triangle again to proceed");
                        //you can put the pause UI here
                    }
                }
                //the player has to press 3 to proceed to next stage
                if ((Input.GetKeyDown(KeyCode.Alpha3) || Gamepad.all[0].buttonNorth.isPressed) && isGamePaused)
                {   
                    //resume the game & does something such as stop boss behavior 
                    Time.timeScale = 1.0f;
                    isGamePaused = false;
                    DirectorBoard.SetActive(false);
                    sfxAudioSource.clip = director01;
                    sfxAudioSource.Play();
                    UpdateUIText("Game Resumed");
                    
                    //we only need transition function here, but I also want it to do some damage check or functions, transition included in this function
                    DirectorSkill_B();
                }       
                break;
            
            case BattleState.SOUNDTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Sound's turn, Press S/circle to open skill tree");
                    GameManager.instance.turnStatus = TurnStatus.SoundTurn;
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.S) || Gamepad.all[0].buttonEast.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.SOUNDMENU:
                if (isTurnStart)
                {
                    UpdateUIText("Sound, Press 1/left & 2/right to attack");
                    GameManager.instance.turnStatus = TurnStatus.SoundBranch;
                    
                    if (inspirationFull)
                    {
                        UpdateUIText("Inspiration Full, press 2/right to use unique skill");
                    }
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    SoundSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    SoundSkill_B();
                    Debug.Log("dpad pressed");
                }
                break;

            case BattleState.CAMERATURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Camera's turn, Press C/square to open skill tree");
                    GameManager.instance.turnStatus = TurnStatus.CameraTurn;
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.C) || Gamepad.all[0].buttonWest.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.CAMERAMENU:
                if (isTurnStart)
                {
                    UpdateUIText("It's Camera's turn, Press 1/left & 2/right to use skills");
                    GameManager.instance.turnStatus = TurnStatus.CameraBranch;
                    isTurnStart = false;
                }
                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    CameraSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    CameraSkill_B();
                }
                break;
                
            case BattleState.INTERNTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Intern's turn, Press I/ X Button to open skill tree");
                    GameManager.instance.turnStatus = TurnStatus.InternTurn;
                    isTurnStart = false;
                }
                if(Input.GetKeyDown(KeyCode.I) || Gamepad.all[0].buttonSouth.isPressed)
                {
                    TransitionToNextTurn();
                }    
                break;

            case BattleState.INTERNMENU:
                if (isTurnStart)
                {
                    UpdateUIText("It's Intern's turn, Press 1/left & 2/right to use skills");
                    GameManager.instance.turnStatus = TurnStatus.InternBranch;
                    isTurnStart = false;
                }

                if (Input.GetKeyDown(KeyCode.Alpha1) || Gamepad.all[0].dpad.left.isPressed)
                {
                    InternSkill_A();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) || Gamepad.all[0].dpad.right.isPressed)
                {
                    InternSkill_B();
                }
                break;

            case BattleState.BOSSTURN:
                if (isTurnStart)
                {
                    UpdateUIText("It's Boss's turn");
                    isTurnStart = false;
                    
                    int skillChoice = Random.Range(0, 4);

                    //boss has 4 skills and it's randomly chosen
                    switch(skillChoice)
                    {
                        case 0:
                            BossSkill_A();
                            break;
                        case 1:
                            BossSkill_B();
                            break;
                        case 2:
                            BossSkill_C();
                            break;
                        case 3:
                            BossSkill_D();
                            break;
                    }
                }
                break;

            case BattleState.WON:
                UpdateUIText("You Win The JRPG Battle!");
                GameManager.instance.turnDisplay.enabled = false;
                break;

            case BattleState.LOST:
                UpdateUIText("You LOST!");
                GameManager.instance.turnDisplay.enabled = false;
                break;
        }
        if (shaketrue)
        {
            if (shakeDuration > 0) {
                vCam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2f;
                vCam2.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 2f;
                shakeDuration -= Time.deltaTime * decreaseFactor;
            } else
            {
                shakeDuration = 1f;
                vCam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
                vCam2.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
                shaketrue = false;
            }
        }

        if (inspirationFull == true)
        {
            Unit soundUnit = GameManager.instance.sound.GetComponent<Unit>();
            int inspiration = soundUnit.inspirationBar;
            SoundInspiration.value = inspiration;
        }
        else
        {
            SoundInspiration.value = 0;
        }
        //team health bar

        updateBossHealth();
        
        
    }

#region Character Skills
    void HostSkill_A()
    {
        hostAttacking();
        vfxAnimations.GetComponent<Animator>().Play("HostAttack01");
        sfxAudioSource.clip = hostClip01;
        sfxAudioSource.Play();

        Debug.Log("Host Damaged Boss by Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(hostUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(hostUnit.magicalDamage);

       

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    void HostSkill_B()
    {
        hostAttacking();
        vfxAnimations.GetComponent<Animator>().Play("HostAttack02");
        sfxAudioSource.clip = hostClip02;
        sfxAudioSource.Play();



        Debug.Log("Host Damaged Boss by Skill 2");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(hostUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(hostUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    
    void HostComboAttack()
    {
        if (inspirationFull)
        {   
            soundUnit.inspirationBar = 0;
            inspirationFull = false;
            vfxAnimations.GetComponent<Animator>().Play("HostCombo");
            sfxAudioSource.clip = hostComboClip;
            sfxAudioSource.Play();

            //take summon & host attack
            bool physicalIsDead = bossUnit.TakePhysicalDamage(hostUnit.physicalDamage, this);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(summonUnit.magicalDamage, this);

            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
                // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                TransitionToNextTurn();
            }
            else
            {
                TransitionToNextTurn();
            }

            DamageMonitor();
        }
    }


    void SummonSkill_A()
    {   
        Debug.Log("Summon Damaged Boss by Skill 1");
        vfxAnimations.GetComponent<Animator>().Play("SummonAttack01");
        sfxAudioSource.clip = summon02;
        sfxAudioSource.Play();

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(summonUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(summonUnit.magicalDamage);
        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    void SummonSkill_B()
    {
        Debug.Log("Summon Damaged Boss by Skill 2");
        vfxAnimations.GetComponent<Animator>().Play("SummonAttack02");
        sfxAudioSource.clip = summon01;
        sfxAudioSource.Play();

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(summonUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(summonUnit.magicalDamage);
        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    
    void DirectorSkill_A()
    {
        Debug.Log("Director Damaged Boss Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(directorUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(directorUnit.magicalDamage);
        vfxAnimations.GetComponent<Animator>().Play("SingleHit");
        sfxAudioSource.clip = singlehit;
        sfxAudioSource.Play();

        //check if boss is dead or if the current bar reaches 0

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();    
    }
    void DirectorSkill_B()
    {
        //add future function to stop boss behavior
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(directorUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(directorUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
            // Check if only one of them is dead
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();   
    }

    void SoundSkill_A()
    {
        Debug.Log("Sound Damaged Boss Skill 1");

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(soundUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(soundUnit.magicalDamage);
        vfxAnimations.GetComponent<Animator>().Play("SingleHit");
        sfxAudioSource.clip = singlehit;
        sfxAudioSource.Play();

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }

        DamageMonitor();
    }
    void SoundSkill_B()
    {
        if (inspirationFull)
        {   
            if (songPlaying >= songs.Count -1)
            {
                songPlaying = 0;
            }
            else
            {
                songPlaying += 1;
            }
        
            ChangeSong(songPlaying);

            Debug.Log("Music Changed");
                //reset the inspiration bar
            soundUnit.inspirationBar = 0;
            inspirationFull = false;

            bool physicalIsDead = bossUnit.TakePhysicalDamage(soundUnit.physicalDamage, this);
            bool magicalIsDead = bossUnit.TakeMagicalDamage(soundUnit.magicalDamage, this);

            if (physicalIsDead && magicalIsDead)
            {
                EndBattle(); // Game over
            }
                // Check if only one of them is dead
            else if (physicalIsDead || magicalIsDead)
            {
                TransitionToNextTurn();
            }
            else
            {
                TransitionToNextTurn();
            }

            DamageMonitor();
        }
    }
    
    //I made switch camera skill into skill_B, so that every Skill_A is normal attack and Skill_B is skill
    void CameraSkill_A()
    {   
        Debug.Log("Camera Uses Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(cameraUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(cameraUnit.magicalDamage);
        vfxAnimations.GetComponent<Animator>().Play("SingleHit");
        sfxAudioSource.clip = singlehit;
        sfxAudioSource.Play();

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    void CameraSkill_B()
    {
        vCam1On = !vCam1On;
        if (vCam1On) 
        {
            vCam1.Priority = 1;
            vCam2.Priority = 0;
        } 
        else 
        {
            vCam1.Priority = 0;
            vCam2.Priority = 1;
        }
        SetBossVisibility(vCam2.Priority > vCam1.Priority);
        //Player sees boss sprite after this line
        Debug.Log("Camera Uses Skill 2");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(cameraUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(cameraUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    
    void InternSkill_A()
    {
        Debug.Log("Intern Uses Skill 1");
        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(internUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(internUnit.magicalDamage);
        vfxAnimations.GetComponent<Animator>().Play("SingleHit");
        sfxAudioSource.clip = singlehit;
        sfxAudioSource.Play();

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();

        }
        else
        {
            TransitionToNextTurn();

        }
        DamageMonitor();
    }
    void InternSkill_B()
    {
        Debug.Log("Intern Lucky Time Skill 2");
        
        //basically have a 50% chance to either increase or decrease health
        //I didn't include magical health because it doesn't make sense for an intern to be able to influence our majesty (summon);
        int chance = Random.Range(0, 2);
        if (chance == 0)
        {
            teamphysicalHP -= 100;
            Debug.Log("A o! Damn!");
            vfxAnimations.GetComponent<Animator>().Play("InternBad");
            sfxAudioSource.clip = Intern02;
            sfxAudioSource.Play();
        }
        
        else if (teamphysicalHP + 50 > maxTeamPhysicalHP )
        {
            teamphysicalHP = maxTeamPhysicalHP; 
            Debug.Log("Capped Max Health");
        }

        else
        {
            teamphysicalHP += 100;
            Debug.Log("Yeah Nice Intern but we underpay you!");
            vfxAnimations.GetComponent<Animator>().Play("InternGood");
            sfxAudioSource.clip = Intern01;
            sfxAudioSource.Play();
        }

        bool physicalIsDead = bossUnit.bossTakePhysicalDamage(internUnit.physicalDamage);
        bool magicalIsDead = bossUnit.bossTakeMagicalDamage(internUnit.magicalDamage);

        if (physicalIsDead && magicalIsDead)
        {
            EndBattle(); // Game over
        }
        else if (physicalIsDead || magicalIsDead)
        {
            TransitionToNextTurn();

        }
        else
        {
            TransitionToNextTurn();

        }
        DamageMonitor();
    }

#endregion
 
#region Boss Behavior & Skills
    void BossSkill_A()
    {
        Debug.Log("Boss uses Skill 1");
        bossAttacking();
        //check if animation is running
        vfxAnimations.GetComponent<Animator>().Play("BossAttack");
        sfxAudioSource.clip = Boss01;
        sfxAudioSource.Play();
        //deal both physical and magical damage for now
        bool isDead = soundUnit.TakePhysicalDamage(bossUnit.physicalDamage, this);
        bool summonisDead = summonUnit.TakeMagicalDamage(bossUnit.magicalDamage, this);
        inspirationFull = soundUnit.UpdateInspirationBar(bossUnit.physicalDamage);
        //check if the character is dead
        if (isDead || summonisDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }

    void BossSkill_B()
    {
        Debug.Log("Boss uses Skill 2");
        //check if animation is running
        bossAttacking();
        vfxAnimations.GetComponent<Animator>().Play("BossAttack");
        sfxAudioSource.clip = BossHit02;
        sfxAudioSource.Play();
        //deal double physical damage
        bool isDead = soundUnit.TakePhysicalDamage(bossUnit.physicalDamage * 2, this);
        bool summonisDead = summonUnit.TakeMagicalDamage(bossUnit.magicalDamage, this);
        inspirationFull = soundUnit.UpdateInspirationBar(bossUnit.physicalDamage);
        //check if the character is dead
        if (isDead || summonisDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }

    void BossSkill_C()
    {
        Debug.Log("Boss uses Skill 3");
        //check if animation is running
        bossAttacking();
        vfxAnimations.GetComponent<Animator>().Play("BossAttack");
        sfxAudioSource.clip = BossHit01;
        sfxAudioSource.Play();
        //deal double physical damage
        bool isDead = soundUnit.TakePhysicalDamage(bossUnit.physicalDamage, this);
        bool summonisDead = summonUnit.TakeMagicalDamage(bossUnit.magicalDamage *2, this);
        inspirationFull = soundUnit.UpdateInspirationBar(bossUnit.physicalDamage);
        //check if the character is dead
        if (isDead || summonisDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
    
    void BossSkill_D()
    {
        Debug.Log("Boss uses Skill 4 and disappeared");
        //deal 1/3 damage & disappear 
        SetBossVisibility(false);
        bool isDead = soundUnit.TakePhysicalDamage(bossUnit.physicalDamage/3, this);
        bool summonisDead = summonUnit.TakeMagicalDamage(bossUnit.magicalDamage/3, this);
        inspirationFull = soundUnit.UpdateInspirationBar(bossUnit.physicalDamage);
        //check if the character is dead
        if (isDead || summonisDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            TransitionToNextTurn();
        }
        DamageMonitor();
    }
#endregion
    
    void Setup()
    {
        //this function spawns every unit at the beginning of the game
        GameObject playerHost = Instantiate(hostPrefab, hostBattleStation);
        hostUnit = playerHost.GetComponent<Unit>();

        GameObject playerSummon = Instantiate(summonPrefab, summonBattleStation);
        summonUnit = playerSummon.GetComponent<Unit>();

        GameObject playerDirector = Instantiate(directorPrefab, directorBattleStation);
        directorUnit = playerDirector.GetComponent<Unit>();

        GameObject playerSound = Instantiate(soundPrefab, soundBattleStation);
        soundUnit = playerSound.GetComponent<Unit>();

        GameObject playerCamera = Instantiate(cameraPrefab, cameraBattleStation);
        cameraUnit = playerCamera.GetComponent<Unit>();

        GameObject playerIntern = Instantiate(internPrefab, internBattleStation);
        internUnit = playerIntern.GetComponent<Unit>();

        //GO stands for gameobject
        GameObject bossGO = Instantiate(bossPrefab, bossBattleStation);
        bossUnit = bossGO.GetComponent<Unit>();
        bossSpriteRenderer = bossGO.transform.Find("bossegg").GetComponent<SpriteRenderer>();

        state = BattleState.HOSTTURN;
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            Debug.Log("You Win The JRPG Battle!");
            winUI.SetActive(true);
         
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("You Are So WEAK! You Lost!");
            loseUI.SetActive(true);
        }

    }

    void TransitionToNextTurn() //this function now is a state machine switch
    {
        //Debug.Log($"Current state before transition: {state}");
        switch (state)
        {
            case BattleState.HOSTTURN:
                state = BattleState.HOSTMENU;
                break;
            case BattleState.HOSTMENU:
                state = BattleState.SUMMONTURN;
                break;
            case BattleState.SUMMONTURN:
                state = BattleState.SUMMONMENU;
                break;
            case BattleState.SUMMONMENU:
                state = BattleState.DIRECTORTURN;
                break;
            case BattleState.DIRECTORTURN:
                state = BattleState.DIRECTORMENU;
                break;
            case BattleState.DIRECTORMENU:
                state = BattleState.SOUNDTURN;
                break;
            case BattleState.SOUNDTURN:
                state = BattleState.SOUNDMENU;
                break;
            case BattleState.SOUNDMENU:
                state = BattleState.CAMERATURN;
                break;
            case BattleState.CAMERATURN:
                state = BattleState.CAMERAMENU;
                break;
            case BattleState.CAMERAMENU:
                state = BattleState.INTERNTURN;
                break;
            case BattleState.INTERNTURN:
                state = BattleState.INTERNMENU;
                break;
            case BattleState.INTERNMENU:
                state = BattleState.BOSSTURN;
                break;
            case BattleState.BOSSTURN:
                state = BattleState.HOSTTURN;
                break;
        }

        isTurnStart = true;
  
        //Debug.Log($"Transitioning to: {state}");
    }
   
    void DamageMonitor()
    {
        if (DamageMonitorOn == true)
        {
            //show all stats
            Debug.Log("Team Physical HP: " + teamphysicalHP + " " + "Team Magical HP: " + teamMagicalHP);
            Debug.Log("Boss physical HP: " + bossUnit.currentPhysicalHP + " " + "Boss magical HP: " + bossUnit.currentMagicalHP);
            Debug.Log("Inspiration:" + soundUnit.inspirationBar + " " + "InspirationFull is" + inspirationFull);
        }
    }
    
    //took this from our studio 2 project, we can adjust it later and maybe simplify it if it's weird
    public void ChangeSong(int newSongIndex)
    {
        if (newSongIndex != currentSongIndex && newSongIndex < songs.Count)
        {
            StartCoroutine(FadeChangeSong(newSongIndex));
             bossWeak = (newSongIndex == 1);
        }
    }    
    private IEnumerator FadeChangeSong(int newSongIndex)
    {
        // Fade out the current song if it's playing
        if (currentSongIndex != -1)
        {
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                MusicSource.volume = (1 - t / fadeTime);
                yield return null;
            }
            MusicSource.Stop();
            MusicSource.volume = 1; // Reset volume
        }

        // Update the current song index
        currentSongIndex = newSongIndex;

        // Set the new song and fade it in
        MusicSource.clip = songs[newSongIndex];
        MusicSource.Play();

        MusicSource.volume = 0; // Start at 0 volume to fade in
        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        { MusicSource.volume = t / fadeTime;
            yield return null;
        }

        MusicSource.volume = 1; // Ensure volume is back to full after fade in
    }
    
    private void UpdateUIText(string message)//this method sync debug and ui message
    {
        uiMessage = message;
        testingPrompts.text = uiMessage; //update ui message 
        Debug.Log(uiMessage); //show the updated message in console as well
    }

    public void SetBossVisibility(bool isVisible) 
    {
        if (bossSpriteRenderer != null) 
        {
            bossSpriteRenderer.enabled = isVisible;
            Debug.Log("Setting boss visibility to " + isVisible);
        }
        else
        {
            Debug.LogError("Boss SpriteRenderer not found.");
        }
    }

   void bossAttacking()
   {
           bossUnit.GetComponent<Animator>().Play("Boss_Attack");
           shakeCamera();
   }

   void hostAttacking()
   {
       hostUnit.GetComponent<Animator>().Play("Host_Attack");
       shakeCamera();
   }

   void shakeCamera()
   {
       shaketrue = true; 
   }

    void updateBossHealth()
    {
        TeamHealth.value = teamphysicalHP;
        SummonHealth.value = teamMagicalHP;
        //this is tracking the health of boss to the sliders
        if (bossUnit.currentPhysicalHP >= 300 & bossUnit.currentPhysicalHP <= 400)
        {
            bossHealth04.value = bossUnit.currentPhysicalHP;
            if(bossUnit.currentPhysicalHP <322 & hostUnit.physicalDamage >= 20)
            {
                bossHealth04.value = 300;
            }
        }
        if (bossUnit.currentPhysicalHP >= 200 & bossUnit.currentPhysicalHP <= 300)
        {
            bossHealth03.value = bossUnit.currentPhysicalHP;
            if (bossUnit.currentPhysicalHP < 232 & hostUnit.physicalDamage >= 20)
            {
                bossHealth03.value = 200;
            }
        }
        if (bossUnit.currentPhysicalHP >= 100 & bossUnit.currentPhysicalHP <= 200)
        {
            bossHealth02.value = bossUnit.currentPhysicalHP;
            if (bossUnit.currentPhysicalHP < 122 & hostUnit.physicalDamage >= 20)
            {
                bossHealth02.value = 100;
            }
        }
         if (bossUnit.currentPhysicalHP >= 0 && bossUnit.currentPhysicalHP <= 100)
        {
            bossHealth01.value = bossUnit.currentPhysicalHP;
            if (bossUnit.currentPhysicalHP <22 & hostUnit.physicalDamage >= 20)
            {
                bossHealth01.value = 0;
            }
        }
        //this is tracking the spirit health of boss to the sliders    
        if (bossUnit.currentMagicalHP >= 300 & bossUnit.currentMagicalHP <= 400)
        {
            bossSpirit04.value = bossUnit.currentMagicalHP;
            if (bossUnit.currentMagicalHP < 322 & summonUnit.magicalDamage >= 20)
            {
                bossSpirit04.value = 300;
            }
        }
        if (bossUnit.currentMagicalHP >= 200 & bossUnit.currentMagicalHP <= 300)
        {
            bossSpirit03.value = bossUnit.currentMagicalHP;
            if (bossUnit.currentMagicalHP < 232 & summonUnit.magicalDamage >= 20)
            {
                bossSpirit03.value = 200;
            }
        }
        if (bossUnit.currentMagicalHP >= 100 & bossUnit.currentMagicalHP <= 200)
        {
            bossSpirit02.value = bossUnit.currentMagicalHP;
            if (bossUnit.currentMagicalHP < 122 & summonUnit.magicalDamage >= 20)
            {
                bossSpirit02.value = 100;
            }
        }
        if (bossUnit.currentMagicalHP >= 0 && bossUnit.currentMagicalHP <= 100)
        {
            bossSpirit02.value = bossUnit.currentMagicalHP;
            if (bossUnit.currentMagicalHP < 22 & summonUnit.magicalDamage >= 20)
            {
                bossSpirit01.value = 0;
            }
        }
    }

}  