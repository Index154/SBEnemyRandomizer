Tachy notes:
    Tachy's charactertable entry has a property "StanceAliasArray" containing the CharacterStance M_Tachy_Finish. The stance comes with a condition of being at 1 HP. Once triggered, this applies the effect Passive_DeathReady which causes Passive_FinishReady and removes immortality. Passive_FinishReady allows for the player to perform the SkillActiveStep P_Eve_Sword_Normal_FinishLinkAttack1_1_Cast1_Tachy (finisher animation thingy) which applies M_Tachy_FinishStart. This triggers the cutscene
    Applying M_Tachy_Finish to another boss allows it to enter the state of ready to be killed and the button prompt is also being displayed. But pressing the button does not do anything.

    AI file references SkillCommandTable entry
    SkillCommandTable entry references SkillTable entry
    SkillTable entry references TargetFilterTable entry
    SkillTable entry references CharacterMoveTable entry (validity check)
    SkillTable entry references SkillActiveStepTable (individual sub-steps of skills)
    SkillActiveStepTable entry references others in same file as a chain of events
    SkillActiveStepTable entry references CharacterMoveTable entry (actual movement values)
    SkillActiveStepTable entry determines used animation
    SkillActiveStepTable entry can also apply effects to self (phase change)

    EffectTable entry references CharacterStanceTable entry
    CharacterStanceTable entry lists valid skill names?


Music list:
    ELDER_P1
    ELDER_P2_INTRO
    LILY_EXO_P1
    LILY_EXO_P2
    ABADDON
    M_Opener_Phase2
    ABADDON_DESERT
    M_OpenerWasteland_Phase2
    GrubShooter
    WASTELAND_GRUBSHOOTER
    GIGAS
    RAVEN
    BRUTE
    M_HedgeBoarBrute_Phase2
    DESERT_STALKER
    DESERT_BEHEMOTH
    JUGGERNAUT_INTRO
    TACHY_INTRO
    ME_TACHY_P2
    BELIAL_01
    M_WeaponMasterA_Phase2
    M_WeaponMasterA_Phase3
    KARAKURI_ENTER
    M_Marionette_Phase2
    Seq_Subtitle_SE10_CrawlerAppearance_09
    M_Crawler_Phase2
    M_Crawler_Phase3
    WLA_GIGAS
    MANN_INTRO
    Seq_Subtitle_Nikke_Scarlet_Entrance_08
    DED40_AreaCVol_001_Enable
    Seq_Subtitle_SE11_ShooterModeOpenning_09
    AYL_06_AreaCVol_001_Enable