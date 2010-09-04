#region GPL_FILE_HEADER
/* Stats.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

Stats.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

Stats.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with Stats.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.Collections;

namespace mcww
{
	public class ExtendedStatsElement
	{
		public double successrate;
		public double meanRaw;
		public double medianRaw;
		public double stdRaw;
		public double outlier;
		public double meanMinusOutliers;
		public double medianMinusOutliers;
		public double stdMinusOutliers;
	}

	/// <summary>
	/// Statistics object.
	/// </summary>
	public class Stats
	{
		#region NUMERIC_COMPARERS
		private class IndividualTestComparer : IComparer
		{
			public int Compare(object a, object b)
			{
				IndividualTest a1 = a as IndividualTest;
				IndividualTest b1 = b as IndividualTest;
				double x1 = (double)a1.responseTime;
				double x2 = (double)b1.responseTime;
				if (x1 < x2)
					return -1;
				else if (x1 == x2)
					return 0;
				return 1;
			}
		}

		private class DoubleComparer : IComparer
		{
			public int Compare(object a, object b)
			{
				double x1 = (double)a;
				double x2 = (double)b;
				if (x1 < x2)
					return -1;
				else if (x1 == x2)
					return 0;
				return 1;
			}
		}
		#endregion

		private ArrayList sourceData;
		private ArrayList outputData = new ArrayList();
		private ArrayList fullData = new ArrayList();
		private ArrayList clusterData = new ArrayList();
		private IndividualTestComparer individualTestComparer = new IndividualTestComparer();
		private DoubleComparer doubleComparer = new DoubleComparer();
		private const int maxGroups = 10;

		public Stats()
		{
			#region test
			/*
			ArrayList temp = new ArrayList();
			temp.Add(34.2);
			temp.Add(25.16);
			//temp.Add(4.3);
			//temp.Add(0.0);
			temp.Add(18.0);
			temp.Add(64.0);
			temp.Add(16.4);
			temp.Add(11.23);
			//temp.Add(0.23);
			temp.Add(42.0);

			temp.Sort(doubleComparer);

			ArrayList list = new ArrayList();
			foreach (double d in temp)
			{
				IndividualTest it = new IndividualTest();
				it.responseTime = d;
				it.fail = false;
				list.Add(it); 
			}
			list.Sort(numericComparer);

			ArrayList cluster;
			cluster = ComputeDataClusters(temp, 4);

			temp.Add(99);
			*/
			#endregion
		}

		public void SetRawData(ArrayList sourceData)
		{
			this.sourceData = sourceData;
			GenerateStats();
		}

		public ArrayList GetSummaryData()
		{
			return outputData;
		}

		public ArrayList GetSuccessRateData()
		{
			ArrayList arr = new ArrayList();
			foreach (ExtendedStatsElement d in outputData)
			{
				arr.Add(d.successrate);
			}
			return arr;
		}

		public ArrayList GetClusteredDataSet()
		{
			return clusterData;
		}

		private void GenerateStats()
		{
			outputData.Clear();

			ArrayList data = new ArrayList();
			foreach (TestDataElement d1 in sourceData)
			{
				data.Add(d1);
			}

			foreach (TestDataElement d2 in data)
			{
				d2.responseTimes.Sort(individualTestComparer);

				ExtendedStatsElement elm = new ExtendedStatsElement();
				elm.successrate = ComputeSuccessRate(d2.responseTimes);
				elm.meanRaw = ComputeMeanRaw(d2.responseTimes);
				elm.medianRaw = ComputeMedianRaw(d2.responseTimes);
				elm.stdRaw = ComputeSTDRaw(d2.responseTimes);
				elm.outlier = ComputeOutlier(d2.responseTimes);
				elm.meanMinusOutliers = ComputeMedianMinusOutlier(d2.responseTimes, elm.outlier);
				elm.medianMinusOutliers = ComputeMedianMinusOutlier(d2.responseTimes, elm.outlier);
				elm.stdMinusOutliers = ComputeSTDMinusOutlier(d2.responseTimes, elm.outlier);

				outputData.Add(elm);
			}

			foreach (TestDataElement t in sourceData)
			{
				foreach (IndividualTest it in t.responseTimes)
				{
					fullData.Add(it.responseTime);
				}
			}
			fullData.Sort(doubleComparer);
			if (fullData.Count > 0)
				clusterData = ComputeDataClusters(fullData, maxGroups);
		}

		private double ComputeMeanRaw(ArrayList list)
		{
			if (list.Count == 0)
				return 0.0;
			double tot = 0.0;
			foreach (IndividualTest d in list)
			{
				tot += d.responseTime;
			}
			tot = tot / list.Count;
			return tot;
		}

		private double ComputeMedianRaw(ArrayList list)
		{
			if (list.Count == 0)
				return 0.0;

			double ret;
			int isEven = (list.Count % 2) - 1;
			if (isEven == -1)
			{
				IndividualTest d1, d2;
				int mid = list.Count / 2;
				d1 = list[mid-1] as IndividualTest;
				d2 = list[mid] as IndividualTest;
				ret = (d1.responseTime + d2.responseTime) / 2.0;
			}
			else
			{
				IndividualTest d;
				d = list[list.Count / 2] as IndividualTest;
				ret = d.responseTime;
			}
			return ret;
		}

		private double ComputeSTDRaw(ArrayList list)
		{
			return ComputeSTD(list);
		}

		private double ComputeSTD(ArrayList list)
		{
			if (list.Count == 0)
				return 0.0;
			double mean = ComputeMeanRaw(list);
			double sqrdiv = 0.0;
			double prestd = 0.0;
			double std;
			foreach (IndividualTest d in list)
			{
				sqrdiv = (d.responseTime - mean);
				sqrdiv *= sqrdiv;
				prestd += sqrdiv;
			}
			std = Math.Sqrt(prestd / (list.Count-1));
			return std;
		}

		private double ComputeOutlier(ArrayList list)
		{
			double mean = ComputeMeanRaw(list);
			double std = ComputeSTD(list);	
			return (mean + (2 * std));
		}

		private double ComputeMeanMinusOutlier(ArrayList list, double outlier)
		{
			double tot = 0.0;
			double num = 0.0;
			double cnt = 0;
			foreach (IndividualTest d in list)
			{
				num = d.responseTime;
				if (num <= outlier)
				{
					tot += num;
					cnt++;
				}
			}
			tot = tot / cnt;
			return tot;
		}

		private double ComputeMedianMinusOutlier(ArrayList list, double outlier)
		{
			if (list.Count < 2)
				return 0.0;
			ArrayList data = new ArrayList();
			double num = 0.0;
			foreach (IndividualTest d in list)
			{
				num = d.responseTime;
				if (num <= outlier)
					data.Add(d);
			}
			IndividualTest it = data[data.Count / 2] as IndividualTest;
			return it.responseTime;
		}

		private double ComputeSTDMinusOutlier(ArrayList list, double outlier)
		{
			ArrayList data = new ArrayList();
			double num = 0.0;
			foreach (IndividualTest d in list)
			{
				num = d.responseTime;
				if (num <= outlier)
					data.Add(d);
			}
			return ComputeSTD(data);
		}

		private double ComputeSuccessRate(ArrayList list)
		{
			double success = 0.0;
			foreach (IndividualTest d in list)
			{
				if (d.fail == false)
					success++;
			}
			if (success == 0)
				return 0.0;
			return (success / list.Count) * 100.0;
		}

		private ArrayList ComputeDataClusters(ArrayList list, int clusters)
		{
			ArrayList clusterData = new ArrayList();

			// determine data range
			double max = (double)list[list.Count-1];
			double min = (double)list[0];
			double range = max - min;

			// determine increment (intevals)
			double incr = range / clusters;
	
			// setup cluster data structure
			for (int i=0; i < maxGroups+1; i++)
				clusterData.Add(new ArrayList());

			double distance = 0.0;
			double minDistance = double.MaxValue;
			int idx = 0;
			foreach (double d in list)
			{
				idx = 0;
				minDistance = double.MaxValue;
				for (int i=0; i<maxGroups; i++)
				{
					distance = ((double)Math.Abs(d - (incr*i)));
					if (distance < minDistance)
					{
						minDistance = distance;
						idx = i;
					}
				}
				((ArrayList)clusterData[idx]).Add(d);
			}

			// remove unused entries
			int j;
			while (true)
			{
				for (j=0; j < clusterData.Count; j++)
				{
					ArrayList l = clusterData[j] as ArrayList;
					if (l.Count == 0)
					{
						clusterData.RemoveAt(j);
						break;
					}
				}
				if (j == clusterData.Count)
					break;
			}
			return clusterData;
		}

	}
}

