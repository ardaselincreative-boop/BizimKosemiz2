using BizimKosemiz.Networking;
using BizimKosemiz.Player;
using NUnit.Framework;
using UnityEngine;

namespace BizimKosemiz.Tests
{
    public class BizimKosemizArchitectureTests
    {
        [Test]
        public void CharacterController_BottomAlignsWithRootPivotZero()
        {
            GameObject go = new GameObject("TestPlayer");
            go.AddComponent<CharacterController>();
            go.AddComponent<GroundChecker>();
            go.AddComponent<LocalPlayerInputSource>();
            go.AddComponent<PlayerAnimatorBridge>();
            go.AddComponent<PlayerNetworkAdapter>();
            PlayerMovementController movement = go.AddComponent<PlayerMovementController>();

            movement.EnsureCharacterControllerDimensions();
            CharacterController cc = go.GetComponent<CharacterController>();

            float bottomY = cc.center.y - (cc.height * 0.5f);
            Assert.AreEqual(0f, bottomY, 0.001f, "Karakter kapsülünün en alt noktası y = 0.0 (ayak hizası) olmalıdır.");

            Object.DestroyImmediate(go);
        }

        [Test]
        public void RemotePlayer_DoesNotReadLocalInputs()
        {
            GameObject go = new GameObject("RemotePlayer");
            LocalPlayerInputSource input = go.AddComponent<LocalPlayerInputSource>();
            input.ReadInputs(isLocalOwner: false);

            Assert.AreEqual(Vector2.zero, input.MoveInput);
            Assert.AreEqual(Vector2.zero, input.LookInput);
            Assert.IsFalse(input.IsSprintPressed);

            Object.DestroyImmediate(go);
        }
    }
}
