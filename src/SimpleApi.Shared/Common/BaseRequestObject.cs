using System;

namespace SimpleApi.Shared.Common;

public abstract class BaseRequestObject
{
    public Guid? Id { get; set; }
}