﻿using NUnit.Framework;

using Game.Models;

namespace UnitTests.Models
{
    [TestFixture]
    public class AttributeEnumExtensionsTests
    {
        [Test]
        public void AttributeEnumExtensionsTests_Unknown_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = AttributeEnum.Unknown.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("Unknown", result);
        }

        [Test]
        public void AttributeEnumExtensionsTests_Attack_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = AttributeEnum.Attack.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("ATK", result);
        }

        [Test]
        public void AttributeEnumExtensionsTests_CurrentHealth_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = AttributeEnum.CurrentHealth.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("HP", result);
        }

        [Test]
        public void AttributeEnumExtensionsTests_Defense_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = AttributeEnum.Defense.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("DEF", result);
        }

        [Test]
        public void AttributeEnumExtensionsTests_MaxHealth_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = AttributeEnum.MaxHealth.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("MaxHP", result);
        }

        [Test]
        public void AttributeEnumExtensionsTests_Speed_Default_Should_Pass()
        {
            // Arrange

            // Act
            var result = AttributeEnum.Speed.ToMessage();

            // Reset

            // Assert
            Assert.AreEqual("SPD", result);
        }
    }
}
