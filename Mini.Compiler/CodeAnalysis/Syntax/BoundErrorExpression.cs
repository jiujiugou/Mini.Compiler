using Mini.Compiler.CodeAnalysis.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini.Compiler.CodeAnalysis.Syntax
{
    /// <summary>
    /// 专门用于表示绑定阶段的错误表达式
    /// </summary>
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public override BoundNodeKind Kind => BoundNodeKind.ErrorExpression;
        public override Type Type => typeof(object);
    }
}
