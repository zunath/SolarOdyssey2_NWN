﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Freescape.Game.Server.Data.Contracts;
using Freescape.Game.Server.Data.Entities;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service;
using Freescape.Game.Server.Service.Contracts;
using NSubstitute;
using NUnit.Framework;
using NWN;
using Object = NWN.Object;

namespace Freescape.Game.Server.Tests.Service
{
    public class DeathServiceTests
    {
        private IDataContext _db;

        [SetUp]
        public void SetUp()
        {
            PlayerCharacter dbPC1 = new PlayerCharacter
            {
                PlayerID = "123",
                RespawnLocationX = 0.0f,
                RespawnLocationY = 0.0f,
                RespawnLocationZ = 0.0f,
                RespawnLocationOrientation = 0.0f,
                RespawnAreaTag = string.Empty
            };

            PlayerCharacter dbPC2 = new PlayerCharacter
            {
                PlayerID = "456",
                RespawnLocationX = 50.0f,
                RespawnLocationY = 120.0f,
                RespawnLocationZ = 3.0f,
                RespawnLocationOrientation = 10.0f,
                RespawnAreaTag = "myarea2"
            };

            PlayerCharacter dbPC3 = new PlayerCharacter
            {
                PlayerID = "789",
                RespawnLocationX = 0.23f,
                RespawnLocationY = 1.64f,
                RespawnLocationZ = 200.0f,
                RespawnLocationOrientation = 140.34f,
                RespawnAreaTag = "myarea3"
            };

            IQueryable<PlayerCharacter> players =
                new List<PlayerCharacter>
                {
                    dbPC1,
                    dbPC2,
                    dbPC3
                }.AsQueryable();


            IDbSet<PlayerCharacter> playerDbSet = Substitute.For<IDbSet<PlayerCharacter>>();
            playerDbSet.Provider.Returns(players.Provider);
            playerDbSet.Expression.Returns(players.Expression);
            playerDbSet.ElementType.Returns(players.ElementType);
            playerDbSet.GetEnumerator().Returns(players.GetEnumerator());


            _db = Substitute.For<IDataContext>();
            _db.PlayerCharacters.Returns(playerDbSet);
        }

        [Test]
        public void DeathService_BindPlayerSoul_ShouldUpdateDatabaseAndNotCallFloatingTextStringOnCreature()
        {
            // Arrange
            int callCount = -1;
            INWScript script = Substitute.For<INWScript>();
            INWNXCreature nwnxCreature = Substitute.For<INWNXCreature>();
            ISerializationService serialization = Substitute.For<ISerializationService>();
            IRandomService random = Substitute.For<IRandomService>();
            IColorTokenService color = Substitute.For<IColorTokenService>();
            script.When(x => x.FloatingTextStringOnCreature(Arg.Any<string>(), Arg.Any<Object>(), Arg.Any<int>())).Do(x => callCount++);
            
            DeathService service = new DeathService(_db, script, serialization, random, color);
            NWPlayer player = Substitute.For<NWPlayer>(script, nwnxCreature);
            player.Object.Returns(x => new Object());
            player.GlobalID.Returns("123");
            player.Position.Returns(x => new Vector(43.2f, 22.2f, 87.0f));
            player.Facing.Returns(x => 320.666f);
            player.Area.Returns(x => new NWArea(script));
            player.Area.Tag.Returns("a_fake_area_tag");

            // Act
            service.BindPlayerSoul(player, false);
            var result = _db.PlayerCharacters.Single(x => x.PlayerID == "123");

            // Assert
            Assert.AreEqual(43.2f, result.RespawnLocationX);
            Assert.AreEqual(22.2f, result.RespawnLocationY);
            Assert.AreEqual(87.0f, result.RespawnLocationZ);
            Assert.AreEqual(320.666f, result.RespawnLocationOrientation);
            Assert.AreEqual("a_fake_area_tag", result.RespawnAreaTag);
            Assert.AreEqual(-1, callCount);
        }

        [Test]
        public void DeathService_BindPlayerSoul_ShouldUpdateDatabaseAndCallFloatingTextStringOnCreature()
        {
            // Arrange
            int callCount = 0;
            
            INWScript script = Substitute.For<INWScript>();
            INWNXCreature nwnxCreature = Substitute.For<INWNXCreature>();
            ISerializationService serialization = Substitute.For<ISerializationService>();
            IRandomService random = Substitute.For<IRandomService>();
            IColorTokenService color = Substitute.For<IColorTokenService>();
            script.When(x => x.FloatingTextStringOnCreature(Arg.Any<string>(), Arg.Any<Object>(), Arg.Any<int>())).Do(x => callCount++);

            DeathService service = new DeathService(_db, script, serialization, random, color);
            NWPlayer player = Substitute.For<NWPlayer>(script, nwnxCreature);
            player.Object.Returns(x => new Object());
            player.GlobalID.Returns("123");
            player.Position.Returns(x => new Vector(43.2f, 22.2f, 87.0f));
            player.Facing.Returns(x => 320.666f);
            player.Area.Returns(x => new NWArea(script));
            player.Area.Tag.Returns("a_fake_area_tag");
            

            // Act
            service.BindPlayerSoul(player, true);
            var result = _db.PlayerCharacters.Single(x => x.PlayerID == "123");

            // Assert
            Assert.AreEqual(43.2f, result.RespawnLocationX);
            Assert.AreEqual(22.2f, result.RespawnLocationY);
            Assert.AreEqual(87.0f, result.RespawnLocationZ);
            Assert.AreEqual(320.666f, result.RespawnLocationOrientation);
            Assert.AreEqual("a_fake_area_tag", result.RespawnAreaTag);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void DeathService_BindPlayerSoul_ArgumentNullPlayer_ShouldThrowArgumentNullException()
        {
            // Arrange
            ISerializationService serialization = Substitute.For<ISerializationService>();
            IRandomService random = Substitute.For<IRandomService>();
            IColorTokenService color = Substitute.For<IColorTokenService>();
            DeathService service = new DeathService(_db, Substitute.For<INWScript>(), serialization, random, color);

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                // Act
                service.BindPlayerSoul(null, false);
            });
        }

        [Test]
        public void DeathService_BindPlayerSoul_ArgumentNullPlayerObject_ShouldThrowArgumentNullException()
        {
            // Arrange
            INWScript script = Substitute.For<INWScript>();
            INWNXCreature nwnxCreature = Substitute.For<INWNXCreature>();
            ISerializationService serialization = Substitute.For<ISerializationService>();
            IRandomService random = Substitute.For<IRandomService>();
            IColorTokenService color = Substitute.For<IColorTokenService>();

            DeathService service = new DeathService(_db, script, serialization, random, color);
            NWPlayer player = Substitute.For<NWPlayer>(script, nwnxCreature);
            player.Object.Returns(x => null);

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () =>
            {
                // Act
                service.BindPlayerSoul(player, false);
            });
        }

        [Test]
        public void DeathService_BindPlayerSoul_NoPlayers_ShouldThrowInvalidOperationException()
        {
            // Arrange
            INWScript script = Substitute.For<INWScript>();
            INWNXCreature nwnxCreature = Substitute.For<INWNXCreature>();
            ISerializationService serialization = Substitute.For<ISerializationService>();
            IRandomService random = Substitute.For<IRandomService>();
            IColorTokenService color = Substitute.For<IColorTokenService>();
            DeathService service = new DeathService(_db, script, serialization, random, color);
            NWPlayer player = Substitute.For<NWPlayer>(script, nwnxCreature);

            // Assert
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                // Act
                service.BindPlayerSoul(player, false);
            });
        }
    }
}
