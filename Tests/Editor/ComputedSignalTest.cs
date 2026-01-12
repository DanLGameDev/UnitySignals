using System;
using System.Diagnostics.Contracts;
using DGP.UnitySignals.Signals;
using NUnit.Framework;
using UnityEngine;

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

            isPaused.AddObserver((newValue) => {
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

        [Test]
        public void TestSignalDeath()
        {
            var signal = new IntegerValueSignal(42);
            Assert.IsFalse(signal.IsDead, "Signal should not be dead initially");

            signal.Dispose();
            Assert.IsTrue(signal.IsDead, "Signal should be dead after disposal");
        }

        [Test]
        public void TestComputedSignalDependencyDeath()
        {
            var baseSignal = new IntegerValueSignal(42);
            var computed = new ComputedSignal<int>(() => baseSignal.GetValue() * 2);

            Assert.IsFalse(computed.IsDead, "Computed signal should not be dead initially");
            Assert.IsTrue(computed.HasValidDependencies(), "Dependencies should be valid initially");

            baseSignal.Dispose();

            Assert.IsFalse(computed.HasValidDependencies(), "Dependencies should be invalid after base signal disposal");
            Assert.IsTrue(computed.IsDead, "Computed signal should be dead after dependency death");
        }

        [Test]
        public void TestSignalDiedEvent()
        {
            var signal = new IntegerValueSignal(42);
            bool eventFired = false;

            signal.SignalDied += (sender) => {
                eventFired = true;
                Assert.AreEqual(signal, sender, "Sender should be the signal that died");
            };

            signal.Dispose();
            Assert.IsTrue(eventFired, "SignalDied event should fire on disposal");
        }

        [Test]
        public void TestNestedComputedSignalDependencyDeath()
        {
            var baseSignal = new IntegerValueSignal(42);
            var computed1 = new ComputedSignal<int>(() => baseSignal.GetValue() * 2);
            var computed2 = new ComputedSignal<int>(() => computed1.GetValue() + 1);

            bool computed1DiedEventFired = false;
            computed1.SignalDied += (sender) => { computed1DiedEventFired = true; };

            Assert.IsFalse(computed2.IsDead, "Computed signal should not be dead initially");

            baseSignal.Dispose();

            // Check if the event fired
            Assert.IsTrue(computed1DiedEventFired, "SignalDied event should fire for computed1");

            // Death should propagate through the chain
            Assert.IsTrue(computed1.IsDead, "First computed signal should be dead after base signal death");
            Assert.IsTrue(computed2.IsDead, "Second computed signal should be dead after dependency chain death");
        }

        [Test]
        public void TestDeadSignalBehavior()
        {
            var signal = new IntegerValueSignal(42);
            int invoked = 0;

            // Use specific Action<int> signature to avoid ambiguity
            Action<int> observer = (int newValue) => invoked++;
            signal.AddObserver(observer);

            signal.Dispose();

            // Observers should no longer be notified after death
            signal.SetValue(99);
            Assert.AreEqual(0, invoked, "Dead signal should not notify observers");
        }

        [Test]
        public void TestAccessingDeadComputedSignal()
        {
            var baseSignal = new IntegerValueSignal(42);
            var computed = new ComputedSignal<int>(() => baseSignal.GetValue() * 2);
            int initialValue = computed.GetValue();

            baseSignal.Dispose();
            int valueAfterDeath = computed.GetValue();

            // The computed signal should return its last known value
            Assert.AreEqual(initialValue, valueAfterDeath, "Dead computed signal should return last known value");
        }

        [Test]
        public void TestComplexSignalArrangement()
        {
            IntegerValueSignal signalA = new(1);

            ComputedSignal<int> computedA = new ComputedSignal<int>(() => signalA.GetValue() + 1);
            ComputedSignal<int> computedB = new ComputedSignal<int>(() => signalA.GetValue() * 3);

            ComputedSignal<int> computedC = new ComputedSignal<int>(() => computedB.GetValue() + computedA.GetValue());

            int computedACallCount = 0;
            int computedBCallCount = 0;
            int computedCCallCount = 0;

            computedA.AddObserver((Action<int>)(newValue => { computedACallCount++; }));
            computedB.AddObserver((Action<int>)(newValue => { computedBCallCount++; }));
            computedC.AddObserver((Action<int>)(newValue => { computedCCallCount++; }));

            Assert.AreEqual(5, computedC.GetValue(), "Initial computedC value incorrect");

            signalA.Value = 5;
            Assert.AreEqual(1, computedACallCount, "computedA should have been called once after signalA change");
            Assert.AreEqual(1, computedBCallCount, "computedB should have been called once after signalA change");
            Assert.AreEqual(1, computedCCallCount, "computedC should have been called once after signalA change");

            Assert.AreEqual(21, computedC.GetValue(), "Updated computedC value incorrect");
        }

        [Test]
        public void TestManualRecalculate_WithNonReactiveVariable()
        {
            int externalValue = 5;
            var signal = new ComputedSignal<int>(() => externalValue * 2);

            Assert.AreEqual(10, signal.GetValue(), "Initial value should be 10");

            // Change external value - signal won't know automatically
            externalValue = 10;
            Assert.AreEqual(10, signal.GetValue(), "Value should still be 10 (not recalculated)");

            // Manual recalculate
            int result = signal.Recalculate();
            Assert.AreEqual(20, result, "Recalculate should return new value of 20");
            Assert.AreEqual(20, signal.GetValue(), "GetValue should now return 20");
        }

        [Test]
        public void TestManualRecalculate_NotifiesObservers()
        {
            int externalValue = 5;
            var signal = new ComputedSignal<int>(() => externalValue * 2);

            int observerCallCount = 0;
            int lastReceivedValue = 0;

            signal.AddObserver((int newValue) => {
                observerCallCount++;
                lastReceivedValue = newValue;
            });

            externalValue = 10;
            signal.Recalculate();

            Assert.AreEqual(1, observerCallCount, "Observer should be called once");
            Assert.AreEqual(20, lastReceivedValue, "Observer should receive new value of 20");
        }

        [Test]
        public void TestManualRecalculate_NoChangeNoNotification()
        {
            int externalValue = 5;
            var signal = new ComputedSignal<int>(() => externalValue * 2);

            int observerCallCount = 0;
            signal.AddObserver((int newValue) => observerCallCount++);

            // Recalculate without changing externalValue
            int result = signal.Recalculate();

            Assert.AreEqual(10, result, "Should return same value");
            Assert.AreEqual(0, observerCallCount, "Observer should not be called when value doesn't change");
        }

        [Test]
        public void TestManualRecalculate_MixedReactiveAndNonReactive()
        {
            var reactiveSignal = new IntegerValueSignal(5);
            int nonReactiveValue = 3;

            var computed = new ComputedSignal<int>(() => reactiveSignal.GetValue() + nonReactiveValue);

            Assert.AreEqual(8, computed.GetValue(), "Initial: 5 + 3 = 8");

            // Change reactive signal - should auto-update
            reactiveSignal.Value = 10;
            Assert.AreEqual(13, computed.GetValue(), "Reactive change: 10 + 3 = 13");

            // Change non-reactive value - needs manual recalculate
            nonReactiveValue = 7;
            Assert.AreEqual(13, computed.GetValue(), "Before recalculate: still 13");

            int result = computed.Recalculate();
            Assert.AreEqual(17, result, "After recalculate: 10 + 7 = 17");
        }
        
        [Test]
        public void TestGetValue_WithDeadDependencies_MarksSignalAsDead()
        {
            var baseSignal = new IntegerValueSignal(42);
            var computed = new ComputedSignal<int>(() => baseSignal.GetValue() * 2);
    
            baseSignal.Dispose();
    
            // GetValue should detect dead dependency and mark itself as dead
            int value = computed.GetValue();
    
            Assert.IsTrue(computed.IsDead, "Computed signal should be marked dead after accessing with dead dependencies");
        }
        
        [Test]
        public void TestComputedSignal_DetectsSelfReference()
        {
            ComputedSignal<int> computed = null;
    
            // This would create a self-reference, which should be caught
            computed = new ComputedSignal<int>(() => computed != null ? computed.GetValue() + 1 : 0);
    
            // Should not throw, should handle gracefully
            int value = computed.GetValue();
            Assert.AreEqual(0, value, "Self-referencing computed should handle gracefully");
        }
        
        [Test]
        public void TestComputedSignalDispose_UnsubscribesFromSources()
        {
            var baseSignal = new IntegerValueSignal(42);
            var computed = new ComputedSignal<int>(() => baseSignal.GetValue() * 2);
    
            int computedCallCount = 0;
            computed.AddObserver((int newValue) => computedCallCount++);
    
            // Dispose computed signal
            computed.Dispose();
    
            // Change base signal - computed should not react anymore
            baseSignal.Value = 99;
    
            Assert.AreEqual(0, computedCallCount, "Disposed computed signal should not react to source changes");
            Assert.IsTrue(computed.IsDead, "Disposed signal should be marked as dead");
        }
    }
}