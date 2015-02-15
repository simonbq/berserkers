using UnityEngine;
using System.Collections;

//stolen from Scorched Kappa DansGame
public class GUIShadow {
	
	public static void LayoutLabel(string text)
	{
		LayoutLabel(text, GUI.skin.label);
	}
	
	public static void LayoutLabel(string text, GUIStyle style)
	{
		LayoutLabel(new GUIContent(text), style);
	}
	
	public static void LayoutLabel(string text, params GUILayoutOption[] options)
	{
		LayoutLabel(new GUIContent(text), GUI.skin.label, options);
	}
	
	public static void LayoutLabel(GUIContent content, GUIStyle style)
	{
		LayoutLabel(content, style, new GUILayoutOption[]{});
	}
	
	public static void LayoutLabel(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
	{
		Rect pos = GUILayoutUtility.GetRect(content, style, options);
		
		Label(pos, content, style);
	}
	
	public static void Label(Rect position, string text)
	{
		Label(position, text, GUI.skin.label);
	}
	
	public static void Label(Rect position, string text, GUIStyle style)
	{
		Label(position, new GUIContent(text), style);
	}
	
	public static void Label(Rect position, GUIContent content, GUIStyle style)
	{
		Rect shadow = position;
		shadow.x++;
		shadow.y++;
		
		Color old = GUI.color;
		Color s = Color.black;
		s.a = old.a;
		GUI.color = s;
		GUI.Label(shadow, new GUIContent(RemoveRichText(content.text)), style);
		shadow.x++;
		shadow.y++;
		GUI.Label(shadow, new GUIContent(RemoveRichText(content.text)), style);
		
		GUI.color = old;
		GUI.Label(position, content, style);
	}
	
	public static void LabelGlow(Rect position, string text, GUIStyle style, int glowSize, Color glowColor)
	{
		LabelGlow(position, new GUIContent(text), style, glowSize, glowColor);
	}
	
	public static void LabelGlow(Rect position, GUIContent content, GUIStyle style, int glowSize, Color glowColor)
	{
		Color old = GUI.color;
		content.text = RemoveRichText(content.text);
		
		for (int i = glowSize; i >= 1; i--)
		{
			Color current = glowColor;
			current.a = 1 - ((float)i / (float)glowSize);
			current.a /= (float)glowSize;
			GUI.color = current;
			
			Rect glow = position;
			glow.x += i;
			GUI.Label(glow, content, style);
			
			glow = position;
			glow.x -= i;
			GUI.Label(glow, content, style);
			
			glow = position;
			glow.y += i;
			GUI.Label(glow, content, style);
			
			glow = position;
			glow.y -= i;
			GUI.Label(glow, content, style);
			
			glow = position;
			glow.x += i;
			glow.y += i;
			GUI.Label(glow, content, style);
			
			glow = position;
			glow.x -= i;
			glow.y -= i;
			GUI.Label(glow, content, style);
			
			glow = position;
			glow.x -= i;
			glow.y += i;
			GUI.Label(glow, content, style);
			
			glow = position;
			glow.x += i;
			glow.y -= i;
			GUI.Label(glow, content, style);
		}
		
		GUI.color = old;
		GUI.Label(position, content, style);
	}
	
	
	public static string RemoveRichText(string text)
	{
		string res = text;
		int pos = 0;
		res = res.Replace("</color>", "");
		
		while (res.IndexOf("<color=", pos) != -1)
		{
			int start = res.IndexOf("<color=", pos);
			int end = res.IndexOf('>', start) - start;
			end = Mathf.Clamp(end, 0, text.Length) + 1;
			res = res.Remove(start, end);
			
			pos += 8;
		}
		
		
		return res;
	}
}