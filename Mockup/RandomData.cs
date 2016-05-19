using System;
using System.Collections.Generic;
using System.IO;

public class RandomData
{
	private static Random random = new Random(1);

	public static void Main(string[] args)
	{
		new RandomData("sampleData.csv");
	}
	
	public RandomData(string filename)
	{
		int lineCount = 100;
		List<DateTime> dates = DateProgression(lineCount, new DateTime(2016, 5, 19), 0.4F);
		List<DateTime> times = RandomTimes(lineCount);
		List<int> basic0_100 = RandomInts(lineCount, 0, 100);
		List<int> basic32_110 = RandomInts(lineCount, 32, 110);
		List<int> basic0_1000 = RandomInts(lineCount, 0, 1000);
		List<int> coinToss = RandomInts(lineCount, 0, 1);
		List<int> gaussian = RandomGaussian(lineCount, 50, 1);
		
		using(StreamWriter writer = new StreamWriter(filename))
		{
			writer.WriteLine("Date,Time,Uniform 0 To 100,Temperature,Uniform 0 To 1000,Coin Toss,Gaussian");
			for(int i=0; i<lineCount; i++)
			{
				writer.WriteLine("{0:MM/dd/yy},{1:HH:mm:ss},{2},{3},{4},{5},{6}",dates[i],times[i],basic0_100[i],basic32_110[i],basic0_1000[i],coinToss[i],"todo");
			}
		}
	}
	
	private List<DateTime> DateProgression(int count, DateTime currentDate, float chanceDateProgresses)
	{
		List<DateTime> list = new List<DateTime>();
		for(int i=0; i<count; i++)
		{
			list.Add(currentDate);
			if(random.Next(0,100) <= chanceDateProgresses * 100)
				currentDate = currentDate.AddDays(1);
		}
		return list;
	}
	
	private List<DateTime> RandomTimes(int count)
	{
		List<DateTime> list = new List<DateTime>();
		for(int i=0; i<count; i++)
		{
			list.Add(new DateTime(2016, 5, 19, random.Next(0,24), random.Next(0,60), random.Next(0,60)));
		}
		return list;
	}
	
	private List<int> RandomInts(int count, int min, int max)
	{
		List<int> list = new List<int>();
		for(int i=0; i<count; i++)
		{
			list.Add(random.Next(min, max+1));
		}
		return list;
	}
	
	private double CauchyQuantile(double p)
	{
		return Math.Tan(Math.PI * (p - 0.5));
	}

	private List<int> RandomGaussian(int count, int mean, float standardDeviation)
	{
		List<int> list = new List<int>();
		for(int i=0; i<count; i++)
		{
			list.Add(BoxMullerTransform(mean, standardDeviation));
		}
		return list;
	}
	
	private int BoxMullerTransform(int mean, float standardDeviation)
	{
		double a = random.NextDouble();
		double b = random.NextDouble();
		int randomStandardNormal = (int)(System.Math.Sqrt(-2.0 * System.Math.Log(1)) * System.Math.Sin(2.0 * System.Math.PI * b));
		return  (int)(mean + standardDeviation * randomStandardNormal);
	}
}
