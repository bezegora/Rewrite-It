using System;

namespace Rewrite_It
{
    public class Phrase
    {
        public string Text { get; }
        public int WaitingInMilliseconds { get; }
        public Action ExtraEvent { get; }
        public bool TalkingVisitor { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="waitingInMilliseconds"></param>
        /// <param name="talkingVisitor">По умолчанию true</param>
        /// <param name="extraEvent"></param>
        public Phrase(string text, int waitingInMilliseconds, bool talkingVisitor, Action extraEvent)
        {
            Text = text;
            WaitingInMilliseconds = waitingInMilliseconds;
            TalkingVisitor = talkingVisitor;
            ExtraEvent = extraEvent;
        }

        public Phrase(string text, int waitingInMilliseconds, bool talkingVisitor)
            : this(text, waitingInMilliseconds, talkingVisitor, null) { }

        public Phrase(string text, int waitingInMilliseconds, Action extraEvent)
            : this(text, waitingInMilliseconds, true, extraEvent) { }

        public Phrase(string text, int waitingInMilliseconds)
            : this(text, waitingInMilliseconds, true, null) { }
    }
}
