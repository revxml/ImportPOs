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

#region --- Main MACOLA Window load and navigation to our destination

	_WinWaitActivate(" Progression Workflow Explorer","Progress Bar")
	;Set the root Macola window active
	Local $hndlParentWindow = WinActive("")
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
#EndRegion

#region --- File setup (get source file, setup log file) ---
	Local $itemNo = ""; this will hold the value we need to insert/update
	Sleep(200)
	Local $sPath = "C:\NewItemFiles\"	;local path to the files
	Local $sDate = StringReplace(_NowCalcDate(),"/","") ;get the current date in YYYYMMDD format
	Local $fileName =  $sDate & "_NewItemList.txt" ;the file to upload should be prefixed with the current date
	Local $iLineCount = _FileCountLines($sPath & $fileName) ;find out how many items we need to process
	Local $log = FileOpen($sPath  & $sDate & "_NewItem.log",1) ;create and open a log file for today's processing
	if $log = -1 Then
		MsgBox($MB_SYSTEMMODAL,"Error","Error Creating/Opening Log File: " & $sPath & $sDate & "_NewItem.log")
		Exit
	EndIf

	Local $file = FileOpen($sPath &  $fileName,0) ;open the source file for processing
	;if the file wouldn't open, or doesn't exists, write to the log, show a message, and exit the process
	If $file = -1 Then
		FileWriteLine($log,@YEAR&@MON&@MDAY&" "&@HOUR&":"&@MIN&":"&@SEC & "	Error reading file:" & $fileName)
		MsgBox($MB_SYSTEMMODAL,"","An error occurrd when reading the file. ")
		Exit
	EndIf
	;log the time and file name we are going to process
	FileWriteLine($log,@YEAR&@MON&@MDAY&" "&@HOUR&":"&@MIN&":"&@SEC & "	File opened for reading:" & $fileName)
#EndRegion

#region --- File Processing by line ---
	Local $itemCnt = 1 ;internal counter for itterrating through the source file

	;While there are lines to read from the source file, call the processing function
	While True
		$itemNo =  FileReadLine($file,$itemCnt)
		ProcessNewItem($itemNo);
	WEnd
#EndRegion

#region --- The Processing function ---
	;This function performs all the work for adding the new items
	Func ProcessNewItem($itemNo)
		#region -- Entering the Item number ---
		Sleep(200)
		;Get a handle on the "Starting Parent Item No. input Text Box
		Local $hndlTextBox = ControlGetHandle($hndWindow,"","Edit1")
		;Set the Text value of the TextBox to the ItemNumber being processed
		ControlSetText("","",$hndlTextBox,$itemNo)
		Sleep(100)
		;Move to the "Ending Parent Item No." Text box
		Send("{TAB}")
		Sleep(200)
		;Get a handle on the "Ending Parent Item No. Input Text Box
		Local $hndTextConfirm = ControlGetHandle($hndWindow,"","Edit2")
		;Set the Text Value of the TextBox to the Item Number being processed
		ControlSetText("","",$hndTextConfirm,$itemNo)
		;pause a nanosecond
		Sleep(250)
		#EndRegion --- Entering the Item Number ---

		#region -- First Item processed for the run ---
		;from testing - the three checkboxes only need to be set (checked) on the first item
		;on subsequent items, these retain their checked status
		If $itemCnt = 1 Then
			#region --- Three Checkboxes that must be selected ---
			;Get a handle on the "Reflect The Shrink/Scrap Factor in Costs?" control
			Local $hndlReflectCB = ControlGetHandle($hndWindow,"","[CLASS:Button; INSTANCE:35]");,"Reflect The Shrink/Scrap Factor In Costs?"
			;Click the "Reflect THe Shrink/Scrap Factor in Costs?" checkbox
			ControlClick($hndWindow,"Reflect The Shrink/Scrap Factor In Costs?",$hndlReflectCB)
			;pause briefly
			Sleep(200)
			;Get a handle on the "Blow Through Phantom Items?" control
			Local $hBLowCB = ControlGetHandle($hndWindow,"","[CLASS:Button; INSTANCE:36]")
			;Click the "Blow Through Phantom Items?" checkbox
			ControlClick($hndWindow,"Blow Through Phantom Items?",$hBLowCB)
			;pause briefly
			Sleep(200)
			;MouseClick("left",287,317,1)
			;Get a handle on the "Update Parent Cost In Inventory?" control
			Local $hUpdateParent = ControlGetHandle($hndWindow,"","[CLASS:Button; INSTANCE:38]")
			;Click the "Update Parent Cost In Invetory?" checkbox
			ControlClick($hndWindow,"Update Parent Cost In Inventory?",$hUpdateParent)
			;pause briefly
			Sleep(200)
			#EndRegion -- THree Checkboxes that must be selected --
		EndIf
		#endregion -- First Item Processed for the run ---

		#region -- Submitting the Item for Processing ---
		;Get a handle on the "OK" button
		Local $hOK = ControlGetHandle($hndWindow,"","[CLASS:Button; INSTANCE:18]")
		;Click the "OK" button
		ControlClick($hndWindow,"OK",$hOK)
		Sleep(200)
		#EndRegion -- Submitting the Item for Processing ---

		#region -- Notification dialogs that appear ---
		;Now we get a big WARNING window, so we wait for it
		_WinWaitActivate("Updating Costs","Method Is Either *AV")
		Sleep(200)
		;Get a handle on the WARNING Window
		Local $hWarningWindow = WinActive("")
		;The "Cancel" buttin has focuse when the dialog loads/displays
		;send a Shift+Tab to move to the "OK" button
		Send("+{TAB}")
		;send "Enter" to click "OK"
		Send("{SPACE}")

		#region --- Print Options - "File" must be selected here ---
		;now we get the "Print Options" dialog window
		_WinWaitActivate("Print Options","&Change Defaults...")
		Sleep(200)
		;Get a handle on the Print Options window
		Local $hPrintOptionsWin = WinACtive("")
		;Get a handle on the "File" radio button control
		Local $hFileRB = ControlGetHandle($hPrintOptionsWin,"","[CLASS:Button; INSTANCE:7]")
		;Click the "File" radio button control
		ControlClick($hPrintOptionsWin,"",$hFileRB)
		;This radio wasn't accepting the checked state, only the focused state
		;so click it a second time
		ControlClick($hPrintOptionsWin,"",$hFileRB)
		;Pause briefly
		Sleep(200)
		;Get a handle on the Print Options OK button control
		Local $hPrintOptionsOK = ControlGetHandle($hPrintOptionsWin,"","[CLASS:Button; INSTANCE:1]")
		;Click the Print Options OK button control
		ControlClick($hPrintOptionsWin,"",$hPrintOptionsOK)
		#EndRegion -- Print Options - "File" must be selected here ---
		#region --- Print Status - this appears and remains in focus for unknown periods ---
		;The Print Status window takes about a second to pop up, so I need to wait for it
		Sleep(1000)
		;hopefully it is visible and has focus
		Local $hPrintingWin = WinActive("")
		;we need to wait for it to complete before we proceed to the next item
		WinWaitNotActive($hPrintingWin)
		Sleep(1000)
		#EndRegion --- Print Status ---

		#region --- Log results of each item processing ---
		;if there has been an error, log it
		If @error Then
			FileWriteLine($log,@YEAR&@MON&@MDAY&" "&@HOUR&":"&@MIN&":"&@SEC & "	Error adding item: " & $itemNo)
		Else ;otherwise, log the item as processed
			FileWriteLine($log,@YEAR&@MON&@MDAY&" "&@HOUR&":"&@MIN&":"&@SEC & "	Item added: " & $itemNo)
		EndIf
		#region --- Log results of each item processing ---

		#region -- Item Count and EOF ---
		;increment our internal counter to the next line number in the source file
		$itemCnt = $itemCnt + 1
		;if the internal counter value is now greater than the number of lines in the source file,
		;log the end of file and move the source file to the archive directory
		;then exit the app/script
		if $itemCnt > $iLineCount Then
			FileWriteLine($log,@YEAR&@MON&@MDAY&" "&@HOUR&":"&@MIN&":"&@SEC & "	End of file.")
			FileMove($sPath & $fileName,$sPath & "\Archive\" & @YEAR & @MON & "\" ,8)
			Exit
		EndIf
		#EndRegion
	EndFunc
	#EndRegion -- File Processing Function

	#endregion --- Au3Recorder generated code End ---

	#region -- Utilities ---
	;this is a hotkey assigned function.
	;clicking the ESC key while the script is running
	;will terminate the script
	Func Terminate()
		Exit
	EndFunc   ;==>Terminate
	#EndRegion -- Utilities ---

