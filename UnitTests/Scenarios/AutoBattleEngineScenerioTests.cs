﻿using System.Threading.Tasks;

using NUnit.Framework;

using Game.Engine.EngineKoenig;
using Game.Models;
using Game.Helpers;
using Game.ViewModels;

namespace Scenario
{
    [TestFixture]
    public class AutoBattleEngineScenarioTests
    {
        AutoBattleEngine AutoBattle;

        [SetUp]
        public void Setup()
        {
            AutoBattle = new AutoBattleEngine();

            AutoBattle.Battle.EngineSettings.CharacterList.Clear();
            AutoBattle.Battle.EngineSettings.MonsterList.Clear();
            AutoBattle.Battle.EngineSettings.CurrentDefender = null;
            AutoBattle.Battle.EngineSettings.CurrentAttacker = null;

            _ = AutoBattle.Battle.StartBattle(true);   // Clear the Engine
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void AutoBattleEngine_Constructor_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = AutoBattle;

            // Reset

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_Monsters_1_Should_Pass()
        {
            //Arrange

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 1;

            var CharacterPlayerMike = new PlayerInfoModel(
                            new CharacterModel
                            {
                                Speed = -1,
                                Level = 10,
                                CurrentHealth = 11,
                                ExperienceTotal = 1,
                                ExperienceRemaining = 1,
                                Name = "Mike",
                                ListOrder = 1,
                            });

            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayerMike);


            // Add Monsters

            // Need to set the Monster count to 1, so the battle goes to Next Round Faster
            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 1;

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset

            //Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_Character_Level_Up_Should_Pass()
        {

            /* 
             * Test to force leveling up of a character during the battle
             * 
             * 1 Character, Experience set at next level mark
             * 
             * 6 Monsters
             * 
             * Character Should Level UP 1 level
             * 
             */

            //Arrange

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 1;

            CharacterIndexViewModel.Instance.Dataset.Clear();

            // To See Level UP happening, a character needs to be close to the next level
            var Character = new CharacterModel
            {
                ExperienceTotal = 300,    // Enough for next level
                Name = "Mike Level Example",
                Speed = 100,    // Go first
            };

            // Remember Start Level
            var StartLevel = Character.Level;

            var CharacterPlayer = new PlayerInfoModel(Character);

            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayer);


            // Add Monsters

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset

            //Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(true, AutoBattle.Battle.EngineSettings.BattleScore.CharacterAtDeathList.Contains("Mike Level Example"));
            // Assert.AreEqual(StartLevel+1, Engine.EngineSettings.BattleScore.CharacterModelDeathList.Where(m=>m.Guid.Equals(Character.Guid)).First().Level);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_GameOver_Round_1_Should_Pass()
        {
            /* 
             * 
             * 1 Character, Speed slowest, only 1 HP
             * 
             * 6 Monsters
             * 
             * Should end in the first round
             * 
             */

            //Arrange

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 1;

            var CharacterPlayer = new PlayerInfoModel(
                            new CharacterModel
                            {
                                Speed = -1, // Will go last...
                                Level = 10,
                                CurrentHealth = 1,
                                ExperienceTotal = 1,
                                ExperienceRemaining = 1,
                                ListOrder = 1,
                            });

            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayer);


            // Add Monsters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 6;

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset

            //Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_InValid_Round_Loop_Should_Fail()
        {
            /* 
             * Test infinate rounds.  
             * 
             * Characters overpower monsters, game never ends
             * 
             * 6 Character
             *      Speed high
             *      Hit Hard
             *      High health
             * 
             * 1 Monsters
             *      Slow
             *      Weak Hit
             *      Weak health
             * 
             * Should never end
             * 
             * Inifinite Loop Check should stop the game
             * 
             */

            //Arrange

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 6;

            var CharacterPlayer = new PlayerInfoModel(
                            new CharacterModel
                            {
                                Speed = 100,
                                Level = 20,
                                MaxHealth = 200,
                                CurrentHealth = 200,
                                ExperienceTotal = 1,
                            });

            var CharacterPlayerMin = new PlayerInfoModel(
                new CharacterModel
                {
                    Speed = 99,
                    Level = 1,
                    MaxHealth = 200,
                    CurrentHealth = 200,
                    ExperienceTotal = 1,
                });

            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayer);
            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayer);
            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayer);
            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayer);
            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayer);
            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayerMin);

            // Add Monsters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 1;

            // Controll Rolls,  Hit is always a 3
            _ = DiceHelper.EnableForcedRolls();
            _ = DiceHelper.SetForcedRollValue(3);

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset
            _ = DiceHelper.DisableForcedRolls();

            //Assert
            Assert.AreEqual(false, result);
            Assert.AreEqual(true, AutoBattle.Battle.EngineSettings.BattleScore.RoundCount > AutoBattle.Battle.EngineSettings.MaxRoundCount);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_InValid_Trun_Loop_Should_Fail()
        {
            /* 
             * Test infinate turn.  
             * 
             * Monsters overpower Characters game never ends
             * 
             * 1 Character
             *      Speed low
             *      Hit weak
             *      Health low
             * 
             * 6 Monsters
             *      Speed High
             *      Hit strong
             *      Health High
             * 
             * Rolls for always Miss
             * 
             * Should never end
             * 
             * Inifinite Loop Check should stop the game
             * 
             */

            //Arrange

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 1;

            var CharacterPlayerMike = new PlayerInfoModel(
                            new CharacterModel
                            {
                                Speed = 1,
                                Level = 1,
                                MaxHealth = 1,
                                CurrentHealth = 1,
                            });

            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayerMike);


            // Add Monsters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 6;

            // Controll Rolls,  Always Miss
            _ = DiceHelper.EnableForcedRolls();
            _ = DiceHelper.SetForcedRollValue(1);

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset
            _ = DiceHelper.DisableForcedRolls();

            //Assert
            Assert.AreEqual(false, result);
            Assert.AreEqual(true, AutoBattle.Battle.EngineSettings.BattleScore.TurnCount > AutoBattle.Battle.EngineSettings.MaxTurnCount);
            Assert.AreEqual(true, AutoBattle.Battle.EngineSettings.BattleScore.RoundCount < AutoBattle.Battle.EngineSettings.MaxRoundCount);
        }

       [Test]
        public async Task AutoBattleEngine_RunAutoBattle_Monsters_2_Should_Pass()
        {
            //Arrange

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 2;

            var CharacterPlayerA = new PlayerInfoModel(
                            new CharacterModel
                            {
                                Speed = -1,
                                Level = 10,
                                CurrentHealth = 1,
                                ExperienceTotal = 1,
                                ExperienceRemaining = 1,
                                Name = "Mike",
                                ListOrder = 1,
                            });

            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayerA);
            AutoBattle.Battle.EngineSettings.CharacterList.Add(CharacterPlayerA);


            // Add Monsters

            // Need to set the Monster count to 1, so the battle goes to Next Round Faster
            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 2;

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset
            AutoBattle.Battle.EngineSettings.CharacterList.Clear();

            //Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_Monsters_3_Should_Pass()
        {
            //Arrange
            AutoBattle.Battle.EngineSettings.MaxRoundCount = 1000;
            AutoBattle.Battle.EngineSettings.MaxTurnCount = 100000;

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 3;


            // Add Monsters

            // Need to set the Monster count to 1, so the battle goes to Next Round Faster
            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 3;

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset
            AutoBattle.Battle.EngineSettings.CharacterList.Clear();
            AutoBattle.Battle.EngineSettings.MaxRoundCount = 100;
            AutoBattle.Battle.EngineSettings.MaxTurnCount = 1000;

            //Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_Monsters_4_Should_Pass()
        {
            //Arrange
            AutoBattle.Battle.EngineSettings.MaxRoundCount = 1000;
            AutoBattle.Battle.EngineSettings.MaxTurnCount = 100000;

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 4;


            // Add Monsters

            // Need to set the Monster count to 1, so the battle goes to Next Round Faster
            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 4;

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset
            AutoBattle.Battle.EngineSettings.MaxRoundCount = 100;
            AutoBattle.Battle.EngineSettings.MaxTurnCount = 1000;
            AutoBattle.Battle.EngineSettings.CharacterList.Clear();


            //Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public async Task AutoBattleEngine_RunAutoBattle_Monsters_5_Should_Pass()
        {
            //Arrange
            AutoBattle.Battle.EngineSettings.MaxRoundCount = 1000;
            AutoBattle.Battle.EngineSettings.MaxTurnCount = 100000;

            // Add Characters

            AutoBattle.Battle.EngineSettings.MaxNumberPartyCharacters = 5;


            // Add Monsters

            // Need to set the Monster count to 1, so the battle goes to Next Round Faster
            AutoBattle.Battle.EngineSettings.MaxNumberPartyMonsters = 5;

            //Act
            var result = await AutoBattle.RunAutoBattle();

            //Reset
            AutoBattle.Battle.EngineSettings.MaxRoundCount = 100;
            AutoBattle.Battle.EngineSettings.MaxTurnCount = 1000;
            AutoBattle.Battle.EngineSettings.CharacterList.Clear();

            //Assert
            Assert.AreEqual(true, result);
        }
    }
}