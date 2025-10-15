using DGP.UnitySignals.Signals;
using NUnit.Framework;

namespace DGP.UnitySignals.Editor.Tests
{
    public class ComputedSignalTest
    {
        private enum TestState
        {
            Idle,
            Running,
            Paused,
            Completed
        }

        private enum GameStateEnum
        {
            Playing,
            Paused
        }

        [Test]
        public void TestCalculatedSignal()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            ComputedSignal<int> computedSignal = new ComputedSignal<int>(() => signal.GetValue() * 2);
            
            Assert.AreEqual(2, computedSignal.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(4, computedSignal.GetValue());
        }
        
        [Test]
        public void TestNestedCalculatedSignals()
        {
            IntegerValueSignal signal = new IntegerValueSignal(1);
            ComputedSignal<int> computedSignal = new ComputedSignal<int>(() => signal.GetValue() * 2);
            ComputedSignal<int> computedSignal2 = new ComputedSignal<int>(() => computedSignal.GetValue() * 2);
            
            Assert.AreEqual(4, computedSignal2.GetValue());
            
            signal.SetValue(2);
            Assert.AreEqual(8, computedSignal2.GetValue());
        }

        [Test]
        public void TestCalculatedThresholdValue()
        {
            FloatValueSignal oxygenRemaining = new(1.0f);
            float threshold = 0.25f;
            ComputedSignal<bool> oxygenLow = new(() => oxygenRemaining.GetValue() < threshold);
            
            Assert.IsFalse(oxygenLow.GetValue());
            
            oxygenRemaining.SetValue(0.2f);
            Assert.IsTrue(oxygenLow.GetValue());
        }

        [Test]
        public void TestEnumSignalWithBooleanComputed()
        {
            ValueSignal<GameStateEnum> gameState = new(GameStateEnum.Playing);
            ComputedSignal<bool> isPaused = new(() => gameState.Value == GameStateEnum.Paused);
            
            Assert.IsFalse(isPaused.Value, "Should not be paused initially");
            
            gameState.Value = GameStateEnum.Paused;
            Assert.IsTrue(isPaused.Value, "Should be paused after setting to Paused");
            
            gameState.Value = GameStateEnum.Playing;
            Assert.IsFalse(isPaused.Value, "Should not be paused after resuming");
        }

        [Test]
        public void TestEnumSignalComputed()
        {
            ValueSignal<TestState> state = new(TestState.Idle);
            ComputedSignal<bool> isActive = new(() => 
                state.Value == TestState.Running || state.Value == TestState.Paused);
            
            Assert.IsFalse(isActive.Value, "Should not be active when Idle");
            
            state.Value = TestState.Running;
            Assert.IsTrue(isActive.Value, "Should be active when Running");
            
            state.Value = TestState.Paused;
            Assert.IsTrue(isActive.Value, "Should be active when Paused");
            
            state.Value = TestState.Completed;
            Assert.IsFalse(isActive.Value, "Should not be active when Completed");
        }

        [Test]
        public void TestEnumToEnumComputed()
        {
            ValueSignal<TestState> state = new(TestState.Idle);
            ComputedSignal<TestState> nextState = new(() => 
                state.Value == TestState.Idle ? TestState.Running :
                state.Value == TestState.Running ? TestState.Paused :
                state.Value == TestState.Paused ? TestState.Running :
                state.Value == TestState.Completed ? TestState.Idle :
                TestState.Idle
            );
            
            Assert.AreEqual(TestState.Running, nextState.Value);
            
            state.Value = TestState.Running;
            Assert.AreEqual(TestState.Paused, nextState.Value);
            
            state.Value = TestState.Paused;
            Assert.AreEqual(TestState.Running, nextState.Value);
        }

        [Test]
        public void TestEnumComputedNotifiesObservers()
        {
            ValueSignal<GameStateEnum> gameState = new(GameStateEnum.Playing);
            ComputedSignal<bool> isPaused = new(() => gameState.Value == GameStateEnum.Paused);
            
            bool observerCalled = false;
            bool receivedValue = false;
            
            isPaused.AddObserver((newValue) =>
            {
                observerCalled = true;
                receivedValue = newValue;
            });
            
            gameState.Value = GameStateEnum.Paused;
            
            Assert.IsTrue(observerCalled, "Observer should be called when computed value changes");
            Assert.IsTrue(receivedValue, "Observer should receive true when paused");
        }

        [Test]
        public void TestMultipleEnumSignalsInComputed()
        {
            ValueSignal<TestState> playerState = new(TestState.Idle);
            ValueSignal<TestState> enemyState = new(TestState.Idle);
            
            ComputedSignal<bool> bothActive = new(() => 
                (playerState.Value == TestState.Running || playerState.Value == TestState.Paused) &&
                (enemyState.Value == TestState.Running || enemyState.Value == TestState.Paused));
            
            Assert.IsFalse(bothActive.Value, "Both should not be active initially");
            
            playerState.Value = TestState.Running;
            Assert.IsFalse(bothActive.Value, "Only player is active");
            
            enemyState.Value = TestState.Running;
            Assert.IsTrue(bothActive.Value, "Both should be active now");
            
            playerState.Value = TestState.Idle;
            Assert.IsFalse(bothActive.Value, "Player no longer active");
        }
    }
}