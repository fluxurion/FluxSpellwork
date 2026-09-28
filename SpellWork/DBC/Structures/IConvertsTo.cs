namespace SpellWork.DBC.Structures
{
    public interface IConvertsTo<out T>
    {
        T ToCanonical();
    }
}
