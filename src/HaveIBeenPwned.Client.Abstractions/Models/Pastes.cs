// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Abstractions;

/// <summary>
/// See <a href="https://haveibeenpwned.com/API/v3#PasteModel"></a>
/// </summary>
public sealed class Pastes
{
    /// <summary>
    /// The paste service the record was retrieved from.
    /// Current values are:
    /// Pastebin, Pastie, Slexy, Ghostbin, QuickLeak, JustPaste, AdHocUrl, PermanentOptOut, OptOut.
    /// </summary>
    public string Source { get; set; } = null!;

    /// <summary>
    /// The ID of the paste as it was given at the source service.
    /// Combined with the <see cref="Source"/> attribute, this can be used to resolve the URL of the paste.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// The title of the paste as observed on the source site.
    /// This may be <see langword="null" /> and if so will be omitted from the response.
    /// </summary>
    public string? Title { get; set; } = null!;

    /// <summary>
    /// The date and time (precision to the second) that the paste was posted.
    /// This is taken directly from the paste site when this information is available but may be null if no date is published.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// The number of emails that were found when processing the paste.
    /// Emails are extracted by using the regular expression <c>\b[a-zA-Z0-9\.\-_\+]+@[a-zA-Z0-9\.\-_]+\.[a-zA-Z]+\b</c>.
    /// </summary>
    public int EmailCount { get; set; }
}
