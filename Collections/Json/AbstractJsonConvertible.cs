using System;

namespace Silksong.PurenailUtil.Collections.Json;

public abstract class AbstractJsonConvertible
{
    internal abstract Type GetRepType();

    internal abstract object ConvertToRepRaw();

    internal abstract void ReadRepRaw(object value);
}

public abstract class AbstractJsonConvertible<RepT> : AbstractJsonConvertible where RepT : class
{
    internal override Type GetRepType() => typeof(RepT);

    internal override object ConvertToRepRaw() => ConvertToRep();

    internal abstract RepT ConvertToRep();

    internal override void ReadRepRaw(object value) => ReadRep((RepT)value);

    internal abstract void ReadRep(RepT value);
}
