﻿using Habr.Common.Exceptions.Base;

namespace Habr.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message, 404) { }
}
