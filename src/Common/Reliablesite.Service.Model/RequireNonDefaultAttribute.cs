using System;
using System.ComponentModel.DataAnnotations;

namespace Reliablesite.Service.Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class RequireNonDefaultAttribute : ValidationAttribute
    {
        public RequireNonDefaultAttribute()
            : base("The {0} field requires a non-default value.")
        {
        }

        public override bool IsValid(object value)
        {
            if (value is null)
                return false;

            var type = value.GetType();
            return !Equals(value, Activator.CreateInstance(Nullable.GetUnderlyingType(type) ?? type));
        }
    }
}
