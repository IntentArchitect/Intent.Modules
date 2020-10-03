using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.TypeScript.Editor;
using Intent.Modules.Common.TypeScript.Editor.Parsing;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.React.Editor
{
    public class TsxFileEditor : TypeScriptFileEditor
    {
        public TsxFileEditor(string source) : base(source)
        {
        } 

        public override TypeScriptFileTreeWalker GetTreeWalker(TypeScriptNode node)
        {
            return new TsxFileTreeWalker(node, this);
        }
    }

    public class TsxFileTreeWalker : TypeScriptFileTreeWalker
    {
        public TsxFileTreeWalker(TypeScriptNode node, TsxFileEditor editor) : base(node, editor)
        {
        }

        protected override TypeScriptNode CreateMethod(Node node, TypeScriptNode parent)
        {
            return new TxsMethod(node, parent);
        }
    }

    public class TxsMethod : TypeScriptMethod
    {
        public TxsMethod(Node node, TypeScriptNode parent) : base(node, parent)
        {
        }

        public override void UpdateNode(Node node)
        {
            base.UpdateNode(node);
        }
    }
}
