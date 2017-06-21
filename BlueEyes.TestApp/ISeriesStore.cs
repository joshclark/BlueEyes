namespace BlueEyes.TestApp
{
    interface ISeriesStore
    {
        string Name { get; }
        byte[] SavePoints(DataPoint[] dataPoints);

        DataPoint[] LoadPoints(byte[] buffer);
    }
}