// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Authentication;

namespace System.Net
{
    internal static class SecurityProtocol
    {
        public const SslProtocols DefaultSecurityProtocols =
            // SP_PROT_TLS1_3 / SslProtocols.Tls13 in netcoreapp3.0
            ((SslProtocols)12288) |
            SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;

        public const SslProtocols SystemDefaultSecurityProtocols = SslProtocols.None;
    }
}
