using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
static public class LogUtil 
{
	static public readonly string ColorEnd = "</color>";

	private static readonly Color methodLogColor = new Color(0, 0.7f, 0.1f);
	private static readonly Color typeLogColor = new Color(0, 1f, 0.1f);
	private static readonly Color lineLogColor = new Color(0, 0.5f, 0.4f);
	static public string ColorStart(Color color) 
		=>"<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";
	static public string ColorGreen(string str) => Color(str, UnityEngine.Color.green);
	static public string ColorRed(string str) => Color(str, UnityEngine.Color.red);
	static public string Color(string str, Color color)
		=> ColorStart(color) + str + ColorEnd;
	  
    static public string GetStackLog(int skipFrames = 1, int maxFrameCount = 10 , bool useHyperLink = true)
	{  
		var frames = new StackTrace(skipFrames , true).GetFrames();
		string traceStr = "";
#if UNITY_EDITOR
		if(useHyperLink)
		{
            var filePath = frames[0].GetFileName();
            filePath = filePath.Replace( '\\' , '/');
            filePath = filePath.TrimStartUntil("/Assets/");
            var lineNumber = frames[0].GetFileLineNumber();
			var href = $"href=\"{filePath}\""; 
			var lineRef = $"\" line=\"{lineNumber}\"";
			var hyperLink = $"<a {href} {lineRef}> {filePath}:{lineNumber} </a>"; 
            traceStr += hyperLink + "\n";
        }
#endif 
        for (int i = 0; i < frames.Length && i < maxFrameCount; i++) 
			traceStr += '\n' + Frame(frames[i]); 

        return traceStr + '\n';
	} 
	 

    static public string Frame(StackFrame frame , Color typeColor , Color methodColor , Color lineColor)
	{ 
		return Color(frame.GetMethod().DeclaringType.FullName, typeColor)+
				Color('.'+frame.GetMethod().Name + "() : ", methodColor)+ 
				Color("line " + frame.GetFileLineNumber().ToString(), lineColor);
	}
	static public string Frame(StackFrame frame)
	{
		return Frame(frame , typeLogColor , methodLogColor, lineLogColor);
	}

	static public string ToStringFormat(this float f, int maxChars)
	{
		var format = "0.";
		for (int i = 0; i < maxChars; i++)
			format += '0';
		return f.ToString(format);
	}
	 
}
