namespace Baseline
{
    public interface IFlatFileWriter
    {
        void WriteProperty(string name, string value);
        void WriteLine(string line);
        void Sort();
    }
}