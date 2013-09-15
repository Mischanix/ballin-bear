using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Cecil;
using LinFu.AOP.Interfaces;
using LinFu.AOP.Cecil;
using LinFu.AOP.Cecil.Interfaces;

namespace Our {
    class AroundLog : IAroundInvoke {
        public void BeforeInvoke(IInvocationInfo info) {
            Util.Log("{0}", info.Arguments);
        }

        public void AfterInvoke(IInvocationInfo info, object returnValue) {
            return;
        }
    }

    class SimpleAroundInvokeProvider : IAroundInvokeProvider {
        AroundLog around;

        public SimpleAroundInvokeProvider() {
            around = new AroundLog();
        }

        public IAroundInvoke GetSurroundingImplementation(IInvocationInfo context) {
            if (context.TargetMethod.Name.Substring(0, 3) == "Log")
                return around;

            return null;
        }
    }
}
