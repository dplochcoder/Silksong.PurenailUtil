using System.Collections.Generic;

namespace Silksong.PurenailUtil.Collections;

public static class EmptyCollection<T>
{
    public static readonly IReadOnlyList<T> Instance = [];
}
