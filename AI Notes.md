Tachy's charactertable entry has a property "StanceAliasArray" containing the CharacterStance M_Tachy_Finish. The stance comes with a condition of being at 1 HP. Once triggered, this applies the effect Passive_DeathReady which chains into Passive_FinishReady and removes immortality. Passive_FinishReady allows for the player to perform the SkillActiveStep P_Eve_Sword_Normal_FinishLinkAttack1_1_Cast1_Tachy (finisher animation thingy) which applies M_Tachy_FinishStart. This triggers the cutscene


AI file references SkillTable / SkillCommandTable entry
SkillCommandTable entry does ???
SkillTable entry references TargetFilterTable entry
SkillTable entry references CharacterMoveTable entry (validity check)
SkillTable entry references SkillActiveStepTable (individual sub-motions)
SkillActiveStepTable entry references others in same file as a chain
SkillActiveStepTable entry determines the motion and used animation
SkillActiveStepTable entry can also apply effects to self (phase change)

EffectTable entry references CharacterStanceTable entry
CharacterStanceTable entry lists valid skill names?