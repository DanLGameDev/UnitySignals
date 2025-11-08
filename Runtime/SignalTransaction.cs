using System;
using System.Collections.Generic;

namespace DGP.UnitySignals
{
    public class SignalTransaction : IDisposable
    {
        private readonly List<ITransactableSignal> _modifiedSignals = new();
        private bool _isCommitted;
        private bool _isDisposed;

        public SignalTransaction Set<TValueType>(ITransactableSignal<TValueType> signal, TValueType value)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(SignalTransaction));

            if (_isCommitted)
                throw new InvalidOperationException("Cannot modify a transaction after it has been committed");

            bool wasModified = signal.SetValueSilently(value);
            
            if (wasModified && !_modifiedSignals.Contains(signal))
                _modifiedSignals.Add(signal);

            return this;
        }

        public void Commit()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(SignalTransaction));

            if (_isCommitted)
                return;

            _isCommitted = true;

            foreach (var signal in _modifiedSignals)
                signal.FlushNotifications();

            _modifiedSignals.Clear();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            if (!_isCommitted)
                Commit();

            _isDisposed = true;
        }
    }
}