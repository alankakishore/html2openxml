/* Copyright (C) Olivier Nizet https://github.com/onizet/html2openxml - All Rights Reserved
 * 
 * This source is subject to the Microsoft Permissive License.
 * Please see the License.txt file for more information.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 * PARTICULAR PURPOSE.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HtmlToOpenXml;

/// <summary>
/// Helper class that provide some extension methods to <see cref="IEnumerable{T}"/> API.
/// </summary>
static class CollectionExtensions
{
    /// <summary>
    /// Executes a <c>for-each</c> operation on an <see cref="IEnumerable{T}" /> in which iterations may run in parallelc.
    /// </summary>
    public static Task ForEachAsync<T>(this IEnumerable<T> source, 
#if NET5_0_OR_GREATER
        Func<T, CancellationToken, ValueTask> asyncAction,
#else
        Func<T, CancellationToken, Task> asyncAction,
#endif
        ParallelOptions parallelOptions)
    {
#if NET5_0_OR_GREATER
        return Parallel.ForEachAsync(source, parallelOptions, asyncAction);
#else
        var throttler = new SemaphoreSlim(initialCount: Math.Max(1, parallelOptions.MaxDegreeOfParallelism));
        var tasks = System.Linq.Enumerable.Select(source, async item =>
        {
            await throttler.WaitAsync(parallelOptions.CancellationToken);
            if (parallelOptions.CancellationToken.IsCancellationRequested) return;

            try
            {
                await asyncAction(item, parallelOptions.CancellationToken).ConfigureAwait(false);
            }
            finally
            {
                throttler.Release();
            }
        });
        return Task.WhenAll(tasks);
#endif
    }

#if NET462
    /// <summary>
    /// Adds a value to the end of the sequence (fallback method for .Net 4.6.2).
    /// </summary>
    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
    {
         if (source == null)
            throw new ArgumentNullException(nameof(source));

        var list = new List<TSource>(source);
        list.Add(element);
        return list;
    }
#endif
}
