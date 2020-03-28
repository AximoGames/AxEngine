namespace Aximo
{

    public interface IVisitor
    {
        void Visit(IVisitorNode node);
    }

    public interface IVisitorNode
    {
        void Accept(IVisitor visitor);
    }

}