using System;
using System.Collections.Generic;
using System.Text;

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
