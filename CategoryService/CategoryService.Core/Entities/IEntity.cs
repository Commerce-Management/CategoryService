using System;

namespace CategoryService.Core.Entities;

public interface IEntity
{
    Guid Id { get; set; }
}