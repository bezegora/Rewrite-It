using System;

namespace Rewrite_It
{
    public class Mistake
    {
        public MistakeType Type { get; private set; }
        public string Explanation { get; set; }

        public Mistake(MistakeType type) => Type = type;
        public Mistake() : this(MistakeType.None) { }
        public Mistake(MistakeType type, string explanation) : this(type) => Explanation = explanation;

        public void SetType(string mistakeName)
        {
            switch (mistakeName)
            {
                case "NoNumbers": Type = MistakeType.NoNumbers; break;
                case "IncorrectDefinitionTargetAudience": Type = MistakeType.IncorrectDefinitionTargetAudience; break;
                case "TooMuchDetails": Type = MistakeType.TooMuchDetails; break;
                case "ConfusedParagraphs": Type = MistakeType.ConfusedParagraphs; break;
                case "PseudoLanguage": Type = MistakeType.PseudoLanguage; break;
                default: throw new ArgumentException("Такого типа ошибки пока не существует");
            }
        }
    }
}
