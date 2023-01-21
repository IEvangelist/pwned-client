// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.AcceptanceTests;

/// <summary>
/// Integration test accounts from the "have i been pwned" API.
/// See <a href="https://haveibeenpwned.com/API/v3#TestAccounts"></a>
/// </summary>
internal static class TestAccounts
{
    /// <summary>
    /// Returns one breach and one paste.
    /// </summary>
    internal const string AccountExists =
        "account-exists@hibp-integration-tests.com";

    /// <summary>
    /// Returns three breaches.
    /// </summary>
    internal const string MultipleBreaches =
        "multiple-breaches@hibp-integration-tests.com";

    /// <summary>
    /// Returns one breach being "Adobe". An inactive breach also exists
    /// against this account in the underlying data structure.
    /// </summary>
    internal const string NotActiveAndActiveBreach =
        "not-active-and-active-breach@hibp-integration-tests.com";

    /// <summary>
    /// Returns no breaches. An inactive data breach also exists against
    /// this account in the underlying data structure.
    /// </summary>
    internal const string NotActiveBreach =
        "not-active-breach@hibp-integration-tests.com";

    /// <summary>
    /// Returns no breaches and no pastes. This account is opted-out of
    /// both pastes and breaches in the underlying data structure.
    /// </summary>
    internal const string OptOut =
        "opt-out@hibp-integration-tests.com";

    /// <summary>
    /// Returns no breaches and no pastes. This account is opted-out of
    /// breaches in the underlying data structure.
    /// </summary>
    internal const string OptOutBreach =
        "opt-out-breach@hibp-integration-tests.com";

    /// <summary>
    /// Returns no breaches and one paste. A sensitive breach exists against
    /// this account in the underlying data structure.
    /// </summary>
    internal const string PasteSensitiveBreach =
        "paste-sensitive-breach@hibp-integration-tests.com";

    /// <summary>
    /// Returns no breaches and no pastes. This account is permanently
    /// opted-out of both breaches and pastes in the underlying data structure.
    /// </summary>
    internal const string PermanentOptOut =
        "permanent-opt-out@hibp-integration-tests.com";

    /// <summary>
    /// Returns two non-sensitive breaches and no pastes. A sensitive breach
    /// exists against this account in the underlying data structure.
    /// </summary>
    internal const string SensitiveAndOtherBreaches =
        "sensitive-and-other-breaches@hibp-integration-tests.com";

    /// <summary>
    /// Returns no breaches and no pastes. A sensitive breach exists against
    /// this account in the underlying data structure.
    /// </summary>
    internal const string SensitiveBreach =
        "sensitive-breach@hibp-integration-tests.com";

    /// <summary>
    /// Returns one unverified breach and no pastes.
    /// </summary>
    internal const string UnverifiedBreach =
        "unverified-breach@hibp-integration-tests.com";
}
