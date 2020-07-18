using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Golap.AppleAuth.Tests.Core
{
    internal class DelegatingHandlerStub : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
        public DelegatingHandlerStub(HttpResponseMessage response)
        {
            _handlerFunc = (request, cancellationToken) => Task.FromResult(response);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _handlerFunc(request, cancellationToken);
        }
    }
}
