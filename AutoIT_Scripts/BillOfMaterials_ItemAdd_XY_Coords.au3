#include <FileConstants.au3>
#include <file.au3>
#include <MsgBoxConstants.au3>
#include<Date.au3>
#include <GuiTreeView.au3>



Opt("WinTitleMatchMode",2);
Opt("WinTextMatchMode", 2) ;1=complete, 2=quick
Opt("WinWaitDelay", 250) ;250 milliseconds
Opt("WinSearchChildren", 1) ;0=no, 1=search children also
Opt("MouseClickDelay", 10) ;10 milliseconds
HotKeySet("{ESC}","Terminate");
;WinActivate("[TITLE:Progression Workflow Explorer;CLASS:ThunderRT6FromDC]");
;Local $hndlWin = WinWAit("[TITLE:Progression Workflow Explorer;CLASS:ThunderRT6FromDC]");
;Local $hndlTree = ControlGetHandle($hndlWin,"","[CLASS:TreeView20WndClass; INSTANCE:1]");
;ControlTreeView($hndlTree,"","","Expand","Root");
;ControlTreeView($hndlTree,"","","Expand","Bill Of Materials");
;ControlTreeView($hndlTree,"","","Expand","Processes");
;ControlTreeView($hndlTree,"","","DoubleClick","Print Costed Bill...");


#region --- Au3Recorder generated code Start (v3.3.9.5 KeyboardLayout=00000409)  ---

#region --- Internal functions Au3Recorder Start ---
Func _Au3RecordSetup()
Opt('WinWaitDelay',100)
Opt('WinDetectHiddenText',1)
Opt('MouseCoordMode',0)
Local $aResult = DllCall('User32.dll', 'int', 'GetKeyboardLayoutNameW', 'wstr', '')
If $aResult[1] <> '00000409' Then
  MsgBox(64, 'Warning', 'Recording has been done under a different Keyboard layout' & @CRLF & '(00000409->' & $aResult[1] & ')')
EndIf

EndFunc

Func _WinWaitActivate($title,$text,$timeout=0)
	WinWait($title,$text,$timeout)
	If Not WinActive($title,$text) Then WinActivate($title,$text)
	WinWaitActive($title,$text,$timeout)
EndFunc

_AU3RecordSetup()
#endregion --- Internal functions Au3Recorder End ---

	_WinWaitActivate(" Progression Workflow Explorer","Progress Bar")
	Local $hndlParentWindow = WinActive("")
	;this all works under specific login and window sizes
	;need to grap control handles instead
	;MouseMove(19,148)
	;MouseDown("left")
	;MouseMove(19,147)
	;MouseUp("left")
	;MouseMove(37,210)
	;MouseDown("left")
	;MouseMove(38,210)
	;MouseUp("left")
	;MouseClick("left",111,273,2)

	;Get Handle on Parent (first) Macola Window
	Local $hndlMenuDD = ControlGetHandle($hndlParentWindow,"","[CLASS:ThunderRT6ComboBox; INSTANCE:2]")
	;Select "Bill Of Materials" from DropDown
	ControlCommand($hndlParentWindow,"","[CLASS:ThunderRT6ComboBox; INSTANCE:2]","SelectString","Bill Of Materials")
	;Tab to the Tree View Menu
	Send("{TAB 4}")
	;Move down the Tree View Menu to the "Processes" node
	Send("{DOWN 4}")
	;Activate the "Processes" Tree View node
	Send("{ENTER}")
	;Move down the Tree View "Processes" node to the "Print Costed Bill.." node
	Send("{DOWN 4}")
	;Activate the "Print Costed Bill..." Tree View node
	Send("{ENTER}")
	;Wait for the new dialog window to open
	Sleep(5000);
	;Get a handle on the new window, which is where all the activity will occur
	Local $hndWindow = WinGetHandle("")
	;pause a millisecond
	Sleep(200)

	Local $itemNo = ""; this will hold the value we need to insert/update
	Sleep(200)
	Local $sPath = "C:\NewItemFiles\"	;local path to the files
	Local $sDate = StringReplace(_NowCalcDate(),"/","") ;get the current date in YYYYMMDD format
	Local $fileName =  $sDate & "_NewItemList.txt" ;the file to upload should be prefixed with the current date
	Local $iLineCount = _FileCountLines($sPath & $fileName) ;find out how many items we need to process
	Local $log = FileOpen($sPath  & $sDate & "_NewItem.log",1) ;create and open a log file for today's processing
	Local $file = FileOpen($sPath &  $fileName,0) ;open the source file for processing
	;if the file wouldn't open, or doesn't exists, write to the log, show a message, and exit the process
	If $file = -1 Then
		FileWriteLine($log,_Now & "	Error reading file:" & $fileName)
		MsgBox($MB_SYSTEMMODAL,"","An error occurrd when reading the file. ")
		Exit
	EndIf
	;log the time and file name we are going to process
	FileWriteLine($log,_Now & "	File opened for reading:" & $fileName)

	Local $itemCnt = 1 ;internal counter for itterrating through the source file

	;While there are lines to read from the source file, call the processing function
	While True
		$itemNo =  FileReadLine($file,$itemCnt)
		ProcessNewItem($itemNo);
	WEnd

	;This function performs all the work for adding the new items
	Func ProcessNewItem($itemNo)
		;MouseClick("left",339,132,1)
		Sleep(200)

		Local $hndlTextBox = ControlGetHandle($hndWindow,"","Edit1")
		ControlSetText("","",$hndlTextBox,$itemNo)
		Sleep(100)
		Send("{TAB}")
		Sleep(200)
		Local $hndTextConfirm = ControlGetHandle($hndWindow,"","Edit2")
		ControlSetText("","",$hndTextConfirm,$itemNo)
		Sleep(250)
		;Terminate();

		If $itemCnt = 1 Then
			;attempt finding control
			Local $hndlReflectCB = ControlGetHandle($hndWindow,"","[CLASS:Button; INSTANCE:35]");,"Reflect The Shrink/Scrap Factor In Costs?"
			ControlClick($hndWindow,"Reflect The Shrink/Scrap Factor In Costs?",$hndlReflectCB)
			Exit

			Sleep(200)
			MouseMove(289,296)
			MouseUp("left")
			Sleep(200)
			MouseClick("left",287,317,1)
			Sleep(200)
			MouseClick("left",281,363,1)
			Sleep(200)
		EndIf
		MouseClick("left",37,71,1)
		Sleep(200)
		_WinWaitActivate("Updating Costs","Method Is Either *AV")
		Sleep(200)
		MouseMove(39,27)
		MouseDown("left")
		Sleep(200)
		MouseMove(39,28)
		MouseUp("left")
		Sleep(200)
		_WinWaitActivate("Print Options","&Change Defaults...")
		Sleep(200)
		MouseClick("left",246,135,1)
		Sleep(200)
		MouseClick("left",50,38,1)
		Sleep(10000)
		;WinWaitActive($hndWindow,"")
		;While WinActive("Print Status","")
		;	Sleep(250)
		;Wend

		If @error Then
			FileWriteLine($log,_Now & "	Error adding item: " & $itemNo)
		Else
			FileWriteLine($log,_Now & "	Item added: " & $itemNo)
		EndIf

		$itemCnt = $itemCnt + 1

		if $itemCnt > $iLineCount Then
			FileWriteLine($log,_Now & "	End of file.")
			;FileClose($file)
			FileMove($file,$sPath & "\Archive\" & $fileName)
			Exit
		EndIf
	EndFunc
	#endregion --- Au3Recorder generated code End ---

	Func Terminate()
		Exit
	EndFunc   ;==>Terminate


