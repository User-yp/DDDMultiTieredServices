namespace Commons.OperateHelper;

public static class NormalRandomHelper
{
    private static Random random=new Random();

    public static List<double> GetDoubles(int num, double mean = 0, double stdDev = 1)
    {
        List<double> doubles = [];
        for (int i = 0; i < num; i++)
        {
            doubles.Add(NextGaussian(mean, stdDev));
        }
        return doubles;
    }

    public static double NextGaussian( double mean = 0, double stdDev = 1)
    {
        double u1 = random.NextDouble(); // 随机数 [0,1)
        double u2 = random.NextDouble();

        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // 标准正态分布（均值0，标准差1）
        double randNormal = mean + stdDev * randStdNormal; // 转换为指定均值和标准差的正态分布
        return randNormal;
    }
}
