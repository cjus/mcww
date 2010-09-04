#region GPL_FILE_HEADER
/* ReportGenerator.cs
   Copyright (C) 2005 Carlos Justiniano
   cjus@chessbrain.net, cjus34@yahoo.com, cjus@users.sourceforge.net

ReportGenerator.cs is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License as
published by the Free Software Foundation; either version 2 of the
License, or (at your option) any later version.

ReportGenerator.cs was developed by Carlos Justiniano for use on the
msgCourier project and the ChessBrain Project and is now distributed in
the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A
PARTICULAR PURPOSE.  See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License
along with ReportGenerator.cs; if not, write to the Free Software 
Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
*/
#endregion

using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using ZedGraph;

namespace mcww
{
	/// <summary>
	/// Summary description for ReportGenerator.
	/// </summary>
	public class ReportGenerator
	{
		private ArrayList testDataList;
		private string reportPath = "";
		private MainForm mainForm;
		private Stats statsEngine;
		private StringBuilder rangeTable = null;
		private StringBuilder successTable = null;
		private StringBuilder distTable = null;
		private StringBuilder statSummaryTable = null;

		#region NUMERIC_COMPARER
		private class NumericComparer : IComparer
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

		#region REPORT_CSS_FILE
		private string reportCSSFile = 
@"html, body 
{ 
	color: black; 
	font-family: Geneva, Arial, Helvetica, sans-serif;
	padding: 0px;
}

#container
{
	width: 100%;
	text-align: left;
}

p 
{ 
	font-size: 14px; 
	text-align: justify;
	line-height: 20px; 
	padding: 0px;
	margin-top: 12px; margin-bottom: 12px; 
	margin-left: 0px; margin-right: 0px; 
}

p.qoute
{ 
	font-size: 14px; 
	text-align: justify;
	line-height: 18px; 
	margin-top: 14px; margin-bottom: 14px; 
	margin-left: 10%; margin-right: 10%; 
}

p.warning
{ 
	font-size: 14px; 
	font-weight: bolder;
	color: #CC0000;
	text-align: justify;
	line-height: 18px; 
	text-decoration: underline;
	margin-top: 14px; margin-bottom: 14px; 
	margin-left: 10%; margin-right: 10%; 
}

p.note
{ 
	font-size: 12px; 
	text-align: justify;
	line-height: 16px; 
	margin-top: 14px; margin-bottom: 14px; 
	margin-left: 15%; margin-right: 15%; 

	background-color: #FFFFCC;
	padding: 8px;
	border-left: 1px solid #DDDDDD;
	border-top: 1px solid #DDDDDD;
	border-bottom: 1px solid #AAAAAA;
	border-right: 1px solid #999999;	
}

a:link 
{ 
	color: #003399; 
	text-decoration: none; 
}

a:visited 
{ 
	color: #003399; 
	text-decoration: none; 
} 

a:hover 
{ 
	color: #003399; 
	text-decoration: underline; 
}

h1
{ 
	color: #003366; 
	font-size: 24px; 
	font-weight: bold;
	margin-bottom: 8px; 
}

h2 
{ 
	color: #003366; 
	font-size: 20px; 
	font-weight: bold;
	margin-bottom: 8px; 
}

h3 
{ 
	color: #003366; 
	font-size: 16px; 
	font-weight: bold;
	margin-bottom: 8px; 
}

h4 
{ 
	color: #003366; 
	font-size: 14px; 
	font-weight: bold;
	margin-top: 2px; 
	margin-bottom: 4px; 
}

hr
{
	border-top: 1px dashed #aaaaaa;
	border-bottom: 1px dashed #FFFFFF;
	width: 80%
}

li
{
	font-size: 14px; 
	text-align: justify;
	line-height: 16px; 
	padding: 0px;
	margin-top: 4px; margin-bottom: 6px; 
	margin-left: 0px; margin-right: 0px; 
	padding-left: 0px;
}

pre.example
{
	background-color: #EEEEEE;
	margin-top: 10px; margin-bottom: 10px; 
	margin-left: 10px; margin-right: 10px; 
	padding: 4px;
	border-left: 1px solid #DDDDDD;
	border-top: 1px solid #DDDDDD;
	border-bottom: 1px solid #AAAAAA;
	border-right: 1px solid #999999;	
}

table 
{
	font-size: 12px; 
	background-color: #FFFFFF;
	width: 500px;
}
td
{
	padding-left: 5px;
	padding-right: 5px;
	vertical-align: middle;
}
td.s
{
	white-space: nowrap;
	vertical-align: middle;
	text-align: left;
}
td.m
{
	white-space: nowrap;
	vertical-align: middle;
	text-align: left;
}
td.l
{
	vertical-align: middle;
	text-align: left;
}
th
{
	font-weight:lighter;
	text-align: left;
	text-indent: 5px;	
	border-bottom: #ffffff 1px solid;
	background-color: #D4D4D4;
	border-left: 1px solid #DDDDDD;
	border-top: 1px solid #DDDDDD;
	border-bottom: 1px solid #666666;
	border-right: 1px solid #666666;
	padding-left: 1px;
	padding-right: 1px;
}
tr.odd
{
	font-weight:normal;
	text-indent: 5px;	
	background-color:#FFFFFF;
	border-collapse:collapse;
}
tr.even
{
	font-weight:normal;
	text-indent: 5px;	
	background-color:#EAEAEA;
	border-collapse:collapse;
}
";
	#endregion

		#region REPORT_HTML_TEMPLATE
		private string reportHTMLTemplate = 
@"
<html>
<head>
<title>{REPORTTITLE}</title>
<link rel='stylesheet' type='text/css' media='screen' href='report.css'>
</head>

<body>
<div id='container'>

<center>
<h2>{MAINTITLE}</h2>
<h4>{SUBTITLE}</h4>
</center>
<br>

<h3>Test Summary</h3>
<p>{TESTSUMMARY}</p>
<p>The raw test data is available at the <a href='#rawdata'>end</a> of this report.</p>
<center>
<h4>Summary Table</h4>
{SUMMARYTABLE}
</center>
<br>
<br>

<h3>Performance Graphs and Tables</h3>
<p>The Range Graph shows the range in response time (measured in milliseconds) for each test.  A blue colored bar displays the full range per test while the blue gradient area below the red line indicates the average response times.</p>
<center>
<h4>Range Graph</h4>
<img src='range.png' width='420px' height='280px'/>
<br>
<br>
<h4>Range Table</h4>
{RANGETABLE}
</center>

<br>
<br>
<center>
<p>The Success Graph illustrates the percentage ratio between successful requests and failures.</p>
<h4>Success Graph</h4>
<img src='success.png' width='420px' height='280px'/>
<br>
<br>
<h4>Success Table</h4>
{SUCCESSTABLE}
</center>

<br>
<br>
<center>
<p>The Distribution Graph shows the distribution of response times amoung all tests. Response times for request failures are excluded.</p>
<p>Darker circles represent more heavily populated groups.</p>
<h4>Distribution Graph</h4>
<img src='distribution.png' width='512px' height='384px'/>
<br>
<br>
<h4>Distribution Table</h4>
{DISTTABLE}
</center>

<a name='rawdata' id='rawdata'><h3>Raw Test Data</h3></a>
<p>The generated raw data of response times is shown below.</p>
<p class='note'>TIP: You can import the <a href='{DATAFILE}' target='_blank'>data file</a> shown below into Micrsoft Excel.</p>
<center><iframe src='sourcedata.txt' frameborder='1' style='width: 80%; height: 100px;' scrolling=yes marginwidth='0' marginheight='0'></iframe></center>

</div>
</body>
</html>
";
		#endregion

		private NumericComparer numericComparer = new NumericComparer();

		public ReportGenerator(MainForm mainForm)
		{
			statsEngine = new Stats();
			this.mainForm = mainForm;

			// ensure report has a subfolder
			reportPath = Directory.GetCurrentDirectory() + "\\report";
			Directory.CreateDirectory(reportPath);
		}

		public void SetRawData(ArrayList testDataList)
		{
			this.testDataList = testDataList;
			DumpRawData();
			statsEngine.SetRawData(testDataList);
		}

		public void GenerateReport(string title)
		{
			GenerateRangeGraphAndTable();
			GenerateSuccessGraphAndTable();
			GenerateDistributionGraphAndTable();
			string testSummary = GenerateTestSummaryText();

			// write CSS file
			FileStream file = new FileStream(reportPath + @"\report.css", FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(file);
			sw.Write(reportCSSFile);
			sw.Close();
			file.Close();

			// insert data into HTML template
			reportHTMLTemplate = reportHTMLTemplate.Replace("{REPORTTITLE}", title + " Report");
			reportHTMLTemplate = reportHTMLTemplate.Replace("{MAINTITLE}", title + " Report");
			reportHTMLTemplate = reportHTMLTemplate.Replace("{SUBTITLE}", "Generated: " + GetDate());
			reportHTMLTemplate = reportHTMLTemplate.Replace("{TESTSUMMARY}", testSummary);
			reportHTMLTemplate = reportHTMLTemplate.Replace("{SUMMARYTABLE}", statSummaryTable.ToString());
			reportHTMLTemplate = reportHTMLTemplate.Replace("{DATAFILE}", "file:///" + reportPath + "/sourcedata.txt");
			reportHTMLTemplate = reportHTMLTemplate.Replace("{RANGETABLE}", rangeTable.ToString());
			reportHTMLTemplate = reportHTMLTemplate.Replace("{SUCCESSTABLE}", successTable.ToString());			
			reportHTMLTemplate = reportHTMLTemplate.Replace("{DISTTABLE}", distTable.ToString());			
			

			// write HTML file
			file = new FileStream(reportPath + @"\report.html", FileMode.Create, FileAccess.Write);
			sw = new StreamWriter(file);
			sw.Write(reportHTMLTemplate);
			sw.Close();
			file.Close();

			mainForm.DisplayInWebPage("file:///" + reportPath + @"\report.html");
		}

		private void GenerateRangeGraphAndTable()
		{
			#region Write graph
			GraphPane graphPane = new GraphPane(new Rectangle(0, 0, 420, 280), "", "", "");

			graphPane.CurveList.Clear();
			graphPane.XAxis.Title = "Test Series";
			graphPane.YAxis.Title = "Response time in milliseconds";

			PointPairList list1 = new PointPairList();
			PointPairList hList = new PointPairList();

			int count = 0;
			foreach (TestDataElement d in testDataList)
			{
				count++;
				list1.Add(count, d.avgResponseTime);
				hList.Add(count, d.maxResponseTime, d.minResponseTime);
			}

			ErrorBarItem rangebar = graphPane.AddErrorBar("Range", hList, Color.Blue);
			rangebar.ErrorBar.PenWidth = 3;
			rangebar.ErrorBar.Symbol.IsVisible = false;

			LineItem curve = graphPane.AddCurve("Average", list1, Color.Red, SymbolType.Default);
			curve.Line.IsVisible = true;
			curve.Line.IsSmooth = false;
			curve.Symbol.Size = 4;
			curve.Symbol.Fill = new Fill(Color.Red);
			curve.Line.Fill = new Fill(Color.White, Color.LightSkyBlue, -45F);
			
			graphPane.XAxis.IsShowGrid = true;
			graphPane.YAxis.IsShowGrid = true;
			graphPane.XAxis.Max = testDataList.Count+1;

			SaveGraphImage(graphPane, reportPath + @"\range.png");
			#endregion

			#region Write table
			// write table header
			rangeTable = new StringBuilder();
			rangeTable.Append("<table><thead><tr>\n");
			rangeTable.Append("<th width=\"25%\">Thread</th>\n");
			rangeTable.Append("<th width=\"25%\">Mean response</th>\n");
			rangeTable.Append("<th width=\"25%\">Min response</th>\n");
			rangeTable.Append("<th width=\"25%\">Max response</th>\n");
			rangeTable.Append("</tr></thead><tbody>\n");
			count = 0;
			bool oddline = true;
			foreach (TestDataElement d in testDataList)
			{
				count++;
				if (oddline == true)
					rangeTable.Append("<tr class=\"odd\">\n");
				else
					rangeTable.Append("<tr class=\"even\">\n");

				rangeTable.Append("<td class=\"s\">");
				rangeTable.AppendFormat("{0}", count);
				rangeTable.Append("</td>\n");

				rangeTable.Append("<td class=\"s\">");
				rangeTable.AppendFormat("{0:f2}", d.avgResponseTime);
				rangeTable.Append("</td>\n");

				rangeTable.Append("<td class=\"s\">");
				rangeTable.AppendFormat("{0:f2}", d.minResponseTime);
				rangeTable.Append("</td>\n");

				rangeTable.Append("<td class=\"s\">");
				rangeTable.AppendFormat("{0:f2}", d.maxResponseTime);
				rangeTable.Append("</td>\n");
				oddline = !oddline;
			}
			rangeTable.Append("</tr></table>\n");
			#endregion
		}

		private void GenerateSuccessGraphAndTable()
		{
			#region Write graph
			GraphPane graphPane = new GraphPane(new Rectangle(0, 0, 420, 280), "", "", "");
			graphPane.CurveList.Clear();
			
			BarItem bar;

			// Set the bar type to percent stack, which makes the bars sum up to 100%
			graphPane.BarType = BarType.PercentStack;

			graphPane.XAxis.Title = "Test Series";
			graphPane.YAxis.Title = "Success rate";
			graphPane.YAxis.Max = 120;

			graphPane.XAxis.IsShowGrid = true;
			graphPane.YAxis.IsShowGrid = true;

			ArrayList SuccessData = statsEngine.GetSuccessRateData();
			graphPane.XAxis.Max = SuccessData.Count+1;

			int cnt = 0;
			double []ySuccess = new double[SuccessData.Count];
			double []yFailure = new double[SuccessData.Count];
			foreach (double d in SuccessData)
			{
				ySuccess[cnt] = d;
				yFailure[cnt] = 100 - d;
				cnt++;
			}

			// Add a gradient red bar
			bar = graphPane.AddBar("Failure",  null, yFailure, Color.Red);
			bar.Bar.Fill = new Fill(Color.Red, Color.White, Color.Red);

			// Add a gradient green bar
			bar = graphPane.AddBar("Success", null, ySuccess, Color.LightGreen);
			bar.Bar.Fill = new Fill(Color.LightGreen, Color.White, Color.LightGreen);

			SaveGraphImage(graphPane, reportPath + @"\success.png");
			#endregion

			#region Write table
			// write table header
			successTable = new StringBuilder();
			successTable.Append("<table><thead><tr>\n");
			successTable.Append("<th width=\"20%\">Thread</th>\n");
			successTable.Append("<th width=\"40%\">Success</th>\n");
			successTable.Append("<th width=\"40%\">Failure</th>\n");
			successTable.Append("</tr></thead><tbody>\n");
			bool oddline = true;
			for (int i=0; i<cnt; i++)
			{
				if (oddline == true)
					successTable.Append("<tr class=\"odd\">\n");
				else
					successTable.Append("<tr class=\"even\">\n");

				successTable.Append("<td class=\"s\">");
				successTable.AppendFormat("{0}", i+1);
				successTable.Append("</td>\n");

				successTable.Append("<td class=\"s\">");
				successTable.AppendFormat("{0}", ySuccess[i]);
				successTable.Append("</td>\n");

				successTable.Append("<td class=\"s\">");
				if (yFailure[i] != 0)
					successTable.AppendFormat("<b>{0}</b>", yFailure[i]);
				else
					successTable.AppendFormat("{0}", yFailure[i]);
				successTable.Append("</td>\n");

				oddline = !oddline;
			}
			successTable.Append("</tr></table>\n");
			#endregion
		}

		private void GenerateDistributionGraphAndTable()
		{
			#region Write graph
			GraphPane graphPane = new GraphPane(new Rectangle(0, 0, 512, 384), "", "", "");
			graphPane.CurveList.Clear();
			graphPane.Legend.Position = LegendPos.Right;
			graphPane.Legend.IsVisible = false;

			ArrayList groups = statsEngine.GetClusteredDataSet();
			PieItem []segments = new PieItem[groups.Count];
			for (int idx = 0; idx < groups.Count; idx++)
			{
				ArrayList items = groups[idx] as ArrayList;
				string start = string.Format("{0:f2}", items[0]);
				string end = string.Format("{0:f2}", items[items.Count-1]);
				string label =  start + " - " + end;
				int colorseg = 255 - ((255 / groups.Count) * (idx+1));
				segments[idx] = graphPane.AddPieSlice(items.Count, Color.FromArgb(colorseg, colorseg, colorseg), 0.0, label);
				segments[idx].LabelDetail.FontSpec.Border.IsVisible = false;
				segments[idx].LabelType = PieLabelType.Name_Value_Percent;
			}

			SaveGraphImage(graphPane, reportPath + @"\distribution.png");
			#endregion

			#region Write table
			// write table header
			distTable = new StringBuilder();
			distTable.Append("<table><thead><tr>\n");
			distTable.Append("<th width=\"20%\">Group</th>\n");
			distTable.Append("<th width=\"20%\">Range in milliseconds</th>\n");
			distTable.Append("<th width=\"20%\">Total tests</th>\n");
			distTable.Append("<th width=\"20%\">Percent of total</th>\n");
			distTable.Append("</tr></thead><tbody>\n");

			int totalEntries = 0;
			for (int i=0; i<groups.Count; i++)
			{
				ArrayList items = groups[i] as ArrayList;
				totalEntries += items.Count;
			}

			bool oddline = true;
			for (int i=0; i<groups.Count; i++)
			{
				ArrayList items = groups[i] as ArrayList;
				string start = string.Format("{0:f2}", items[0]);
				string end = string.Format("{0:f2}", items[items.Count-1]);
				string label =  start + " - " + end;

				if (oddline == true)
					distTable.Append("<tr class=\"odd\">\n");
				else
					distTable.Append("<tr class=\"even\">\n");

				distTable.Append("<td class=\"s\">");
				distTable.AppendFormat("{0}", i+1);
				distTable.Append("</td>\n");

				distTable.Append("<td class=\"s\">");
				distTable.Append(label);
				distTable.Append("</td>\n");

				distTable.Append("<td class=\"s\">");
				distTable.AppendFormat("{0}", items.Count);
				distTable.Append("</td>\n");
				
				double percent = 100 - (((double)(totalEntries - items.Count) / (double)totalEntries) * 100.0);
				distTable.Append("<td class=\"s\">");
				distTable.AppendFormat("{0:f2}", percent);
				distTable.Append("</td>\n");

				oddline = !oddline;
			}
			distTable.Append("</tr></table>\n");
			#endregion
		}

		private string GenerateTestSummaryText()
		{
			string s;
			TestDataElement t = testDataList[0] as TestDataElement;
			s = "Test \"" + t.taskName + "\" ";
			s += "consists of " + testDataList.Count + " threads each iterating " + t.totalIterations + " times.";

			ArrayList statSummary = statsEngine.GetSummaryData();
			statSummaryTable = new StringBuilder();

			#region Write table
			// write table header
			statSummaryTable = new StringBuilder();
			statSummaryTable.Append("<table><thead><tr>\n");
			statSummaryTable.Append("<th width=\"12%\">Thread</th>\n");
			statSummaryTable.Append("<th width=\"12%\">SuccessRate</th>\n");
			statSummaryTable.Append("<th width=\"12%\">Mean</th>\n");
			statSummaryTable.Append("<th width=\"12%\">Median</th>\n");
			statSummaryTable.Append("<th width=\"12%\">STD</th>\n");
			statSummaryTable.Append("<th width=\"12%\">Mean-Outlier</th>\n");
			statSummaryTable.Append("<th width=\"12%\">Median-Outlier</th>\n");
			statSummaryTable.Append("<th width=\"12%\">STD-Outlier</th>\n");
			statSummaryTable.Append("</tr></thead><tbody>\n");

			bool oddline = true;
			for (int i=0; i<statSummary.Count; i++)
			{
				ExtendedStatsElement item = statSummary[i] as ExtendedStatsElement;

				if (oddline == true)
					statSummaryTable.Append("<tr class=\"odd\">\n");
				else
					statSummaryTable.Append("<tr class=\"even\">\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0}", i+1);
				statSummaryTable.Append("</td>\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0}", item.successrate);
				statSummaryTable.Append("</td>\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0:f2}", item.meanRaw);
				statSummaryTable.Append("</td>\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0:f2}", item.medianRaw);
				statSummaryTable.Append("</td>\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0:f2}", item.stdRaw);
				statSummaryTable.Append("</td>\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0:f2}", item.meanMinusOutliers);
				statSummaryTable.Append("</td>\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0:f2}", item.medianMinusOutliers);
				statSummaryTable.Append("</td>\n");

				statSummaryTable.Append("<td class=\"s\">");
				statSummaryTable.AppendFormat("{0:f2}", item.stdMinusOutliers);
				statSummaryTable.Append("</td>\n");

				oddline = !oddline;
			}
			statSummaryTable.Append("</tr></table>\n");
			#endregion
			return s;
		}

		#region Utilitity functions
		private void DumpRawData()
		{
			string s = "";
			foreach (TestDataElement item in testDataList)
			{
				for (int i=0; i<item.responseTimes.Count; i++)
				{
					IndividualTest d = item.responseTimes[i] as IndividualTest;
					s += d.responseTime.ToString();
					if (i != item.responseTimes.Count-1)
						s += "\t";
				}
				s += "\r\n";
			}

			FileStream file = new FileStream(reportPath + @"\sourcedata.txt", FileMode.Create, FileAccess.Write);
			StreamWriter sw = new StreamWriter(file);
			sw.Write(s);
			sw.Close();
			file.Close();
		}

		private string GetDate()
		{
			string TimeInString = "";
			int hour = DateTime.Now.Hour;
			if (hour > 12) hour -= 12;
			int min = DateTime.Now.Minute;
			int sec = DateTime.Now.Second;
			TimeInString = DateTime.Now.Year.ToString() + 
				DateTime.Now.Month.ToString() + 
				DateTime.Now.Day.ToString();
			TimeInString += "-";
			TimeInString += (hour < 10)? "0" + hour.ToString() : hour.ToString();
			TimeInString += ((min<10)? "0" + min.ToString() : min.ToString());
			TimeInString += ((sec<10)? "0" + sec.ToString() : sec.ToString());
			return TimeInString;
		}
	
		private void SaveGraphImage(GraphPane graphPane, string fileName)
		{
			Graphics g = mainForm.CreateGraphics();
			graphPane.AxisChange(mainForm.CreateGraphics());
			g.Dispose();
			graphPane.Image.Save(fileName, System.Drawing.Imaging.ImageFormat.Png); 
		}
		#endregion
	}
}

