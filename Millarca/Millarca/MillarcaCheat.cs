namespace Millarca
{
    internal abstract class MillarcaCheat
    {
        public string Label { get; protected set; }
        public virtual string LabelSuffix { get; set; }

        protected MillarcaCheat(string label)
        {
            Label = label;
        }

        public virtual void HandleEnter() { }
        public virtual void HandleLeft() { }
        public virtual void HandleRight() { }

        public override string ToString() => Label;
    }
}
