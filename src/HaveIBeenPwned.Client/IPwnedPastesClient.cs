// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using HaveIBeenPwned.Client.Models;

namespace HaveIBeenPwned.Client
{
    public interface IPwnedPastesClient
    {
        /// <summary>
        /// The API takes a single parameter which is the email address to be searched for.
        /// The email is not case sensitive and will be trimmed of leading or trailing white spaces.
        /// The email should always be URL encoded.
        /// This is an authenticated API and an HIBP API key must be passed with the request.
        /// See <a href="https://haveibeenpwned.com/API/v3#PastesForAccount"></a>
        /// </summary>
        /// <param name="account"></param>
        /// <returns>An array of <see cref="Pastes"/> if found, otherwise an empty array.</returns>
        /// <exception cref="ArgumentNullException">
        /// When the <paramref name="account"/> is <c>null</c>, empty or whitespace.
        /// </exception>
        Task<Pastes[]> GetPastesAsync(string account);
    }
}
