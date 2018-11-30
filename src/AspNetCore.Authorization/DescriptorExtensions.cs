using System;
using System.Collections.Generic;
using System.Text;
using HotChocolate.Types;

namespace HotChocolate.AspNetCore
{
    public static class ObjectTypeDescriptorExtensions
    {
        public static IObjectTypeDescriptor Authorize(this IObjectTypeDescriptor self, string policy)
        {
            return self.Directive(new AuthorizeDirective(policy));
        }
        
        public static IObjectFieldDescriptor Authorize(this IObjectFieldDescriptor self, string policy)
        {
            return self.Directive(new AuthorizeDirective(policy));
        }

    }
}
