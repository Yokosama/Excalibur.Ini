namespace Excalibur.Ini
{
    /// <summary>
    /// 泛型拷贝对象T的接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICloneable<T> where T : class
    {
        /// <summary>
        /// 拷贝类型T的对象
        /// </summary>
        /// <returns>T类型的对象</returns>
        T Clone();
    }
}
