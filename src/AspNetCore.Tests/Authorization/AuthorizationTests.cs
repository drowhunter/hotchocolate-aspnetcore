
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HotChocolate.AspNetCore.Subscriptions
{
    public class AuthorizationTests
        : IClassFixture<TestServerFactory>
    {
        public AuthorizationTests(TestServerFactory testServerFactory)
        {
            TestServerFactory = testServerFactory;
        }


        private TestServerFactory TestServerFactory { get; }




    }

    public class MinimumAgeRequirement
        : IAuthorizationRequirement
    {
        public int MinimumAge { get; private set; }

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }
    }
}
