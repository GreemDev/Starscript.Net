namespace Starscript.Internal;

public abstract class Expr : IExprVisitable
{
    public int Start { get; }
    public int End { get; }
    public Expr? Parent { get; internal set; }
    public Expr[] Children { get; }
    
    public abstract string ExprName { get; }

    public abstract void Accept(IExprVisitor visitor);

    public Expr(int start, int end) : this(start, end, []) { }
    public Expr(int start, int end, Expr[] children)
    {
        Start = start;
        End = end;
        Children = children;

        foreach (var child in Children)
        {
            child.Parent = this;
        }
    }

    public string GetSource(string source) => source[Start..End];

    public void ReplaceChild(Expr toReplace, Expr replacement)
    {
        for (int i = 0; i < Children.Length; i++)
        {
            if (Children[i] != toReplace) continue;

            Children[i] = replacement;
            toReplace.Parent = null;
            replacement.Parent = this;

            break;
        }
    }

    public void Replace(Expr replacement) => Parent?.ReplaceChild(this, replacement);

    #region Implementations

    public class Null : Expr
    {
        public override string ExprName => nameof(Null);

        public Null(int start, int end) : base(start, end)
        {
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }

    public class String : Expr
    {
        public override string ExprName => nameof(String);
        
        public string Value { get; }
        
        public String(int start, int end, string value) : base(start, end)
        {
            Value = value;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }

    public class Number : Expr
    {
        public override string ExprName => nameof(Number);
        
        public double Value { get; }
        
        public Number(int start, int end, double value) : base(start, end)
        {
            Value = value;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Boolean : Expr
    {
        public override string ExprName => nameof(Boolean);
        
        public bool Value { get; }
        
        public Boolean(int start, int end, bool value) : base(start, end)
        {
            Value = value;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Block : Expr
    {
        public override string ExprName => nameof(Block);
        
        public Expr? Expr => Children.FirstOrDefault();
        
        public Block(int start, int end, Expr? expr) : base(start, end, expr != null ? [ expr ] : [])
        {
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Group : Expr
    {
        public override string ExprName => nameof(Group);
        
        public Expr Expr => Children.First();
        
        public Group(int start, int end, Expr expr) : base(start, end,[ expr ])
        {
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }

    public class Binary : Expr
    {
        public override string ExprName => nameof(Binary);
        
        public Token Operator { get; }

        public Expr Left => Children.First();
        public Expr Right => Children.Last();

        public Binary(int start, int end, Expr left, Token op, Expr right) : base(start, end, [left, right])
        {
            Operator = op;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Unary : Expr
    {
        public override string ExprName => nameof(Unary);
        
        public Token Operator { get; }
        
        public Expr Right => Children.First();

        public Unary(int start, int end, Token op, Expr right) : base(start, end, [right])
        {
            Operator = op;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Variable : Expr
    {
        public override string ExprName => nameof(Variable);
        
        public string Name { get; }
        
        public Variable(int start, int end, string name) : base(start, end)
        {
            Name = name;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Get : Expr
    {
        public override string ExprName => nameof(Get);
        
        public string Name { get; }
        public Expr Object => Children.First();
        
        public Get(int start, int end, Expr obj, string name) : base(start, end, [obj])
        {
            Name = name;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }

    public class Call : Expr
    {
        public override string ExprName => nameof(Call);
        
        public Expr Callee => Children.First();
        public int ArgCount => Children.Length - 1;
        public Expr GetArg(int idx) => Children[idx + 1];

        public IEnumerable<Expr> Arguments => Children.Skip(1);
        
        public Call(int start, int end, Expr callee, IEnumerable<Expr> args) : base(start, end, args.Prepend(callee).ToArray())
        {
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Logical : Expr
    {
        public override string ExprName => nameof(Logical);
        
        public Token Operator { get; }

        public Expr Left => Children.First();
        public Expr Right => Children.Last();

        public Logical(int start, int end, Expr left, Token op, Expr right) : base(start, end, [left, right])
        {
            Operator = op;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }
    
    public class Conditional : Expr
    {
        public override string ExprName => nameof(Conditional);
        
        public Expr Condition => Children.First();
        public Expr TrueBranch => Children[1];
        public Expr FalseBranch => Children.Last();

        public Conditional(int start, int end, Expr condition, Expr trueBranch, Expr falseBranch) : base(start, end, [condition, trueBranch, falseBranch])
        {
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }

    public class Section : Expr
    {
        public override string ExprName => nameof(Section);

        public int Index { get; }
        public Expr Expr => Children.First();
        
        public Section(int start, int end, int index, Expr expr) : base(start, end, [expr])
        {
            Index = index;
        }

        public override void Accept(IExprVisitor visitor) => visitor.Visit(this);
    }

    #endregion
}