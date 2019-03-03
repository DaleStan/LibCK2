﻿//Modified from: dotnet/corefx
//Original license reproduced below
//  https://github.com/dotnet/corefx/blob/master/LICENSE.TXT @ 2019-03-02
/*
    The MIT License (MIT)

    Copyright (c) .NET Foundation and Contributors

    All rights reserved.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace LibCK2.Parsing
{
    internal static class BufferExtensions
    {
        /// <summary>
        /// Returns position of first occurrence of item in the <see cref="ReadOnlySequence{T}"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static SequencePosition? PositionOfAny<T>(in this ReadOnlySequence<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        {
            if (source.IsSingleSegment)
            {
                int index = source.First.Span.IndexOfAny(values);
                if (index != -1)
                {
                    return source.GetPosition(index);
                }

                return null;
            }
            else
            {
                return PositionOfAnyMultiSegment(source, values);
            }
        }

        private static SequencePosition? PositionOfAnyMultiSegment<T>(in ReadOnlySequence<T> source, ReadOnlySpan<T> values) where T : IEquatable<T>
        {
            SequencePosition position = source.Start;
            SequencePosition result = position;
            while (source.TryGet(ref position, out ReadOnlyMemory<T> memory))
            {
                int index = memory.Span.IndexOfAny(values);
                if (index != -1)
                {
                    return source.GetPosition(index, result);
                }
                else if (position.GetObject() == null)
                {
                    break;
                }

                result = position;
            }

            return null;
        }
    }
}
