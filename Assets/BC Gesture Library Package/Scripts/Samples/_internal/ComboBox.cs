using UnityEngine;

public class ComboBox
{
	private static bool forceToUnShow = false;
	private bool isClickedComboButton = false;
	private int selectedItemIndex = 0;
	private string[] texts;
	private string loneButtonName;
	
	protected const float boxHeight = 30f;
	protected const float buttonHeight = 26f;
	protected const float buttonIndent = 13f;
	protected const float buttonTopHeight = 17f;
	protected const float buttonBottomHeight = 32f;
	protected const float topBottomCushion = 50f;
	
	private Rect[] popoutTopRect;
	private Rect[] popoutRect;
	private Rect[] popoutBottonRect;
	private Rect popoutArrowRect;
	private bool initizalize = true;
	private Rect[] buttonRects;
	private int columns;
	private int columnRows;
	
	private static ComboBox currentlyOpenComboBox = null;
	
	public void SetButtonName(string aName)
	{
		loneButtonName = aName;
	}
	
	public int List (Rect rect, GUIStyle buttonStyle, GUIStyle listButtonStyle)
	{
		if (forceToUnShow) {
			forceToUnShow = false;
			isClickedComboButton = false;
		}
		
		
		bool done = false;
		if (texts.Length == 0) {
			isClickedComboButton = false;
			return GetSelectedItemIndex ();
		}
		string buttonName = loneButtonName;
		if (buttonName == null) {
			buttonName = texts[selectedItemIndex];
		}
		if (GUI.Button (rect, buttonName, buttonStyle)) {
			isClickedComboButton = !isClickedComboButton;
			//Debug.Log("ComboBox:List button pressed " + isClickedComboButton);
		}
		
		if (isClickedComboButton) {
			if (currentlyOpenComboBox != null && currentlyOpenComboBox != this) {
				//Debug.Log("ComboBox:List other combo open, close it ");
				currentlyOpenComboBox.CloseNow();
			}
			currentlyOpenComboBox = this;
		
			if (initizalize) {
				float width = rect.width + 21f;
				columns = 1;
				columnRows = texts.Length;
				float buttonTopBottomHeight = buttonTopHeight + buttonBottomHeight;
				float repeaterHeight = (texts.Length * boxHeight) - buttonTopHeight - buttonTopHeight + 7;
				float defaultStartingY = rect.y - buttonTopHeight;
				float startingY = defaultStartingY;
				float totalHeight = repeaterHeight + buttonTopBottomHeight;
				
				if ((startingY + totalHeight + topBottomCushion) > Screen.height) {
					startingY = Screen.height - totalHeight - topBottomCushion;
					if (startingY < topBottomCushion) {
						while (startingY < topBottomCushion) {
							columns++;
							columnRows = texts.Length / columns;
							if (texts.Length > (columnRows * columns)) {
								columnRows += texts.Length - (columnRows * columns);
							}
							repeaterHeight = (columnRows * boxHeight) - buttonTopHeight - buttonTopHeight + 7;
							totalHeight = repeaterHeight + buttonTopBottomHeight;
							startingY = Screen.height - totalHeight - topBottomCushion;
							if (startingY > defaultStartingY) {
								startingY = defaultStartingY;
							}
						}
					}
					
				}
				//Debug.Log("ComboBox:List isClickedComboButton columns=" + columns + ", columnRows=" + columnRows + " from " + texts.Length);
				
				popoutArrowRect = new Rect(rect.xMax - 5f, rect.y - 15f, 28f, 55f);
				popoutTopRect = new Rect[columns];
				popoutRect = new Rect[columns];
				popoutBottonRect = new Rect[columns];
				float popoutStarty = startingY - 10f;
				float xPos = popoutArrowRect.xMax  - 7f;
				for (int column = 0; column < columns; column++) {
					popoutTopRect[column] = new Rect(xPos, popoutStarty, width, buttonTopHeight);
					popoutRect[column] = new Rect(xPos, popoutTopRect[column].yMax, width, repeaterHeight);
					popoutBottonRect[column] = new Rect(xPos, popoutRect[column].yMax, width, buttonBottomHeight);
					xPos += width - 15f;
				}
				
				buttonRects = new Rect[texts.Length];
				float buttonY;
				float buttonSpace = (boxHeight - buttonHeight) / 2;
				float buttonX;
				float buttonWidth = width - (buttonIndent * 2f);
				int index = 0;
				for (int column = 0; column < columns; column++) {
					buttonY = popoutStarty + 5;
					buttonX = popoutRect[column].x + buttonIndent;
					//Debug.Log("ComboBox:List isClickedComboButton start loop buttonY=" + buttonY + ", buttonX=" + buttonX);
					for (int row = 0; row < columnRows && index < texts.Length; row++) {
						buttonRects[index] = new Rect(buttonX, buttonY + buttonSpace, buttonWidth, buttonHeight);
						buttonY += boxHeight;
						index++;
					}
				}
				
				initizalize = false;
			}
			//Debug.Log("ComboBox:List isClickedComboButton  DrawTexture");
			
			for (int column = 0; column < columns; column++) {
				GUI.DrawTexture(popoutTopRect[column], SampleGUI.Factory().comboBoxListTop);
				GUI.DrawTexture(popoutBottonRect[column], SampleGUI.Factory().comboBoxListBottom);
				GUI.DrawTexture(popoutRect[column], SampleGUI.Factory().comboBoxListRepeater);
			}
			GUI.DrawTexture(popoutArrowRect, SampleGUI.Factory().comboBoxListPointer);
			
			for (int i = 0; i < texts.Length; i++) {
				if (GUI.Button(buttonRects[i], texts[i], listButtonStyle)) {
					isClickedComboButton = false;
					selectedItemIndex = i;
				}
			}
			
//			//Rect listRect = new Rect (rect.x, rect.y + listStyle.CalcHeight (listContent[0], 1.0f) + 8f, rect.width, (listStyle.CalcHeight(listContent[0], 1.0f) + 5f)* listContent.Length);
//			Rect listRect = new Rect (rect.xMax, rect.y, rect.width, (listStyle.CalcHeight(listContent[0], 1.0f) + 6f)* listContent.Length);
//			
//			GUI.Box (listRect, "", boxStyle);
//			int newSelectedItemIndex = GUI.SelectionGrid (listRect, selectedItemIndex, listContent, 1, listStyle);
//			//Debug.Log("ComboBox:List SelectionGrid returned " + newSelectedItemIndex);
//			if (newSelectedItemIndex != selectedItemIndex) {
//				selectedItemIndex = newSelectedItemIndex;
//				isClickedComboButton = false;
//			}
		}
		
		
		
		if (done) {
			isClickedComboButton = false;
		}
		
		return GetSelectedItemIndex ();
	}
	
	protected void CloseNow()
	{
		isClickedComboButton = false;
	}
	
	public void SetListText(string[] listText)
	{
		texts = listText;					
	}
	
	public void UpdateListText(int i, string text)
	{
		texts[i] = text;
	}
	
	public void SetSelectedItemIndex (int index)
	{
		selectedItemIndex = index;
	}

	public int GetSelectedItemIndex ()
	{
		return selectedItemIndex;
	}
}
