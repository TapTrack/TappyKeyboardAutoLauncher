using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TapTrack.Tcmp.Communication;
using TapTrack.Tcmp.CommandFamilies.BasicNfc;
using TapTrack.Tcmp.Communication.Exceptions;
using NDesk.Options;

namespace TappyKeyboardAutoLauncher
{

    class Program
    {
        static void Main(string[] args)
        {
            List<RecordType> recordTypesToPrint = new List<RecordType>();
            List<ControlCharacter> controlCharactersToEnter = new List<ControlCharacter>();
            List<String> recordTypeArgs = new List<String>();
            List<String> controlCharArgs = new List<String>();
            bool formModeActive = false;
            bool showHelp = false;
            int scanStaggerMs = 2000;

            var p = new OptionSet() {
    { "rt|RecordType=", "A type of NDEF record to enter as keystrokes. If unspecified only text records are included. Options are 'T' for text, 'U' for URI/URL, 'M' for MIME, 'E' for external",
       v => recordTypeArgs.Add (v) },
    { "cc|ControlChar=",
       "A control character to enter after each record contained in the NDEF message.  Valid options are 'return' and 'tab'",
       v => controlCharArgs.Add(v) },
    { "FormMode", "Flag to indicate form mode, which insters the TAB character after each record except the final one where RETURN is inserted. If absent form mode is disabled.",
       v => { if (v != null) formModeActive = true; }  },
    {"sd|ScanDelay=", "Delay in milliseconds to wait after each tag is scanned before initiating another scan.  This must be an integer.  The default value is 2000ms.",
       (int v) => scanStaggerMs = v},
    { "h|help",  "Show this message and exit",
          v => showHelp = true},
            };

            void AddRecordTypeToPrint(String recordTypeStr)
            {
                if (recordTypeStr != null && RecordTypeMethods.FromString(recordTypeStr) != RecordType.INVALID)
                {
                    recordTypesToPrint.Add(RecordTypeMethods.FromString(recordTypeStr));
                }
                else
                {
                    showHelp = true;
                }
            }

            void AddControlCharactersToEnter(String controlCharStr)
            {
                if (controlCharStr != null && ControlCharacterMethods.FromString(controlCharStr) != ControlCharacter.INVALID)
                {
                    controlCharactersToEnter.Add(ControlCharacterMethods.FromString(controlCharStr));
                }
                else
                {
                    showHelp = true;
                }
            }

            if (args.Length == 0)
            {
                recordTypesToPrint.Add(RecordType.TEXT);
                controlCharactersToEnter.Add(ControlCharacter.CRLF);
            }
            else
            {
                List<string> extra;
                try
                {
                    extra = p.Parse(args);

                    foreach (String s in recordTypeArgs)
                    {
                        AddRecordTypeToPrint(s);
                    }

                    foreach (String s in controlCharArgs)
                    {
                        AddControlCharactersToEnter(s);
                    }

                    if (showHelp == true)
                    {
                        Console.WriteLine("Usage: TappyKeyboardAutoLauncher [OPTIONS]");
                        Console.WriteLine("A simple command line utility designed to automatically detect a Tappy reader and engadge keyboard entry mode");
                        Console.WriteLine();
                        Console.WriteLine("***If no options are provided*** this utility will accept text records as keystrokes with the {ENTER} key at the end of each record");
                        Console.WriteLine();
                        Console.WriteLine("**Hint:***");
                        Console.WriteLine("Put this command into a a simple batch (.bat) file and place the file in the Startup folder to have Windows automatically start accepting NFC tag data as keyboard entry upon bootup");
                        Console.WriteLine();
                        Console.WriteLine("Options:");
                        p.WriteOptionDescriptions(Console.Out);
                        return;
                    }
                }
                catch (OptionException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Try -help' for more information.");
                    return;
                }
            }




            TappyUSBKeyboardWedge tappyKeyboadWedge = new TappyUSBKeyboardWedge(recordTypesToPrint, controlCharactersToEnter, scanStaggerMs, formModeActive);
            //  ManualResetEvent _pauseEvent = new ManualResetEvent(true);
            DetectSingleNdef read = new DetectSingleNdef(0, DetectTagSetting.Type2Type4AandMifare);
            if (tappyKeyboadWedge.tappy.AutoDetect())
            {
                Console.WriteLine("Connected to TappyUSB on {0}", tappyKeyboadWedge.tappy.DeviceName);
                tappyKeyboadWedge.tappy.SendCommand(read, tappyKeyboadWedge.KeyboardWedge);
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
                //while (true)
                //{
                //	_pauseEvent.WaitOne(Timeout.Infinite);
                //}
            }
            else
            {
                Console.WriteLine("No TappyUSB found");

            }
        }
    }

}
