using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Resonance.Tests.TestHelpers
{
    /// <summary>
    /// Fake implementation van ISession voor tests
    /// </summary>
    public class MockHttpSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public string Id => "test-session-id";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[]? value)
        {
            if (_sessionStorage.TryGetValue(key, out var storedValue))
            {
                value = storedValue;
                return true;
            }
            value = null;
            return false;
        }
    }
}

