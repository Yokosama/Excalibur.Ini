namespace Excalibur.Ini
{
    public interface ICloneable<T> where T : class
    {
        T Clone();
    }
}
