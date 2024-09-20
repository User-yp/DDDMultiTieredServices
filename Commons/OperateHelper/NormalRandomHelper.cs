namespace Commons.OperateHelper;

public static class NormalRandomHelper
{
    private static Random random=new Random();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="num">数据条数</param>
    /// <param name="mean">均值</param>
    /// <param name="stdDev">标准差</param>
    /// <returns></returns>
    public static List<double> GetNormalDoubles(int num, double mean = 0, double stdDev = 1)
    {
        List<double> doubles = [];
        for (int i = 0; i < num; i++)
        {
            doubles.Add(NextGaussian(mean, stdDev));
        }
        return doubles;
    }

    private static double NextGaussian( double mean = 0, double stdDev = 1)
    {
        double u1 = random.NextDouble(); // 随机数 [0,1)
        double u2 = random.NextDouble();

        // 标准正态分布（均值0，标准差1）
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        // 转换为指定均值和标准差的正态分布
        double randNormal = mean + stdDev * randStdNormal; 
        return randNormal;
    }
}
