# Tappy Automatic NFC Keyboard Wedge Utility

This is a command line utility designed to allow users to easily use a Tappy reader as an NFC keyboard entry device while also providing a simple way to have Windows automatically configure itself for NFC keyboard entry upon bootup. By providing a configurable utility using a command line utility (CLI), TapTrack has allowed a simple batch file to be written and invoked upon bootup by placing the file in the Windows "Startup" folder.  If the TappyUSB is connected to the PC at bootup it will automatically connect to it and initiate scanning for NFC tags while entering the data as keystrokes without any user intervention needed. 

## Supported Readers

As of v1.0 only the TappyUSB reader is supported.  If you're looking for a BLE NFC reader solution for Windows keyboard entry, have a look at the [TappyBLEKeyboardWedge](https://github.com/TapTrack/TappyBLEKeyboardWedge) utility.

In its original release this utility will automatically detect what COM port the TappyUSB is connected to and select the first TappyUSB discovered.  This may be undesirable if one wants multiple TappyUSB readers connected to the PC and wishes to specify the port in this utility.  A future release will include this as a configuration option. 

## Command Line Usage

      --rt, --RecordType=VALUE
                             A type of NDEF record to enter as keystrokes. If
                               unspecified only text records are included.
                               Options are 'T' for text, 'U' for URI/URL, 'M'
                               for MIME, 'E' for external
      --cc, --ControlChar=VALUE
                             A control character to enter after each record
                               contained in the NDEF message.  Valid options
                               are 'return' and 'tab'
      --FormMode             Flag to indicate form mode, which insters the
                               TAB character after each record except the final
                               one where RETURN is inserted. If absent form
                               mode is disabled.
      --sd, --ScanDelay=VALUE
                             Delay in milliseconds to wait after each tag is
                               scanned before initiating another scan.  This
                               must be an integer.  The default value is 2000ms.
      -h, --help             Show this message and exit

## Example

To have NDEF text records inputted as keystrokes followed by the ENTER key:

`TappyKeyboardAutoLauncher -RecordType T -ControlChar return`

To have both NDEF text an URI/URL records inputted as keystrokes followed by the ENTER key:

`TappyKeyboardAutoLauncher -RecordType T -RecordType U -ControlChar return`

To have NDEF MIME records inputted as keystrokes with a delay of half a second between scans:

`TappyKeyboardAutoLauncher -RecordType M -ScanDelay 500`


