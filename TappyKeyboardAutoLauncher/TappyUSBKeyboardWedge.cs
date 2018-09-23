using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TapTrack.Tcmp.Communication;
using TapTrack.Tcmp.CommandFamilies.BasicNfc;
using TapTrack.Tcmp.Communication.Exceptions;
using NdefLibrary.Ndef;

namespace TappyKeyboardAutoLauncher
{
    class TappyUSBKeyboardWedge
    {
        public TappyReader tappy;
        Callback InvokeKeyboardFeature;
        int scanStaggerMs = 2000;
        bool formModeActive = false;
        List<RecordType> recordTypes = new List<RecordType>();
        List<ControlCharacter> controlCharacters = new List<ControlCharacter>();

        public TappyUSBKeyboardWedge()
        {
            tappy = new TappyReader(CommunicationProtocol.Usb);
            InvokeKeyboardFeature = KeyboardWedge;
            recordTypes.Add(RecordType.TEXT);

        }

        public TappyUSBKeyboardWedge(List<RecordType> recordTypes, List<ControlCharacter> controlCharacters, int scanStaggerMs, bool formModeActive)
        {
            tappy = new TappyReader(CommunicationProtocol.Usb);
            InvokeKeyboardFeature = KeyboardWedge;
            this.recordTypes = recordTypes;
            this.controlCharacters = controlCharacters;
            this.scanStaggerMs = scanStaggerMs;
            this.formModeActive = formModeActive;
        }

        public bool CheckForErrorsOrTimeout(ResponseFrame frame, Exception e)
        {
            if (e != null)
            {
                if (e.GetType() == typeof(HardwareException))
                    Console.WriteLine("Tappy is not connected");
                else
                    Console.WriteLine("An error occured");

                return true;
            }
            else if (!TcmpFrame.IsValidFrame(frame))
            {
                Console.WriteLine("Invalid TCMP frame");

                return true;
            }
            else if (frame.IsApplicationErrorFrame())
            {
                ApplicationErrorFrame errorFrame = (ApplicationErrorFrame)frame;
                Console.WriteLine(errorFrame.ErrorString);
                return true;
            }
            else if (frame.CommandFamily0 == 0 && frame.CommandFamily1 == 0 && frame.ResponseCode < 0x05)
            {
                Console.WriteLine(TappyError.LookUp(frame.CommandFamily, frame.ResponseCode));
                return true;
            }
            else if (frame.ResponseCode == 0x03)
            {
                Console.WriteLine("No tag detected");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void KeyboardWedge(ResponseFrame frame, Exception e)
        {
            DetectSingleNdef repeatRead = new DetectSingleNdef(0, DetectTagSetting.Type2Type4AandMifare);
            bool textEntered = false;

            if (CheckForErrorsOrTimeout(frame, e))
            {
                tappy.SendCommand(repeatRead, InvokeKeyboardFeature);
                return;
            }
            else
            {
                try
                {
                    byte[] data = frame.Data;
                    byte[] buf = new byte[data.Length - data[1] - 2];

                    if (buf.Length > 0)
                    {
                        Array.Copy(data, 2 + data[1], buf, 0, buf.Length);

                        NdefMessage message = NdefMessage.FromByteArray(buf);

                        int numRecords = message.Count;
                        int recordNum = 1;

                        foreach (NdefRecord record in message)
                        {
                            string type = Encoding.UTF8.GetString(record.Type);
                            textEntered = false;
                            if (record.TypeNameFormat == NdefRecord.TypeNameFormatType.NfcRtd && type.Equals("T") && recordTypes.Contains(RecordType.TEXT))
                            {
                                NdefTextRecord textRecord = new NdefTextRecord(record);
                                System.Windows.Forms.SendKeys.SendWait(textRecord.Text);
                                textEntered = true;
                            }
                            else if ((record.TypeNameFormat == NdefRecord.TypeNameFormatType.NfcRtd && type.Equals("U") && recordTypes.Contains(RecordType.URI)) ||
                                     (record.TypeNameFormat == NdefRecord.TypeNameFormatType.Uri && recordTypes.Contains(RecordType.URI)))
                            {
                                NdefUriRecord uriRecord = new NdefUriRecord(record);
                                System.Windows.Forms.SendKeys.SendWait(uriRecord.Uri);
                                textEntered = true;
                            }
                            else if ((record.TypeNameFormat == NdefRecord.TypeNameFormatType.ExternalRtd && recordTypes.Contains(RecordType.EXTERNAL)) ||
                                     (record.TypeNameFormat == NdefRecord.TypeNameFormatType.Mime && recordTypes.Contains(RecordType.MIME)))
                            {
                                System.Windows.Forms.SendKeys.SendWait(Encoding.UTF8.GetString(record.Payload));
                                textEntered = true;
                            }

                            if (textEntered == true && formModeActive == true)
                            {
                                if (recordNum == numRecords)
                                {
                                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                                }
                                else
                                {
                                    System.Windows.Forms.SendKeys.SendWait("{TAB}"); 
                                }
                            }
                            else if(textEntered == true)
                            {
                                if (controlCharacters.Contains(ControlCharacter.CRLF))
                                {
                                    System.Windows.Forms.SendKeys.SendWait("{ENTER}");
                                }
                                if (controlCharacters.Contains(ControlCharacter.TAB))
                                {
                                    System.Windows.Forms.SendKeys.SendWait("{TAB}");
                                }
                            }
                            
                            recordNum++;
                        }

                        if (textEntered)
                        {
                            Thread.Sleep(scanStaggerMs);
                        }
                    }
                    tappy.SendCommand(repeatRead, InvokeKeyboardFeature);
                    return;
                }
                catch
                {
                    Console.WriteLine("Error Parsing NDEF Response From Tappy");
                }
               
            }
        }


    }
}
