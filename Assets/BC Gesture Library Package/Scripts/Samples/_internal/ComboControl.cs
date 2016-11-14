using System;
using UnityEngine;

public class ComboControl
{
	private string[] contentList;
	private GUIStyle listButtonStyle;
	private GUIStyle buttonStyle;
	private ComboBox comboBox = new ComboBox();
	
	
	public ComboControl()
	{
		if ( SampleGUI.Factory() != null) {
			listButtonStyle =  SampleGUI.Factory().comboBoxListButtonStyle;
			buttonStyle = SampleGUI.Factory().comboBoxButtonStyle;
		}
	}
	
	public void SetButtonName(string aName)
	{
		comboBox.SetButtonName(aName);
	}
	
	public void SetFingerLocationList(int startValue, bool includeAtLeastOne)
	{
		int count = 3;
		if (includeAtLeastOne) {
			count = 4;
		}
		contentList  = new string[count];
		contentList[0] = BaseGesture.FingerLocation.Over.ToString();
		contentList[1] = BaseGesture.FingerLocation.Always.ToString();
		contentList[2] = BaseGesture.FingerLocation.NotOver.ToString();
		if (includeAtLeastOne) {
			contentList[3] = BaseGesture.FingerLocation.AtLeastOneOver.ToString();
		}
		
		comboBox.SetSelectedItemIndex(startValue);
		comboBox.SetListText(contentList);
	}
	
	public void SetSwipeDirectionList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( SwipeGesture.SwipeDirection )));
		comboBox.SetSelectedItemIndex(startValue);
	}
//	public void SetMotionDirectionList(int startValue)
//	{
//		populateContent(Enum.GetNames(typeof( MotionGesture.MotionDirection )));
//		comboBox.SetSelectedItemIndex(startValue);
//	}	
	public void SetFingerCountList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( BaseGesture.FingerCountRestriction )));
		comboBox.SetSelectedItemIndex(startValue);
	}
	public void SetPinchActionList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( PinchGesture.PinchAction )));
		comboBox.SetSelectedItemIndex(startValue);
	}
	public void SetPinchDirectionList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( PinchGesture.PinchDirection )));
		comboBox.SetSelectedItemIndex(startValue);
	}
	public void SetRotateAxisList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( RotateGesture.RotateAxis )));
		comboBox.SetSelectedItemIndex(startValue);
	}
	
	public void SetLineSwipeDirectionList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( LineGesture.LineSwipeDirection )));
		comboBox.SetSelectedItemIndex(startValue);
	}
	
	public void SetLineFactoryList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( LineFactory.LineType )));
		comboBox.SetSelectedItemIndex(startValue);
		comboBox.UpdateListText(0, "Clear Types");
	}
	
	public void SetDragPositionList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( DragGesture.DragPosition )));
		comboBox.SetSelectedItemIndex(startValue);
	}
	
	public void SetLineIdentificationList(int startValue)
	{
		populateContent(Enum.GetNames(typeof( LineGesture.LineIdentification )));
		comboBox.SetSelectedItemIndex(startValue);
	}
	
	private void populateContent(string[] names)
	{
		contentList  = new string[names.Length];
		for (int i = 0; i < names.Length; i++) {
			contentList[i] = names[i];
		}
		comboBox.SetListText(contentList);
	}
	
	public int GUIShowList(string label, Rect labelRect, Rect listRect)
	{
		GUI.Label(labelRect, label, SampleGUI.Factory().settingsTextStyle);
		int selectedItemIndex = comboBox.GetSelectedItemIndex();
		selectedItemIndex = comboBox.List(listRect, buttonStyle, listButtonStyle);
		return selectedItemIndex;
	}
	
	public int GetSelectedItemIndex()
	{
		return comboBox.GetSelectedItemIndex();
	}
	public void SetSelectedItemIndex(int index)
	{
		comboBox.SetSelectedItemIndex(index);
	}

}

