using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SocketLibrary
{
    public class MessageSocket
    {
        private const string MARKUP_SENDER = @"/$\user/$\";
        private const string MARKUP_TIMESEND = @"/$\timeSend/$\";
        private const string MARKUP_CONTENT = @"/$\content/$\";
        private const string MARKUP_END = @"/$\ENDMS/$\";

        private static readonly byte[] MARKUP_END_BYTE = Encoding.UTF8.GetBytes(MARKUP_END);
        private static readonly byte[] MARKUP_SENDER_BYTE = Encoding.UTF8.GetBytes(MARKUP_SENDER);

        public string Sender { get; private set; } = null;
        public DateTime TimeSend { get; private set; } = new DateTime();
        public string Content { get; private set; } = null;

        public MessageSocket() { }

        public MessageSocket(string userSender, DateTime timeSend, string content)
        {
            this.Sender = userSender;
            this.TimeSend = timeSend;
            this.Content = content;
        }

        public byte[] ToByte()
        {
            string compressedString = MARKUP_SENDER + Sender + MARKUP_TIMESEND + TimeSend.ToString("g", new CultureInfo("en-US")) + MARKUP_CONTENT + Content + MARKUP_END;
            return Encoding.UTF8.GetBytes(compressedString);
        }
        /*
        public void ToMessageSocket(byte[] buffer, int end = 0)
        {
            if (end == 0)
                end = buffer.Length;

            string compressedString = Encoding.UTF8.GetString(buffer, 0, end);
            ToMessageSocket(compressedString);
        }

        public void ToMessageSocket(string compressedString)
        {
            if (!IsMessageSocket(compressedString)) return;
            int indexUser = compressedString.IndexOf(MARKUP_SENDER);
            int indexTimeSend = compressedString.IndexOf(MARKUP_TIMESEND);
            int indexContent = compressedString.IndexOf(MARKUP_CONTENT);
            int indexEnd = compressedString.IndexOf(MARKUP_END);

            this.Sender = compressedString.Substring(indexUser + MARKUP_SENDER.Length, indexTimeSend - (indexUser + MARKUP_SENDER.Length));
            this.TimeSend = DateTime.ParseExact(compressedString.Substring(indexTimeSend + MARKUP_TIMESEND.Length, indexContent - (indexTimeSend + MARKUP_TIMESEND.Length)), "g", new CultureInfo("en-US"));
            this.Content = compressedString.Substring(indexContent + MARKUP_CONTENT.Length, indexEnd - (indexContent + MARKUP_CONTENT.Length));
        }*/
       
        public static MessageSocket TryCreate(in byte[] buffer, int end = 0)
        {
            if (end == 0)
                end = buffer.Length;
            try
            {
                string compressedString = Encoding.UTF8.GetString(buffer, 0, end);
                return MessageSocket.TryCreate(compressedString);
            }
            catch
            {
                return new MessageSocket();
            }
        }

        public static MessageSocket TryCreate(string compressedString)
        {
            if (!IsMessageSocket(compressedString))
                return new MessageSocket();

            int indexUser = compressedString.IndexOf(MARKUP_SENDER);
            int indexTimeSend = compressedString.IndexOf(MARKUP_TIMESEND);
            int indexContent = compressedString.IndexOf(MARKUP_CONTENT);
            int indexEnd = compressedString.IndexOf(MARKUP_END);

            string userSender = compressedString.Substring(indexUser + MARKUP_SENDER.Length, indexTimeSend - (indexUser + MARKUP_SENDER.Length));
            DateTime timeSend = DateTime.ParseExact(compressedString.Substring(indexTimeSend + MARKUP_TIMESEND.Length, indexContent - (indexTimeSend + MARKUP_TIMESEND.Length)), "g", new CultureInfo("en-US"));
            string content = compressedString.Substring(indexContent + MARKUP_CONTENT.Length, indexEnd - (indexContent + MARKUP_CONTENT.Length));

            return new MessageSocket(userSender, timeSend, content);
        }

        /*
        public static bool IsMessageSocket_Old(in byte[] buffer, int end = 0)
        {
            if (end == 0)
                end = buffer.Length;
            string compressedString = Encoding.UTF8.GetString(buffer, 0, end);

            return IsMessageSocket(compressedString);

        }*/

        public static bool IsMessageSocket(in List<byte> buffer)
        {
            if (buffer == null || buffer.Count < MARKUP_END_BYTE.Length) return false;
            List<byte> b = buffer.GetRange(buffer.Count - MARKUP_END_BYTE.Length, MARKUP_END_BYTE.Length);
            for(int i = 0; i < b.Count; ++i)
            {
                if(b[i] != MARKUP_END_BYTE[i]) return false;
            }
            return true;
        }

        public static bool IsMessageSocket(in string str)
        {
            int indexUser = str.IndexOf(MARKUP_SENDER);
            int indexTimeSend = str.IndexOf(MARKUP_TIMESEND);
            int indexContent = str.IndexOf(MARKUP_CONTENT);
            int indexEnd = str.IndexOf(MARKUP_END);

            if (indexUser != -1 && indexTimeSend != -1 && indexContent != -1 && indexEnd != -1)
                return true;
            return false;
        }

        public static bool IsMessageLenghtNeeded(in byte[] datas)
        {
            return datas.Length > StateObject.buffersize;
        }

        public static int GetLengthMessage(in byte[] buffer, int end = 0)
        {
            if (end == 0)
                end = buffer.Length;

            string compressedString = Encoding.UTF8.GetString(buffer, 0, end);
            return GetLengthMessage(compressedString);
        }

        public static int GetLengthMessage(in string str)
        {
            int indexLength = str.IndexOf(MESSAGE_LENGHT);
            int indexEnd = str.IndexOf(MARKUP_END);

            if(indexEnd != -1 && indexLength != -1)
            {
                indexLength += MESSAGE_LENGHT.Length;
                string lnStr = str.Substring(indexLength, indexEnd - indexLength);
                return Convert.ToInt32(lnStr);
            }
            throw new Exception("data not found");
        }

        public static int GetLengthMessage(in MessageSocket ms)
        {
            int indexLength = ms.Content.IndexOf(MESSAGE_LENGHT);

            if (indexLength != -1)
            {
                indexLength += MESSAGE_LENGHT.Length;
                string lnStr = ms.Content.Substring(indexLength);
                return Convert.ToInt32(lnStr);
            }
            throw new Exception("not message length");
        }

        #region TYPED_MESSAGE

        private const string MESSAGE_LENGHT = "#!#MESSAGE_LENGHT#!#";
        private const string REQUEST_NAME = "#!#REQUEST_NAME#!#";
        private const string VALIDATE_CLIENT = "#!#VALIDATE_CLIENT#!#";
        private const string DISCONNECT_CLIENT = "#!#DISCONNECT_CLIENT#!#";

        #region CREATE_MESSAGE

        public static byte[] CreateRequestName(in string name)
        {
            return new MessageSocket(name, DateTime.Now, REQUEST_NAME).ToByte();
        }

        public static byte[] CreateValidationMessage(in string name)
        {
            return new MessageSocket(name, DateTime.Now, VALIDATE_CLIENT).ToByte();
        }

        public static byte[] CreateDisconnectMessage(in string name)
        {
            return new MessageSocket(name, DateTime.Now, DISCONNECT_CLIENT).ToByte();
        }

        public static byte[] CreateLengthMessage(in string name, int length)
        {
            return new MessageSocket(name, DateTime.Now, MESSAGE_LENGHT + length.ToString()).ToByte();
        }

        #endregion

        #region CHECK_MESSAGE

        public static bool IsRequestName(byte[] data, int end = 0)
        {
            return IsRequestName(TryCreate(data, end));
        }

        public static bool IsRequestName(string compressedString)
        {
            MessageSocket ms = TryCreate(compressedString);
            return IsRequestName(ms);
        }

        public static bool IsRequestName(MessageSocket ms)
        {
            if (ms.Content == REQUEST_NAME)
                return true;
            return false;
        }

        public static bool IsValidationMsg(byte[] data, int end = 0)
        {
            return IsValidationMsg(TryCreate(data, end));
        }

        public static bool IsValidationMsg(string compressedString)
        {
            MessageSocket ms = TryCreate(compressedString);
            return IsValidationMsg(ms);
        }

        public static bool IsValidationMsg(MessageSocket ms)
        {
            if (ms.Content == VALIDATE_CLIENT)
                return true;
            return false;
        }

        public static bool IsDisconnectMsg(byte[] data, int end = 0)
        {
            return IsDisconnectMsg(TryCreate(data, end));
        }

        public static bool IsDisconnectMsg(string compressedString)
        {
            MessageSocket ms = TryCreate(compressedString);
            return IsDisconnectMsg(ms);
        }

        public static bool IsDisconnectMsg(MessageSocket ms)
        {
            if (ms.Content == DISCONNECT_CLIENT)
                return true;
            return false;
        }

        public static bool IsMessageLength(byte[] buffer, int end = 0)
        {
            if (end == 0)
                end = buffer.Length;

            string compressedString = Encoding.UTF8.GetString(buffer, 0, end);
            return IsMessageLength(compressedString);
        }

        public static bool IsMessageLength(string str)
        {
            int indexUser = str.IndexOf(MARKUP_SENDER);
            int indexTimeSend = str.IndexOf(MARKUP_TIMESEND);
            int indexContent = str.IndexOf(MARKUP_CONTENT);
            int indexLength = str.IndexOf(MESSAGE_LENGHT);
            int indexEnd = str.IndexOf(MARKUP_END);

            if (indexUser != -1 && indexTimeSend != -1 && indexContent != -1 && indexEnd != -1 && indexLength != -1)
                return true;
            return false;
        }

        public static bool IsMessageLength(MessageSocket ms)
        {
            if(ms.Content == null) return false;
            int indexLength = ms.Content.IndexOf(MESSAGE_LENGHT);
            return indexLength != -1;
        }

#endregion

#endregion
    }
}
