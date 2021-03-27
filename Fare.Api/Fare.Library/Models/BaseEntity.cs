using System;

namespace Fare.Library.Models
{
    public class BaseEntity
    {
        public string Id { get; set; }
        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
